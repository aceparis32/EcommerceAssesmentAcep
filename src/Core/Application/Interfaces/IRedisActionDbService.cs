using Domain.Enums;
using StackExchange.Redis;

namespace Application.Interfaces
{
    public interface IRedisActionDbService
    {
        Task<bool> CreateUserAllowedAction(string username, RoleEnum role);
        Task<bool> GetUserAllowedAction(string allowedAction);
    }
}
