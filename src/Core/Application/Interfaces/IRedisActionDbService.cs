using Domain.Enums;
using StackExchange.Redis;

namespace Application.Interfaces
{
    public interface IRedisActionDbService
    {
        Task<bool> CreateUserAllowedAction(string username, RoleEnum role);
        Task<bool> GetUserAllowedAction(string allowedAction);
        Task<bool> AddUserTransactionAction(Guid transactionId, TransactionStatusEnum transactionStatus, string transactionData);
        Task<bool> GetUserTransactionAction(Guid transactionId, TransactionStatusEnum transactionStatus);
        Task<bool> DeleteUserTransactionAction(Guid transactionId, TransactionStatusEnum transactionStatus);
    }
}
