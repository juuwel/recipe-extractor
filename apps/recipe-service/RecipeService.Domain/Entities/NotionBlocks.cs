using System.Text.Json.Serialization;

namespace RecipeService.Domain.Entities;

/// <summary>
/// Base class for all Notion block types
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type", IgnoreUnrecognizedTypeDiscriminators = true, UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]
[JsonDerivedType(typeof(NotionHeading2Block), "heading_2")]
[JsonDerivedType(typeof(NotionBulletedListItemBlock), "bulleted_list_item")]
[JsonDerivedType(typeof(NotionNumberedListItemBlock), "numbered_list_item")]
public abstract class NotionBlock
{
}

/// <summary>
/// Represents the rich_text array structure used in Notion blocks
/// </summary>
public class NotionRichTextContent(string content)
{
    [JsonPropertyName("rich_text")]
    public NotionRichTextItem[] RichText { get; set; } =
        [
            new(content)
        ];
}

/// <summary>
/// Represents a single rich text item
/// </summary>
public class NotionRichTextItem(string content)
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "text";

    [JsonPropertyName("text")]
    public NotionTextContent Text { get; set; } = new(content);
}

/// <summary>
/// Represents the text content within a rich text item
/// </summary>
public class NotionTextContent(string content)
{
    [JsonPropertyName("content")]
    public string Content { get; set; } = content;
}

/// <summary>
/// Represents a Notion heading_2 block
/// </summary>
public class NotionHeading2Block : NotionBlock
{
    public NotionHeading2Block(string content)
    {
        Heading2 = new NotionRichTextContent(content);
    }

    [JsonPropertyName("heading_2")]
    public NotionRichTextContent Heading2 { get; set; }
}

/// <summary>
/// Represents a Notion bulleted_list_item block
/// </summary>
public class NotionBulletedListItemBlock : NotionBlock
{
    public NotionBulletedListItemBlock(string content)
    {
        BulletedListItem = new NotionRichTextContent(content);
    }

    [JsonPropertyName("bulleted_list_item")]
    public NotionRichTextContent BulletedListItem { get; set; }
}

/// <summary>
/// Represents a Notion numbered_list_item block
/// </summary>
public class NotionNumberedListItemBlock : NotionBlock
{
    public NotionNumberedListItemBlock(string content)
    {
        NumberedListItem = new NotionRichTextContent(content);
    }

    [JsonPropertyName("numbered_list_item")]
    public NotionRichTextContent NumberedListItem { get; set; }
}