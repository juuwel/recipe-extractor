using System.ComponentModel.DataAnnotations;
using RecipeService.Domain.Enums;

namespace RecipeService.Domain.Entities;

/// <summary>
/// Represents a recipe entity structure that matches what will be stored in Notion.
/// This includes both the page properties and the content blocks that will be created.
/// </summary>
public class RecipeNotionEntity
{
    /// <summary>
    /// The recipe name that will be stored in the "Name" title property in Notion
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The source website URL that will be stored in the "Source" URL property in Notion
    /// </summary>
    [Required]
    public string Source { get; set; } = string.Empty;

    /// <summary>
    /// The recipe type that will be stored as a multi-select tag in the "Tags" property in Notion
    /// </summary>
    public RecipeType RecipeType { get; set; } = RecipeType.MainDish;

    /// <summary>
    /// Indicates if the recipe has been processed, stored as a checkbox in the "Processed" property in Notion
    /// </summary>
    public bool Processed { get; set; } = true;

    /// <summary>
    /// List of ingredients that will be rendered as bulleted list items under the "Ingredients" heading
    /// </summary>
    [Required]
    public List<string> Ingredients { get; set; } = [];

    /// <summary>
    /// List of instructions that will be rendered as numbered list items under the "Instructions" heading
    /// </summary>
    [Required]
    public List<string> Instructions { get; set; } = [];

    /// <summary>
    /// Notion page ID if the entity represents an existing page (for updates/archiving)
    /// </summary>
    public string? PageId { get; set; }

    /// <summary>
    /// The Notion database ID where this recipe will be stored
    /// </summary>
    public string? DatabaseId { get; set; }

    /// <summary>
    /// Timestamp when the recipe was created in Notion
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when the recipe was last updated in Notion
    /// </summary>
    public DateTime? LastUpdatedAt { get; set; }

    /// <summary>
    /// Creates the structured list of Notion blocks for the recipe content
    /// </summary>
    public List<NotionBlock> CreateNotionBlocks()
    {
        var blocks = new List<NotionBlock>
        {
            // Add Ingredients heading
            new NotionHeading2Block("Ingredients"),
        };

        // Add ingredient list items
        blocks.AddRange(Ingredients.Select(ingredient => new NotionBulletedListItemBlock(ingredient)));

        // Add Instructions heading
        blocks.Add(new NotionHeading2Block("Instructions"));

        // Add instruction list items
        blocks.AddRange(Instructions.Select(instruction => new NotionNumberedListItemBlock(instruction)));

        return blocks;
    }

    /// <summary>
    /// Validates that all required properties are present and valid
    /// </summary>
    public bool IsValid(out List<string> validationErrors)
    {
        validationErrors = [];

        if (string.IsNullOrWhiteSpace(Name))
            validationErrors.Add("Recipe name is required");

        if (string.IsNullOrWhiteSpace(Source))
            validationErrors.Add("Recipe source URL is required");

        if (!Uri.TryCreate(Source, UriKind.Absolute, out _))
            validationErrors.Add("Recipe source must be a valid URL");

        if (!Ingredients.Any())
            validationErrors.Add("At least one ingredient is required");

        if (!Instructions.Any())
            validationErrors.Add("At least one instruction is required");

        if (Ingredients.Any(string.IsNullOrWhiteSpace))
            validationErrors.Add("All ingredients must have content");

        if (Instructions.Any(string.IsNullOrWhiteSpace))
            validationErrors.Add("All instructions must have content");

        return !validationErrors.Any();
    }
}