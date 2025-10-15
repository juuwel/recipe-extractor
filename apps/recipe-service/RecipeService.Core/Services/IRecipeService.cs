using System.Text.Json;
using RecipeService.Domain.DTOs;

namespace RecipeService.Core.Services;

public interface IRecipeService
{
    Task<JsonDocument> SaveRecipeAsync(ParsedRecipeDto recipe, CancellationToken cancellationToken = default);
}