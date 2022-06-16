using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

namespace HyperUI.Core;

/// <summary>
/// Represents a link.
/// </summary>
public class RichLink
{
    /// <summary>
    /// Default constructor for JSON serialization.
    /// </summary>
    public RichLink()
    {

    }

    /// <summary>
    /// Creates a new <see cref="RichLink"/>.
    /// </summary>
    /// <param name="id">Id.</param>
    /// <param name="description">Description.</param>
    /// <param name="icon">Icon.</param>
    public RichLink(string id, string? description = null, string? icon = null)
    {
        Description = description;
        Icon = icon;
        Id = id;
    }

    /// <summary>
    /// Id (URL.)
    /// </summary>
    [JsonPropertyName("@id")]
    public string? Id { get; set; }

    /// <summary>
    /// Description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Icon.
    /// </summary>
    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

    /// <summary>
    /// Gets the <see cref="OpenApiSchema"/> for <see cref="RichLink"/>.
    /// </summary>
    /// <returns><see cref="OpenApiSchema"/>.</returns>
    public static OpenApiSchema GetOpenApiSchema() => new()
    {
        Title = "Rich link",
        Type = "object",
        Properties = new Dictionary<string, OpenApiSchema>()
        {
            ["description"] = new OpenApiSchema
            {
                ReadOnly = true,
                Type = "string"
            },
            ["icon"] = new OpenApiSchema
            {
                ReadOnly = true,
                Type = "string",
                Description = "Location of an icon representing the link."
            }
        }
    };
}