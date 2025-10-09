using System.ComponentModel.DataAnnotations;

namespace RecipeService.Domain.DTOs;

public class ParsedRecipeDto
{
    [Required]
    public string Website { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public List<string> Ingredients { get; set; } = new();

    [Required]
    public List<string> Instructions { get; set; } = new();

    public RecipeType RecipeType { get; set; } = RecipeType.MainDish;
}