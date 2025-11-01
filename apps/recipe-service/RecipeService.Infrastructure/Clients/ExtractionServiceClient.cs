using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RecipeService.Domain.DTOs;

namespace RecipeService.Infrastructure.Clients;

/// <summary>
/// Client for calling the extraction service to parse recipes
/// </summary>
public class ExtractionServiceClient(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    ILogger<ExtractionServiceClient> logger)
{
    private readonly string _baseUrl = configuration["ExtractionService:BaseUrl"]
                                       ?? throw new InvalidOperationException("ExtractionService:BaseUrl configuration is missing");

    /// <summary>
    /// Parse a recipe from a URL
    /// </summary>
    public async Task<ParsedRecipeDto> ParseRecipeAsync(string url, string recipeType, CancellationToken cancellationToken = default)
    {
        using var httpClient = httpClientFactory.CreateClient();

        var request = new RecipeRequestDto
        {
            Url = url,
            RecipeType = recipeType
        };

        logger.LogInformation("Calling extraction service to parse recipe from {Url}", url);

        var response = await httpClient.PostAsJsonAsync($"{_baseUrl}/recipe/parse", request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError("Extraction service failed: {StatusCode} - {Error}", response.StatusCode, errorContent);
            throw new HttpRequestException($"Failed to parse recipe: {response.StatusCode} - {errorContent}");
        }

        var parsedRecipe = await response.Content.ReadFromJsonAsync<ParsedRecipeDto>(cancellationToken)
            ?? throw new InvalidOperationException("Failed to deserialize parsed recipe response");
        logger.LogInformation("Successfully parsed recipe: {RecipeName}", parsedRecipe.Name);

        return parsedRecipe;
    }
}
