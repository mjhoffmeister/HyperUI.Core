using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

namespace HyperUI.Core;

/// <summary>
/// Represents a collection of links.
/// </summary>
public class LinkCollection
{
    /// <summary>
    /// Default constructor for JSON serialization.
    /// </summary>
    public LinkCollection()
    {

    }

    /// <summary>
    /// Creates a new <see cref="LinkCollection"/>.
    /// </summary>
    /// <param name="links">Links.</param>
    /// <param name="renderMode">Render mode.</param>
    public LinkCollection(IEnumerable<Link> links, LinkCollectionRenderMode? renderMode = null)
    {
        Links = links;

        if (renderMode != null)
            RenderMode = renderMode;
    }

    /// <summary>
    /// Links.
    /// </summary>
    [JsonPropertyName("links")]
    public IEnumerable<Link>? Links { get; set; }

    /// <summary>
    /// Render mode. Defaults to grid.
    /// </summary>
    [JsonConverter(typeof(LinkCollectionRenderModeJsonConverter))]
    [JsonPropertyName("renderMode")]
    public LinkCollectionRenderMode RenderMode { get; set; } = LinkCollectionRenderMode.Default;

    /// <summary>
    /// Gets the <see cref="OpenApiSchema"/> for <see cref="LinkCollection"/>.
    /// </summary>
    /// <returns><see cref="OpenApiSchema"/>.</returns>
    public static OpenApiSchema GetOpenApiSchema() => new()
    {
        Title = "Link collection",
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
                Enum = LinkCollectionRenderMode
                    .GetAll()
                    .Select(renderMode => new OpenApiString(renderMode.Name))
                    .ToList<IOpenApiAny>()
            }
        }
    };
}
