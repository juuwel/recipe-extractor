using Microsoft.AspNetCore.Mvc;

namespace RecipeService.Api.Apis;

public static class RecipeApi
{
    public static RouteGroupBuilder AddRecipeApi(this IEndpointRouteBuilder app)
    {
        var api = app
            .MapGroup("/api/v1/recipe")
            .RequireAuthorization();

        api.MapPost("/test", () => Results.Ok())
            .WithName("Test");

        app.MapPost("", () => CreateRecipe())
            .WithName("CreateRecipe");

        return api;
    }

    public static async Task<IResult> CreateRecipe()
    {
        await Task.Delay(1);
        return Results.Ok();
    }
}
