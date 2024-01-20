using Application.Interfaces;
using Domain.Enums;
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
                    break;
                case RoleEnum.Customer:
                    allowedActions.Add("READ_PRODUCT");
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
    }
}
