using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecipeService.Infrastructure.Clients;
using RecipeService.Infrastructure.Options;

namespace RecipeService.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add HTTP Client
        services.AddHttpClient();
        
        // Configure Notion Client Options
        services.AddOptions<NotionClientOptions>()
            .Bind(configuration.GetSection("NotionClient"))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        // Register Infrastructure services
        services.AddScoped<NotionClient>();
        
        return services;
    }
}