using Application.Interfaces;
using AssesmentAcep.WebAPI.Models;
using StackExchange.Redis;

namespace Microsoft.Extensions.DependencyInjection;

public static class RedisService
{
    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration, string redisConfigName = "RedisOptions")
    {
        var option = configuration.GetSection(redisConfigName).Get<RedisOptionModel>();
        var config = new ConfigurationOptions
        {
            EndPoints =
                {
                    { option.Host, option.Port },
                },
            DefaultDatabase = option.Database,
            Password = option.Password
        };

        var multiplexer = ConnectionMultiplexer.Connect(config);
        services.AddSingleton<IConnectionMultiplexer>(multiplexer);

        return services;
    }
}
