using System.ComponentModel.DataAnnotations;

namespace RecipeService.Infrastructure.Options;

public class NotionClientOptions
{
    [Required]
    [MinLength(1)]
    public required string ApiKey { get; set; }

    [Required]
    [MinLength(1)]
    public required string Version { get; set; }

    [Required]
    [MinLength(1)]
    public required string DatabaseId { get; set; }
}