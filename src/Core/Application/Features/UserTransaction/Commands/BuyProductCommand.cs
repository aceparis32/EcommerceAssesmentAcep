using Application.Exceptions;
using Application.Features.AuditTrail.Commands;
using Application.Features.ErrorLog.Commands;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.UserTransaction.Commands
{
    public class BuyProductCommand : IRequest<Unit>
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string Notes { get; set; } = string.Empty;
    }

    public class BuyProductCommandHandler : IRequestHandler<BuyProductCommand, Unit>
    {
        private readonly IApplicationDbContext dbContext;
        private readonly IUserRepositoryService userRepositoryService;
        private readonly IRedisActionDbService redisActionDbService;
        private readonly IMediator mediator;

        public BuyProductCommandHandler(IApplicationDbContext dbContext, IUserRepositoryService userRepositoryService, IRedisActionDbService redisActionDbService, IMediator mediator)
        {
            this.dbContext = dbContext;
            this.userRepositoryService = userRepositoryService;
            this.redisActionDbService = redisActionDbService;
            this.mediator = mediator;
        }
        public async Task<Unit> Handle(BuyProductCommand request, CancellationToken cancellationToken)
        {
            var userTransactionId = Guid.NewGuid();
            try
            {
                await redisActionDbService.GetUserAllowedAction("BUY_USER_TRANSACTION");

                var productQuery = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == request.ProductId);
                if (productQuery == null)
                    throw new BadRequestException("Product not found!");
                if (productQuery.Stock - request.Quantity < 0)
                    throw new BadRequestException("Quantity request invalid!");

                productQuery.Stock = productQuery.Stock - request.Quantity;
                dbContext.Products.Update(productQuery);

                var newUserTransaction = new Domain.Entities.UserTransaction
                {
                    Id = userTransactionId,
                    UserId = Guid.Parse(userRepositoryService.Id),
                    ProductId = request.ProductId,
                    TransactionStatus = Domain.Enums.TransactionStatusEnum.Bought,
                    Quantity = request.Quantity,
                    Notes = request.Notes,
                    Description = productQuery.Description,
                    CreatedBy = userRepositoryService.Username,
                    CreatedDt = DateTime.UtcNow,
                };

                await dbContext.UserTransactions.AddAsync(newUserTransaction, cancellationToken);
                
                // Create user cache to continue flow
                await redisActionDbService.AddUserTransactionAction(newUserTransaction.Id, Domain.Enums.TransactionStatusEnum.Bought, $"Created New Transaction (Buy), ID : {newUserTransaction.Id}");

                await dbContext.SaveChangesAsync(cancellationToken);

                // Create Audit Trail
                await mediator.Send(new CreateAuditTrailCommand
                {
                    Name = $"Created New Transaction (Buy), ID : {newUserTransaction.Id}"
                });

                return Unit.Value;
            }
            catch (Exception e)
            {
                await redisActionDbService.DeleteUserTransactionAction(userTransactionId, Domain.Enums.TransactionStatusEnum.Bought);
                await mediator.Send(new CreateErrorLogCommand
                {
                    ErrorMessage = e.Message
                });
                throw new BadRequestException(e.Message);
            }                
        }
    }
}
