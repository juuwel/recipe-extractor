using System.Text.Json.Serialization;

namespace RecipeService.Domain.DTOs;

/// <summary>
/// Represents the webhook payload from Notion
/// </summary>
public class NotionWebhookDto
{
    [JsonPropertyName("data")]
    public NotionWebhookData? Data { get; set; }
}

public class NotionWebhookData
{
    [JsonPropertyName("object")]
    public string? Object { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("properties")]
    public NotionPageProperties? Properties { get; set; }
}

public class NotionPageProperties
{
    [JsonPropertyName("Source")]
    public NotionUrlProperty? Source { get; set; }

    [JsonPropertyName("Tags")]
    public NotionMultiSelectProperty? Tags { get; set; }

    [JsonPropertyName("Processed")]
    public NotionCheckboxProperty? Processed { get; set; }
}

public class NotionUrlProperty
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

public class NotionMultiSelectProperty
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("multi_select")]
    public List<NotionSelectOption>? MultiSelect { get; set; }
}

public class NotionSelectOption
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

public class NotionCheckboxProperty
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("checkbox")]
    public bool Checkbox { get; set; }
}
