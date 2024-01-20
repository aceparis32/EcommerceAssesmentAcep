using AssesmentAcep.WebAPI.Models;

namespace Microsoft.Extensions.DependencyInjection;

public static class MongoDbService
{
    public static IServiceCollection AddMongoDbService(this IServiceCollection services, IConfiguration configuration, string defaultSectionName = "MongoDb")
    {
        services.Configure<MongoDbSettingsModel>(configuration.GetSection(defaultSectionName));
        return services;
    }
}
