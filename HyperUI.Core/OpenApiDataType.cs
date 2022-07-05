namespace HyperUI.Core;

/// <summary>
/// OpenApi data type. See https://swagger.io/docs/specification/data-models/data-types/.
/// </summary>
public static class OpenApiDataType
{
    /// <summary>
    /// Array.
    /// </summary>
    public const string Array = "array";

    /// <summary>
    /// Boolean.
    /// </summary>
    public const string Boolean = "boolean";

    /// <summary>
    /// Integer.
    /// </summary>
    public const string Integer = "integer";

    /// <summary>
    /// Number.
    /// </summary>
    public const string Number = "number";

    /// <summary>
    /// Object.
    /// </summary>
    public const string Object = "object";

    /// <summary>
    /// String. Use format for hints at contents.
    /// </summary>
    public const string String = "string";
}
