using Application.Exceptions;
using Application.Extensions;
using Application.Features.User.Models;
using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Application.Features.User.Commands
{
    public class CreateUserCommand : CreateUserModel, IRequest<Unit>
    {

    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Unit>
    {
        private readonly IApplicationDbContext dbContext;

        public CreateUserCommandHandler(IApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Unit> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var userQuery = await dbContext.Users.FirstOrDefaultAsync(x => x.Username == request.Username);
            if (userQuery != null)
                throw new BadRequestException("User already registered!");

            PasswordValidation(request.Password);

            var newUser = new Domain.Entities.User
            {
                Username = request.Username,
                Password = string.Concat(request.Password).ToSHA256(),
                Fullname = request.Fullname,
                Email = request.Email,
                Phone = request.Phone,
                Role = request.Role,
                CreatedBy = "system",
                CreatedDt = DateTime.UtcNow,
            };

            await dbContext.Users.AddAsync(newUser);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

        private void PasswordValidation(String password)
        {
            // Password must be equal or more than 8 words
            if (password.Length < 8)
                throw new BadRequestException("Invalid Password : Password must be more than 8 words!");

            // ReGex to check if a string contains lowercase, uppercase, and numeric value
            string regex = "^(?=.*[a-z])" +
                        "(?=.*[A-Z])" +
                        "(?=.*\\d)";
            var regexValidation = new Regex(regex);
            var match = regexValidation.Match(password);

            if (!match.Success)
                throw new BadRequestException($"Invalid Password : Password must contains lowercase, uppercase, and numeric value!");
        }
    }
}
