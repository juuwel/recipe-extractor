using System.Text.Json;
using RecipeService.Domain.DTOs;
using RecipeService.Infrastructure.Responses;

namespace RecipeService.Core.Services;

public interface IRecipeService
{
    Task<JsonDocument> SaveRecipeAsync(ParsedRecipeDto recipe, CancellationToken cancellationToken = default);

    Task<WebhookResponse> ProcessWebhookAsync(NotionWebhookDto webhook, CancellationToken cancellationToken = default);
}
