using Application.Interfaces;
using Domain.Enums;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace AssesmentAcep.WebAPI.Services
{
    public class RedisActionDbService : IRedisActionDbService
    {
        private readonly IApplicationDbContext dbContext;
        private readonly IUserRepositoryService userRepositoryService;
        private readonly IConnectionMultiplexer connectionMultiplexer;
        private readonly IDatabase redisDb;

        public RedisActionDbService(IApplicationDbContext dbContext, IUserRepositoryService userRepositoryService, IConnectionMultiplexer connectionMultiplexer)
        {
            this.dbContext = dbContext;
            this.userRepositoryService = userRepositoryService;
            this.connectionMultiplexer = connectionMultiplexer;
            redisDb = this.connectionMultiplexer.GetDatabase();
        }

        public async Task<bool> CreateUserAllowedAction(string username, RoleEnum role)
        {
            var allowedActions = new List<string>();
            switch (role)
            {
                case RoleEnum.Admin:
                    allowedActions.Add("CREATE_PRODUCT");
                    allowedActions.Add("READ_PRODUCT");
                    allowedActions.Add("UPDATE_PRODUCT");
                    allowedActions.Add("DELETE_PRODUCT");
                    allowedActions.Add("READ_AUDITTRAIL");
                    break;
                case RoleEnum.Seller:
                    allowedActions.Add("CREATE_PRODUCT");
                    allowedActions.Add("READ_PRODUCT");
                    allowedActions.Add("UPDATE_PRODUCT");
                    allowedActions.Add("DELETE_PRODUCT");
                    allowedActions.Add("SHIP_USER_TRANSACTION");
                    break;
                case RoleEnum.Customer:
                    allowedActions.Add("READ_PRODUCT");
                    allowedActions.Add("BUY_USER_TRANSACTION");
                    allowedActions.Add("PAY_USER_TRANSACTION");
                    allowedActions.Add("ACCEPT_USER_TRANSACTION");
                    allowedActions.Add("COMPLETE_USER_TRANSACTION");
                    allowedActions.Add("REJECT_USER_TRANSACTION");
                    allowedActions.Add("CANCEL_USER_TRANSACTION");
                    break;
                default:
                    return false;
            }

            var expiredCache = DateTime.UtcNow.AddDays(1) - DateTime.UtcNow;
            await redisDb.StringSetAsync($"allowed-action-{username}", JsonConvert.SerializeObject(allowedActions), expiredCache);
            return true;
        }

        public async Task<bool> GetUserAllowedAction(string allowedAction)
        {
            var userAllowedDataExist = await redisDb.StringGetAsync($"allowed-action-{userRepositoryService.Username}");
            if (!userAllowedDataExist.HasValue)
                throw new Exception("User data has been updated. Please login again!");

            List<string> allowedActions = JsonConvert.DeserializeObject<List<string>>(userAllowedDataExist.ToString());

            if (!allowedActions.Contains(allowedAction))
                throw new Exception("Forbidden");

            return true;
        }

        public async Task<bool> AddUserTransactionAction(Guid transactionId, TransactionStatusEnum transactionStatus, string transactionData)
        {
            switch (transactionStatus)
            {
                case TransactionStatusEnum.Bought:
                    var expiredCache = DateTime.UtcNow.AddHours(1) - DateTime.UtcNow;
                    await redisDb.StringSetAsync($"{transactionId}-{transactionStatus}", transactionData, expiredCache);
                    break;
                case TransactionStatusEnum.Paid:
                    await redisDb.KeyDeleteAsync($"{transactionId}-{TransactionStatusEnum.Bought}");
                    await redisDb.StringSetAsync($"{transactionId}-{transactionStatus}", transactionData);
                    break;
                case TransactionStatusEnum.Shipped:
                    await redisDb.KeyDeleteAsync($"{transactionId}-{TransactionStatusEnum.Paid}");
                    await redisDb.StringSetAsync($"{transactionId}-{transactionStatus}", transactionData);
                    break;
                case TransactionStatusEnum.Accepted:
                    await redisDb.KeyDeleteAsync($"{transactionId}-{TransactionStatusEnum.Shipped}");
                    await redisDb.StringSetAsync($"{transactionId}-{transactionStatus}", transactionData);
                    break;
                case TransactionStatusEnum.Completed:
                    await redisDb.KeyDeleteAsync($"{transactionId}-{TransactionStatusEnum.Accepted}");
                    break;
                case TransactionStatusEnum.Rejected:
                    await redisDb.KeyDeleteAsync($"{transactionId}-{TransactionStatusEnum.Completed}");
                    break;
                case TransactionStatusEnum.Cancelled:
                    await redisDb.KeyDeleteAsync($"{transactionId}-{TransactionStatusEnum.Bought}");
                    break;
                default:
                    throw new Exception("Invalid transaction status!");
            }

            return true;
        }

        public async Task<bool> GetUserTransactionAction(Guid transactionId, TransactionStatusEnum transactionStatus)
        {
            bool isCacheExist = await redisDb.KeyExistsAsync($"{transactionId}-{transactionStatus}");
            if (!isCacheExist)
                throw new Exception("User transaction data not found!");

            return true;
        }
    }
}
