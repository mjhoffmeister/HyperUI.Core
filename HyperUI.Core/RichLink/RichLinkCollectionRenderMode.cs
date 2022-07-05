namespace HyperUI.Core;

/// <summary>
/// Render mode for a <see cref="RichLinkCollection"/>.
/// </summary>
public record RichLinkCollectionRenderMode
{
    /// <summary>
    /// Indicates that the link collection should be rendered as a grid.
    /// </summary>
    public static RichLinkCollectionRenderMode Grid => new("grid");

    /// <summary>
    /// Indicates that the link collection should be rendered as a list.
    /// </summary>
    public static RichLinkCollectionRenderMode List => new("list");

    /// <summary>
    /// Default value.
    /// </summary>
    public static RichLinkCollectionRenderMode Default => Grid;

    /// <summary>
    /// Created a new <see cref="RichLinkCollectionRenderMode"/>.
    /// </summary>
    /// <param name="name">Name.</param>
    private RichLinkCollectionRenderMode(string name) => Name = name;

    /// <summary>
    /// Name of the render mode.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets all render modes.
    /// </summary>
    /// <returns>Render modes.</returns>
    public static IEnumerable<RichLinkCollectionRenderMode> GetAll()
    {
        yield return Grid;
        yield return List;
    }

    /// <summary>
    /// Tries to convert a <see cref="string"/> to its equivalent
    /// <see cref="RichLinkCollectionRenderMode"/>, returning a default value if the conversion
    /// failed.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="default">Default.</param>
    /// <returns><see cref="RichLinkCollectionRenderMode"/>.</returns>
    public static RichLinkCollectionRenderMode TryParse(
        string? value, RichLinkCollectionRenderMode @default)
    {
        return GetAll().SingleOrDefault(renderMode => renderMode.Name == value) ?? @default;
    }
}
