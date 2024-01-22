using Application.Exceptions;
using Application.Extensions;
using Application.Features.ErrorLog.Commands;
using Application.Features.User.Models;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.User.Commands
{
    public class UserAuthCommand : IRequest<GetUserAuthModel>
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class UserAuthCommandHandler : IRequestHandler<UserAuthCommand, GetUserAuthModel>
    {
        private readonly IApplicationDbContext dbContext;
        private readonly IConfiguration configuration;
        private readonly IRedisActionDbService redisActionDbService;
        private readonly IMediator mediator;

        public UserAuthCommandHandler(IApplicationDbContext dbContext, IConfiguration configuration, IRedisActionDbService redisActionDbService, IMediator mediator)
        {
            this.dbContext = dbContext;
            this.configuration = configuration;
            this.redisActionDbService = redisActionDbService;
            this.mediator = mediator;
        }
        public async Task<GetUserAuthModel> Handle(UserAuthCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userQuery = await dbContext.Users.FirstOrDefaultAsync(x => x.Username == request.Username);
                if (userQuery == null)
                    throw new BadRequestException("User not found!");

                if (userQuery.Password != string.Concat(request.Password).ToSHA256())
                    throw new BadRequestException("User not found!");

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
                var claims = new[]
                {
                    new Claim("Username", userQuery.Username),
                    new Claim("Fullname", userQuery.Fullname),
                    new Claim("Email", userQuery.Email)
                };

                var token = new JwtSecurityToken(
                    issuer: configuration["JwtSettings:Issuer"],
                    audience: configuration["JwtSettings:Issuer"],
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials);

                // Create or update user action to redis cache
                await redisActionDbService.CreateUserAllowedAction(userQuery.Username, userQuery.Role);

                return new GetUserAuthModel
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token)
                };
            }
            catch (Exception e)
            {
                await mediator.Send(new CreateErrorLogCommand
                {
                    ErrorMessage = e.Message
                });
                throw new BadRequestException(e.Message);
            }
        }
    }
}
