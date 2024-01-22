﻿using Application.Exceptions;
using Application.Features.AuditTrail.Commands;
using Application.Features.ErrorLog.Commands;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.UserTransaction.Commands
{
    public class ShipProductCommand : IRequest<Unit>
    {
        public Guid UserTransactionId { get; set; }
    }

    public class ShipProductCommandHandler : IRequestHandler<ShipProductCommand, Unit>
    {
        private readonly IApplicationDbContext dbContext;
        private readonly IUserRepositoryService userRepositoryService;
        private readonly IRedisActionDbService redisActionDbService;
        private readonly IMediator mediator;

        public ShipProductCommandHandler(IApplicationDbContext dbContext, IUserRepositoryService userRepositoryService, IRedisActionDbService redisActionDbService, IMediator mediator)
        {
            this.dbContext = dbContext;
            this.userRepositoryService = userRepositoryService;
            this.redisActionDbService = redisActionDbService;
            this.mediator = mediator;
        }

        public async Task<Unit> Handle(ShipProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await redisActionDbService.GetUserAllowedAction("SHIP_USER_TRANSACTION");
                await redisActionDbService.GetUserTransactionAction(request.UserTransactionId, Domain.Enums.TransactionStatusEnum.Paid);

                var userTransactionQuery = await dbContext.UserTransactions.FirstOrDefaultAsync(x => x.Id == request.UserTransactionId);
                if (userTransactionQuery == null)
                    throw new BadRequestException("User transaction not found!");
                if (userTransactionQuery.TransactionStatus != Domain.Enums.TransactionStatusEnum.Paid)
                    throw new BadRequestException("Invalid transaction status!");

                userTransactionQuery.TransactionStatus = Domain.Enums.TransactionStatusEnum.Shipped;
                userTransactionQuery.UpdatedBy = userRepositoryService.Fullname;
                userTransactionQuery.UpdatedDt = DateTime.UtcNow;
                dbContext.UserTransactions.Update(userTransactionQuery);
                await dbContext.SaveChangesAsync(cancellationToken);

                // Create Audit Trail
                await mediator.Send(new CreateAuditTrailCommand
                {
                    Name = $"Updated Transaction : {request.UserTransactionId} (Shipped)"
                });

                // Create user cache to continue flow
                await redisActionDbService.AddUserTransactionAction(userTransactionQuery.Id, Domain.Enums.TransactionStatusEnum.Shipped, $"Updated Transaction : {request.UserTransactionId} (Shipped)");

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
