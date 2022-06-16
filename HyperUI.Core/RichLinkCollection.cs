using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

namespace HyperUI.Core;

/// <summary>
/// Represents a collection of links.
/// </summary>
public class RichLinkCollection
{
    /// <summary>
    /// Default constructor for JSON serialization.
    /// </summary>
    public RichLinkCollection()
    {

    }

    /// <summary>
    /// Creates a new <see cref="RichLinkCollection"/>.
    /// </summary>
    /// <param name="links">Links.</param>
    /// <param name="renderMode">Render mode.</param>
    public RichLinkCollection(
        IEnumerable<RichLink> links, RichLinkCollectionRenderMode? renderMode = null)
    {
        Links = links;

        if (renderMode != null)
            RenderMode = renderMode;
    }

    /// <summary>
    /// Links.
    /// </summary>
    [JsonPropertyName("links")]
    public IEnumerable<RichLink>? Links { get; set; }

    /// <summary>
    /// Render mode. Defaults to grid.
    /// </summary>
    [JsonConverter(typeof(RichLinkCollectionRenderModeJsonConverter))]
    [JsonPropertyName("renderMode")]
    public RichLinkCollectionRenderMode RenderMode { get; set; } = 
        RichLinkCollectionRenderMode.Default;

    /// <summary>
    /// Gets the <see cref="OpenApiSchema"/> for <see cref="RichLinkCollection"/>.
    /// </summary>
    /// <returns><see cref="OpenApiSchema"/>.</returns>
    public static OpenApiSchema GetOpenApiSchema() => new()
    {
        Title = "Rich link collection",
        Type = "object",
        Properties = new Dictionary<string, OpenApiSchema>()
        {
            ["links"] = new OpenApiSchema()
            {
                ReadOnly = true,
                Type = "array",
                Items = new OpenApiSchema()
                {
                    Reference = new()
                    {
                        Id = "Link",
                        Type = ReferenceType.Schema
                    }
                }
            },
            ["renderMode"] = new OpenApiSchema()
            {
                ReadOnly = true,
                Type = "string",
                Enum = RichLinkCollectionRenderMode
                    .GetAll()
                    .Select(renderMode => new OpenApiString(renderMode.Name))
                    .ToList<IOpenApiAny>()
            }
        }
    };
}