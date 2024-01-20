using Application.Features.AuditTrail.Models;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Application.Features.AuditTrail.Commands
{
    public class CreateAuditTrailCommand : IRequest<Unit>
    {
        public string Name { get; set; } = string.Empty;
    }

    public class CreateAuditTrailCommandHandler : IRequestHandler<CreateAuditTrailCommand, Unit>
    {
        private readonly IMongoCollection<AuditTrailModel> auditTrailCollection;
        private readonly IUserRepositoryService userRepositoryService;

        public CreateAuditTrailCommandHandler(IConfiguration configuration, IUserRepositoryService userRepositoryService)
        {
            var mongoClient = new MongoClient(configuration.GetSection("MongoDb:ConnectionString").Get<string>());
            var mongoDb = mongoClient.GetDatabase(configuration.GetSection("MongoDb:DatabaseName").Get<string>());
            auditTrailCollection = mongoDb.GetCollection<AuditTrailModel>(configuration.GetSection("MongoDb:CollectionName").Get<string>());
            this.userRepositoryService = userRepositoryService;
        }

        public async Task<Unit> Handle(CreateAuditTrailCommand request, CancellationToken cancellationToken)
        {
            var newAuditTrail = new AuditTrailModel
            {
                Name = request.Name,
                CreatedBy = userRepositoryService.Fullname,
                CreatedDt = DateTime.UtcNow,
            };

            await auditTrailCollection.InsertOneAsync(newAuditTrail);

            return Unit.Value;
        }
    }
}
