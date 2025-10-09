using System.ComponentModel.DataAnnotations;

namespace RecipeService.Domain.DTOs;

public class RecipeRequestDto
{
    [Required]
    public string Url { get; set; } = string.Empty;

    [Required]
    public string RecipeType { get; set; } = string.Empty;
}