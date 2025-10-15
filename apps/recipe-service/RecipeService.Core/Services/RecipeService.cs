using System.Text.Json;
using Microsoft.Extensions.Logging;
using RecipeService.Domain.DTOs;
using RecipeService.Domain.Entities;
using RecipeService.Infrastructure.Clients;

namespace RecipeService.Core.Services;

public class RecipeServiceImpl(
    NotionClient notionClient, 
    ExtractionServiceClient extractionServiceClient,
    ILogger<RecipeServiceImpl> logger) : IRecipeService
{
    public async Task<JsonDocument> SaveRecipeAsync(ParsedRecipeDto recipe, CancellationToken cancellationToken = default)
    {
        var notionEntity = MapToNotionEntity(recipe);
        return await notionClient.SaveRecipeEntityAsync(notionEntity, cancellationToken);
    }

    public async Task<(bool Success, string Message, string? RecipeName)> ProcessWebhookAsync(
        NotionWebhookDto webhook, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate webhook data
            if (webhook.Data?.Object != "page")
            {
                logger.LogInformation("Webhook ignored: not a page object");
                return (false, "No action needed - not a page object", null);
            }

            var pageId = webhook.Data.Id;
            var properties = webhook.Data.Properties;

            if (string.IsNullOrWhiteSpace(pageId) || properties == null)
            {
                logger.LogWarning("Webhook missing page ID or properties");
                return (false, "Invalid webhook data", null);
            }

            // Extract URL from Source property
            var url = properties.Source?.Type == "url" ? properties.Source.Url : null;

            // Extract recipe type from Tags multi_select
            var recipeType = "Main Dish"; // default
            if (properties.Tags?.Type == "multi_select" && 
                properties.Tags.MultiSelect?.Count > 0)
            {
                recipeType = properties.Tags.MultiSelect[0].Name ?? "Main Dish";
            }

            // Check if already processed
            var isProcessed = properties.Processed?.Type == "checkbox" && 
                            properties.Processed.Checkbox;

            logger.LogInformation(
                "Webhook data - Page ID: {PageId}, URL: {Url}, Recipe Type: {RecipeType}, Processed: {IsProcessed}", 
                pageId, url, recipeType, isProcessed);

            // Skip if already processed or no URL
            if (isProcessed)
            {
                logger.LogInformation("Recipe already processed, skipping");
                return (false, "Recipe already processed", null);
            }

            if (string.IsNullOrWhiteSpace(url))
            {
                logger.LogWarning("No URL found in webhook payload");
                return (false, "No URL to process", null);
            }

            // Parse the recipe using extraction service
            logger.LogInformation("Parsing recipe from {Url}", url);
            var parsedRecipe = await extractionServiceClient.ParseRecipeAsync(url, recipeType, cancellationToken);

            // Update the same page with parsed data
            logger.LogInformation("Updating Notion page {PageId} with parsed recipe data", pageId);
            var notionEntity = MapToNotionEntity(parsedRecipe);
            await notionClient.UpdateRecipeEntityAsync(pageId, notionEntity, cancellationToken);

            logger.LogInformation("Recipe processed and updated successfully: {RecipeName}", parsedRecipe.Name);
            return (true, "Recipe processed successfully", parsedRecipe.Name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing webhook");
            throw;
        }
    }

    /// <summary>
    /// Converts a ParsedRecipeDto to a RecipeNotionEntity for Notion storage
    /// </summary>
    private RecipeNotionEntity MapToNotionEntity(ParsedRecipeDto parsedRecipeDto)
    {
        return new RecipeNotionEntity
        {
            Name = parsedRecipeDto.Name,
            Source = parsedRecipeDto.Website,
            RecipeType = parsedRecipeDto.RecipeType,
            Processed = true,
            Ingredients = parsedRecipeDto.Ingredients,
            Instructions = parsedRecipeDto.Instructions,
            DatabaseId = parsedRecipeDto.DataSourceId,
            CreatedAt = DateTime.UtcNow
        };
    }
}