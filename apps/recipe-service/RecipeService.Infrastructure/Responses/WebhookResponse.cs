namespace RecipeService.Infrastructure.Responses;

public readonly struct WebhookResponse(bool success, string message)
{
    public bool Success { get; init; } = success;
    public string Message { get; init; } = message;
}
