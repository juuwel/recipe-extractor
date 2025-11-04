
using RecipeService.Api.FIlters;

namespace RecipeService.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        // Add API-specific services
        services.AddOpenApi();
        services.AddHealthChecks();

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }
}
