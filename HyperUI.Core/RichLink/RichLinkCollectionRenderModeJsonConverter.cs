using System.Text.Json;
using System.Text.Json.Serialization;

namespace HyperUI.Core;

/// <summary>
/// JSON converter for the <see cref="RichLinkCollectionRenderMode"/> type.
/// </summary>
public class RichLinkCollectionRenderModeJsonConverter : JsonConverter<RichLinkCollectionRenderMode>
{
    /// <inheritdoc/>
    public override RichLinkCollectionRenderMode? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        return RichLinkCollectionRenderMode.TryParse(
            reader.GetString(),
            RichLinkCollectionRenderMode.Default);
    }

    /// <inheritdoc/>
    public override void Write(
        Utf8JsonWriter writer,
        RichLinkCollectionRenderMode value,
        JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Name);
    }
}
