using System.ComponentModel.DataAnnotations;
using RecipeService.Domain.Enums;

namespace RecipeService.Domain.DTOs;

public class ParsedRecipeDto
{
    [Required]
    public string DataSourceId { get; set; } = string.Empty;

    [Required]
    public string Website { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public List<string> Ingredients { get; set; } = [];

    [Required]
    public List<string> Instructions { get; set; } = [];

    public RecipeType RecipeType { get; set; } = RecipeType.MainDish;
}