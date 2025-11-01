using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RecipeService.Domain.Entities;
using RecipeService.Domain.Enums;
using RecipeService.Infrastructure.Options;

namespace RecipeService.Infrastructure.Clients;

public class NotionClient(
    IHttpClientFactory httpClientFactory,
    IOptions<NotionClientOptions> notionClientOptions,
    ILogger<NotionClient> logger
    )
{
    private const string BaseUrl = "https://api.notion.com/v1/";
    private const string PageApiUrl = BaseUrl + "pages/";

    private void AddRequestHeaders(HttpClient httpClient)
    {
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {notionClientOptions.Value.ApiKey}");
        httpClient.DefaultRequestHeaders.Add("Notion-Version", notionClientOptions.Value.Version);
    }

    public async Task<JsonDocument> SaveRecipeEntityAsync(RecipeNotionEntity recipeEntity, CancellationToken cancellationToken = default)
    {
        using var httpClient = httpClientFactory.CreateClient();
        AddRequestHeaders(httpClient);

        var blocks = recipeEntity.CreateNotionBlocks();

        var requestBody = new
        {
            parent = new { data_source_id = notionClientOptions.Value.DatabaseId }, // TODO: take this value from request when ready
            properties = new
            {
                Name = new { title = new[] { new { text = new { content = recipeEntity.Name } } } },
                Source = new { url = recipeEntity.Source },
                Tags = new { multi_select = new[] { new { name = recipeEntity.RecipeType.ToDisplayString() } } },
                Processed = new { checkbox = recipeEntity.Processed }
            },
            children = blocks
        };

        var jsonOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
            WriteIndented = true,
        };

        logger.LogDebug("Notion API Request Body: {JsonString}", JsonSerializer.Serialize(requestBody, jsonOptions));

        var response = await httpClient.PostAsJsonAsync(PageApiUrl, requestBody, jsonOptions, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Failed to create entry: {errorContent}");
        }

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var jsonDocument = JsonDocument.Parse(responseContent);

        // Update the entity with the returned page ID and timestamps
        if (jsonDocument.RootElement.TryGetProperty("id", out var idProperty))
        {
            recipeEntity.PageId = idProperty.GetString();
        }

        if (jsonDocument.RootElement.TryGetProperty("created_time", out var createdTimeProperty))
        {
            if (DateTime.TryParse(createdTimeProperty.GetString(), out var createdTime))
            {
                recipeEntity.CreatedAt = createdTime;
            }
        }

        if (jsonDocument.RootElement.TryGetProperty("last_edited_time", out var lastEditedTimeProperty))
        {
            if (DateTime.TryParse(lastEditedTimeProperty.GetString(), out var lastEditedTime))
            {
                recipeEntity.LastUpdatedAt = lastEditedTime;
            }
        }

        return jsonDocument;
    }

    public async Task UpdateRecipeEntityAsync(string pageId, RecipeNotionEntity recipeEntity,
        CancellationToken cancellationToken = default)
    {
        // First create the new recipe
        var saveResponse = await SaveRecipeEntityAsync(recipeEntity, cancellationToken);

        if (!saveResponse.RootElement.TryGetProperty("id", out _))
        {
            throw new InvalidOperationException("Failed to save recipe before archiving old page.");
        }

        // Then archive the old page
        await ArchivePageAsync(pageId, cancellationToken);
    }

    private async Task ArchivePageAsync(string pageId, CancellationToken cancellationToken = default)
    {
        using var httpClient = httpClientFactory.CreateClient();
        AddRequestHeaders(httpClient);

        var url = $"{PageApiUrl}{pageId}";
        var requestBody = new { archived = true };

        var response = await httpClient.PatchAsJsonAsync(url, requestBody, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Failed to archive page: {errorContent}");
        }

        await response.Content.ReadAsStringAsync(cancellationToken);
    }
}
