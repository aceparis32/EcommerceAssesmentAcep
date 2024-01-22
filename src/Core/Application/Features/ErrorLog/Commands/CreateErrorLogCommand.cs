using Amazon.Runtime.Internal;
using Application.Features.AuditTrail.Models;
using Application.Features.ErrorLog.Models;
using Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ErrorLog.Commands
{
    public class CreateErrorLogCommand : IRequest<bool>
    {
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public class CreateErrorLogCommandHandler : IRequestHandler<CreateErrorLogCommand, bool>
    {
        private readonly IUserRepositoryService userRepositoryService;
        private readonly IMongoCollection<ErrorLogModel> errorLogCollection;
        public CreateErrorLogCommandHandler(IConfiguration configuration, IUserRepositoryService userRepositoryService)
        {
            this.userRepositoryService = userRepositoryService;
            var mongoClient = new MongoClient(configuration.GetSection("MongoDb:ConnectionString").Get<string>());
            var mongoDb = mongoClient.GetDatabase(configuration.GetSection("MongoDb:DatabaseName").Get<string>());
            errorLogCollection = mongoDb.GetCollection<ErrorLogModel>(configuration.GetSection("MongoDb:ErrorLogCollectionName").Get<string>());
        }

        public async Task<bool> Handle(CreateErrorLogCommand request, CancellationToken cancellationToken)
        {
            var newErrorLog = new ErrorLogModel
            {
                Message = request.ErrorMessage,
                CreatedBy = userRepositoryService.Fullname,
                CreatedDt = DateTime.UtcNow,
            };

            await errorLogCollection.InsertOneAsync(newErrorLog);
            return true;
        }
    }
}
