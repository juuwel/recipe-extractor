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
            .MapGroup("/recipe");

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

    private static async Task<IResult> CreateRecipe(
        [FromBody] ParsedRecipeDto parsedRecipeDto,
        [FromServices] IRecipeService recipeService
    )
    {
        await recipeService.SaveRecipeAsync(parsedRecipeDto);
        return Results.Ok(parsedRecipeDto); //  TODO: Change to 201 when get by id is implemented
    }

    private static async Task<IResult> RecipeWebhook(
        HttpContext context,
        [FromBody] NotionWebhookDto webhook,
        [FromServices] IRecipeService recipeService,
        [FromServices] WebhookAuthUtils authUtils
    )
    {
        // Verify webhook token
        var token = context.Request.Query["token"].FirstOrDefault();

        if (!authUtils.VerifyWebhookToken(token, context.Connection.RemoteIpAddress))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "Unauthorized",
                detail: "Invalid webhook token"
            );
        }

        // Process the webhook
        var webhookResponse = await recipeService.ProcessWebhookAsync(webhook);

        return webhookResponse.Success
            ? Results.Ok()
            : Results.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Webhook Processing Failed",
                detail: webhookResponse.Message
            );
    }
}
