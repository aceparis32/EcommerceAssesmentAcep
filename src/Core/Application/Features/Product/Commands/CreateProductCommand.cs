using Application.Exceptions;
using Application.Features.AuditTrail.Commands;
using Application.Features.ErrorLog.Commands;
using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Product.Commands
{
    public class CreateProductCommand : IRequest<Unit>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public double Rating { get; set; }
        public int Stock { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }

    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Unit>
    {
        private readonly IApplicationDbContext dbContext;
        private readonly IUserRepositoryService userRepository;
        private readonly IMediator mediator;
        private readonly IRedisActionDbService redisActionDbService;

        public CreateProductCommandHandler(IApplicationDbContext dbContext, IUserRepositoryService userRepository, IMediator mediator, IRedisActionDbService redisActionDbService)
        {
            this.dbContext = dbContext;
            this.userRepository = userRepository;
            this.mediator = mediator;
            this.redisActionDbService = redisActionDbService;
        }
        public async Task<Unit> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await redisActionDbService.GetUserAllowedAction("CREATE_PRODUCT");

                var productQuery = await dbContext.Products.FirstOrDefaultAsync(x => x.Name.ToLower() == request.Name.ToLower() && x.DeletedDt == null);
                if (productQuery != null)
                    throw new Exception("Product already exist!");

                var newProduct = new Domain.Entities.Product
                {
                    Name = request.Name,
                    Description = request.Description,
                    Price = request.Price,
                    Rating = request.Rating,
                    Stock = request.Stock,
                    Brand = request.Brand,
                    Category = request.Category,
                    CreatedBy = userRepository.Fullname,
                    CreatedDt = DateTime.UtcNow,
                };
                await dbContext.Products.AddAsync(newProduct);
                await dbContext.SaveChangesAsync(cancellationToken);

                // Create Audit Trail
                await mediator.Send(new CreateAuditTrailCommand
                {
                    Name = $"Created Product : {Newtonsoft.Json.JsonConvert.SerializeObject(newProduct)}"
                });

                return Unit.Value;
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
