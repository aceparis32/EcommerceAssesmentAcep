using Application.Features.AuditTrail.Models;
using Application.Interfaces;
using Application.Models;
using MediatR;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Application.Features.AuditTrail.Queries
{
    public class GetAuditTrailDatatableQuery : DatatableRequest, IRequest<DatatableResponse>
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }

    public class GetAuditTrailDatatableQueryHandler : IRequestHandler<GetAuditTrailDatatableQuery, DatatableResponse>
    {
        private readonly IMongoCollection<AuditTrailModel> auditTrailCollection;
        private readonly IRedisActionDbService redisActionDbService;

        public GetAuditTrailDatatableQueryHandler(IConfiguration configuration, IRedisActionDbService redisActionDbService)
        {
            var mongoClient = new MongoClient(configuration.GetSection("MongoDb:ConnectionString").Get<string>());
            var mongoDb = mongoClient.GetDatabase(configuration.GetSection("MongoDb:DatabaseName").Get<string>());
            auditTrailCollection = mongoDb.GetCollection<AuditTrailModel>(configuration.GetSection("MongoDb:AuditTrailCollectionName").Get<string>());
            this.redisActionDbService = redisActionDbService;
        }
        public async Task<DatatableResponse> Handle(GetAuditTrailDatatableQuery request, CancellationToken cancellationToken)
        {
            await redisActionDbService.GetUserAllowedAction("READ_AUDITTRAIL");

            var countFacet = AggregateFacet.Create("count",
                PipelineDefinition<AuditTrailModel, AggregateCountResult>.Create(new[]
                {
                    PipelineStageDefinitionBuilder.Count<AuditTrailModel>()
                }));
            var dataFacet = AggregateFacet.Create("data",
                PipelineDefinition<AuditTrailModel, AuditTrailModel>.Create(new[]
                {
                    PipelineStageDefinitionBuilder.Sort(Builders<AuditTrailModel>.Sort.Descending(x => x.CreatedDt)),
                    PipelineStageDefinitionBuilder.Skip<AuditTrailModel>(request.Start),
                    PipelineStageDefinitionBuilder.Limit<AuditTrailModel>(request.Length)
                }));

            // Filter
            var filter = Builders<AuditTrailModel>.Filter.Empty;
            if (!string.IsNullOrEmpty(request.Keyword))
                filter &= Builders<AuditTrailModel>.Filter.Regex(x => x.Name, new MongoDB.Bson.BsonRegularExpression(request.Keyword));
            if (request.DateFrom != null)
                filter &= Builders<AuditTrailModel>.Filter.Gt(x => x.CreatedDt, request.DateFrom.Value.Date);
            if (request.DateTo != null)
                filter &= Builders<AuditTrailModel>.Filter.Lt(x => x.CreatedDt, request.DateTo.Value.Date);

            var aggregationNonFiltered = await auditTrailCollection.Aggregate()
                .Facet(countFacet, dataFacet)
                .ToListAsync();

            var recordsTotal = aggregationNonFiltered.First()
                .Facets.First(x => x.Name == "count")
                .Output<AggregateCountResult>()?.FirstOrDefault()?.Count ?? 0;

            var aggregation = await auditTrailCollection.Aggregate()
                .Match(filter)
                .Facet(countFacet, dataFacet)
                .ToListAsync();

            var recordsFiltered = aggregation.First()
                .Facets.First(x => x.Name == "count")
                .Output<AggregateCountResult>()?.FirstOrDefault()?.Count ?? 0;

            var data = aggregation.First()
                .Facets.First(x => x.Name == "data")
                .Output<AuditTrailModel>();

            return new DatatableResponse
            {
                RecordsTotal = recordsTotal,
                RecordsFiltered = recordsFiltered,
                Data = data
            };
        }
    }
}
