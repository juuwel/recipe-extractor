using Microsoft.Extensions.DependencyInjection;
using RecipeService.Core.Services;

namespace RecipeService.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddTransient<IRecipeService, RecipeServiceImpl>();
        
        return services;
    }
}