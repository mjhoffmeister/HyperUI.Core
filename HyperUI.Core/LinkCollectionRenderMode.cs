namespace HyperUI.Core;

/// <summary>
/// Render mode for a <see cref="LinkCollection"/>.
/// </summary>
public record LinkCollectionRenderMode
{
    /// <summary>
    /// Indicates that the link collection should be rendered as a grid.
    /// </summary>
    public static LinkCollectionRenderMode Grid => new("grid");

    /// <summary>
    /// Indicates that the link collection should be rendered as a list.
    /// </summary>
    public static LinkCollectionRenderMode List => new("list");

    /// <summary>
    /// Default value.
    /// </summary>
    public static LinkCollectionRenderMode Default => Grid;

    /// <summary>
    /// Created a new <see cref="LinkCollectionRenderMode"/>.
    /// </summary>
    /// <param name="name">Name.</param>
    private LinkCollectionRenderMode(string name) => Name = name;

    /// <summary>
    /// Name of the render mode.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets all render modes.
    /// </summary>
    /// <returns>Render modes.</returns>
    public static IEnumerable<LinkCollectionRenderMode> GetAll()
    {
        yield return Grid;
        yield return List;
    }

    /// <summary>
    /// Tries to convert a <see cref="string"/> to its equivalent
    /// <see cref="LinkCollectionRenderMode"/>, returning a default value if the conversion failed.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="default">Default.</param>
    /// <returns><see cref="LinkCollectionRenderMode"/>.</returns>
    public static LinkCollectionRenderMode TryParse(
        string? value, LinkCollectionRenderMode @default)
    {
        return GetAll().SingleOrDefault(renderMode => renderMode.Name == value) ?? @default;
    }
}
