using Microsoft.AspNetCore.Mvc;
using RecipeService.Core.Services;
using RecipeService.Domain.DTOs;

namespace RecipeService.Api.Apis;

public static class RecipeApi
{
    public static RouteGroupBuilder AddRecipeApi(this IEndpointRouteBuilder app)
    {
        var api = app
            .MapGroup("/api/v1/recipe");

        api.MapPost("/", CreateRecipe)
            .WithName("CreateRecipe")
            .Produces<ParsedRecipeDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        api.MapPost("/webhook", RecipeWebhook)
            .WithName("RecipeWebhook")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        app.MapPost("", () => CreateRecipe())
            .WithName("CreateRecipe");

        return api;
    }

    public static async Task<IResult> CreateRecipe(
        [FromBody] ParsedRecipeDto parsedRecipeDto,
        [FromServices] IRecipeService recipeService
    )
    {
        await recipeService.SaveRecipeAsync(parsedRecipeDto);
        return Results.Ok(parsedRecipeDto);
    }

    public static async Task<IResult> RecipeWebhook(
        [FromServices] IRecipeService recipeService
    )
    {
        // Handle webhook logic here
        return Results.Ok("Webhook received");
    }
}
