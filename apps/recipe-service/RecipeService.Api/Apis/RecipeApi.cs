namespace RecipeService.Api.Apis;

public static class RecipeApi
{
    public static RouteGroupBuilder AddTaskApi(this IEndpointRouteBuilder app)
    {
        var api = app
            .MapGroup("/api/v1/recipe")
            .RequireAuthorization();

        api.MapPost("/test", () => Results.Ok())
            .WithName("Test");

        return api;
    }
}
