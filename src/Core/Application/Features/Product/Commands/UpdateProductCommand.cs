using Application.Features.AuditTrail.Commands;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Product.Commands
{
    public class UpdateProductCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public double Rating { get; set; }
        public int Stock { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }

    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
    {
        private readonly IApplicationDbContext dbContext;
        private readonly IUserRepositoryService userRepository;
        private readonly IMediator mediator;
        private readonly IRedisActionDbService redisActionDbService;

        public UpdateProductCommandHandler(IApplicationDbContext dbContext, IUserRepositoryService userRepository, IMediator mediator, IRedisActionDbService redisActionDbService)
        {
            this.dbContext = dbContext;
            this.userRepository = userRepository;
            this.mediator = mediator;
            this.redisActionDbService = redisActionDbService;
        }
        public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            await redisActionDbService.GetUserAllowedAction("UPDATE_PRODUCT");

            var productQuery = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == request.Id && x.DeletedDt == null);
            if (productQuery == null)
                throw new Exception("Product not found!");

            productQuery.Name = request.Name;
            productQuery.Description = request.Description;
            productQuery.Price = request.Price;
            productQuery.Rating = request.Rating;
            productQuery.Stock = request.Stock;
            productQuery.Brand = request.Brand;
            productQuery.Category = request.Category;
            productQuery.UpdatedBy = userRepository.Fullname;
            productQuery.UpdatedDt = DateTime.UtcNow;

            dbContext.Products.Update(productQuery);
            await dbContext.SaveChangesAsync(cancellationToken);

            // Create Audit Trail
            await mediator.Send(new CreateAuditTrailCommand
            {
                Name = $"Updated Product : {Newtonsoft.Json.JsonConvert.SerializeObject(productQuery)}"
            });

            return Unit.Value;
        }
    }
}
