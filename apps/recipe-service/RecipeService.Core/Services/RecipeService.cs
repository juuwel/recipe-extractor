using System.Text.Json;
using RecipeService.Domain.DTOs;
using RecipeService.Domain.Entities;
using RecipeService.Infrastructure.Clients;

namespace RecipeService.Core.Services;

public class RecipeServiceImpl(NotionClient notionClient) : IRecipeService
{
    public async Task<JsonDocument> SaveRecipeAsync(ParsedRecipeDto recipe, CancellationToken cancellationToken = default)
    {
        var notionEntity = MapToNotionEntity(recipe);
        return await notionClient.SaveRecipeEntityAsync(notionEntity, cancellationToken);
    }


    /// <summary>
    /// Converts a RecipeNotionEntity to a RecipeNotionEntity for Notion storage
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