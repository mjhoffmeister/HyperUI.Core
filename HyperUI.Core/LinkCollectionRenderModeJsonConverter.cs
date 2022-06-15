using System.Text.Json;
using System.Text.Json.Serialization;

namespace HyperUI.Core;

/// <summary>
/// JSON converter for the <see cref="LinkCollectionRenderMode"/> type.
/// </summary>
public class LinkCollectionRenderModeJsonConverter : JsonConverter<LinkCollectionRenderMode>
{
    /// <inheritdoc/>
    public override LinkCollectionRenderMode? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        return LinkCollectionRenderMode.TryParse(
            reader.GetString(),
            LinkCollectionRenderMode.Default);
    }

    /// <inheritdoc/>
    public override void Write(
        Utf8JsonWriter writer,
        LinkCollectionRenderMode value,
        JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Name);
    }
}
