using Microsoft.AspNetCore.Mvc;
using RecipeService.Core.Services;
using RecipeService.Domain.DTOs;
using RecipeService.Infrastructure.Utils;

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
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

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
        HttpContext context,
        [FromBody] NotionWebhookDto webhook,
        [FromServices] IRecipeService recipeService,
        [FromServices] WebhookAuthUtils authUtils,
        [FromServices] ILogger logger
    )
    {
        // Verify webhook token
        var token = context.Request.Query["token"].FirstOrDefault();
        
        if (!authUtils.VerifyWebhookToken(token))
        {
            logger.LogWarning("Invalid webhook token received from IP: {IpAddress}", 
                context.Connection.RemoteIpAddress);
            return Results.Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "Unauthorized",
                detail: "Invalid webhook token"
            );
        }

        logger.LogInformation("Received valid Notion webhook");

        // Process the webhook
        var (success, message, recipeName) = await recipeService.ProcessWebhookAsync(webhook);

        if (success)
        {
            return Results.Ok(new { status = "success", recipe_name = recipeName });
        }

        return Results.Ok(new { status = "ignored", message });
    }
}
