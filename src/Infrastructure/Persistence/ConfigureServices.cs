using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistence;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration, string defaultConfigName = "DefaultConection")
    {
        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql("Server=localhost;Port=5432;Database=ecommerce_assesment;User ID=postgres;Password=Aceparis32;",
            builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

        return services;
    }
    
}
