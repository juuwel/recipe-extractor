using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RecipeService.Domain.DTOs;

namespace RecipeService.Infrastructure.Clients;

public class NotionClient(IHttpClientFactory httpClientFactory, IOptions<NotionClientOptions> notionClientOptions)
{
    private const string BaseUrl = "https://api.notion.com/v1/";
    private const string PageApiUrl = BaseUrl + "pages/";

    private void AddRequestHeaders(HttpClient httpClient)
    {
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {notionClientOptions.Value.ApiKey}");
        httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
        httpClient.DefaultRequestHeaders.Add("Notion-Version", notionClientOptions.Value.Version);
    }

    private List<object> PrepareChildrenBlocks(ParsedRecipeDto recipe)
    {
        var blocks = new List<object>
        {
            // Add Ingredients heading
            new
            {
                type = "heading_2",
                heading_2 = new
                {
                    rich_text = new[]
                {
                    new { type = "text", text = new { content = "Ingredients" } }
                }
                }
            }
        };

        // Add ingredient list items
        foreach (var ingredient in recipe.Ingredients)
        {
            blocks.Add(new
            {
                type = "bulleted_list_item",
                bulleted_list_item = new
                {
                    rich_text = new[]
                    {
                        new { type = "text", text = new { content = ingredient } }
                    }
                }
            });
        }

        // Add Instructions heading
        blocks.Add(new
        {
            type = "heading_2",
            heading_2 = new
            {
                rich_text = new[]
                {
                    new { type = "text", text = new { content = "Instructions" } }
                }
            }
        });

        // Add instruction list items
        foreach (var instruction in recipe.Instructions)
        {
            blocks.Add(new
            {
                type = "numbered_list_item",
                numbered_list_item = new
                {
                    rich_text = new[]
                    {
                        new { type = "text", text = new { content = instruction } }
                    }
                }
            });
        }

        return blocks;
    }

    public async Task<JsonDocument> SaveRecipeAsync(ParsedRecipeDto recipe, CancellationToken cancellationToken = default)
    {
        using var httpClient = httpClientFactory.CreateClient();
        AddRequestHeaders(httpClient);

        var requestBody = new
        {
            parent = new { data_source_id = notionClientOptions.Value.DatabaseId },
            properties = new
            {
                Name = new { title = new[] { new { text = new { content = recipe.Name } } } },
                Source = new { url = recipe.Website },
                Tags = new { multi_select = new[] { new { name = recipe.RecipeType } } },
                Processed = new { checkbox = true }
            },
            children = PrepareChildrenBlocks(recipe)
        };

        var response = await httpClient.PostAsJsonAsync(PageApiUrl, requestBody, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Failed to create entry: {errorContent}");
        }

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonDocument.Parse(responseContent);
    }

    public async Task<JsonDocument> UpdateRecipeAsync(string pageId, ParsedRecipeDto recipe, CancellationToken cancellationToken = default)
    {
        var saveResponse = await SaveRecipeAsync(recipe, cancellationToken);
        
        if (!saveResponse.RootElement.TryGetProperty("id", out _))
        {
            throw new InvalidOperationException("Failed to save recipe before archiving old page.");
        }

        await ArchivePageAsync(pageId, cancellationToken);
        return saveResponse;
    }

    public async Task<JsonDocument> ArchivePageAsync(string pageId, CancellationToken cancellationToken = default)
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

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonDocument.Parse(responseContent);
    }
}