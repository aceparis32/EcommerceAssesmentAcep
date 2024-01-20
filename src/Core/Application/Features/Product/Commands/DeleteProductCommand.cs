using Application.Features.AuditTrail.Commands;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson.IO;
using Newtonsoft.Json;

namespace Application.Features.Product.Commands
{
    public class DeleteProductCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }

    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Unit>
    {
        private readonly IApplicationDbContext dbContext;
        private readonly IUserRepositoryService userRepository;
        private readonly IMediator mediator;
        private readonly IRedisActionDbService redisActionDbService;

        public DeleteProductCommandHandler(IApplicationDbContext dbContext, IUserRepositoryService userRepository, IMediator mediator, IRedisActionDbService redisActionDbService)
        {
            this.dbContext = dbContext;
            this.userRepository = userRepository;
            this.mediator = mediator;
            this.redisActionDbService = redisActionDbService;
        }
        public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            await redisActionDbService.GetUserAllowedAction("DELETE_PRODUCT");

            var productQuery = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == request.Id && x.DeletedDt == null);
            if (productQuery == null)
                throw new Exception("Product not found!");

            productQuery.UpdatedDt = DateTime.UtcNow;
            productQuery.DeletedDt = DateTime.UtcNow;
            productQuery.UpdatedBy = userRepository.Fullname;
            productQuery.DeletedBy = userRepository.Fullname;

            dbContext.Products.Update(productQuery);
            await dbContext.SaveChangesAsync(cancellationToken);

            // Create Audit Trail
            await mediator.Send(new CreateAuditTrailCommand
            {
                Name = $"Deleted Product : {productQuery.Id}"
            });
            return Unit.Value;
        }
    }
}
