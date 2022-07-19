using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace HyperUI.Core;

/// <summary>
/// "Only one" dependency.
/// </summary>
public class OnlyOneDependency
{
    /// <summary>
    /// Keyword for the "only one" property inter-dependency.
    /// </summary>
    public static readonly string OnlyOneKeyword = "OnlyOne";

    /// <summary>
    /// Creates a new <see cref="OnlyOneDependency"/>.
    /// </summary>
    /// <param name="propertyKeys">Property keys.</param>
    private OnlyOneDependency(IEnumerable<string> propertyKeys)
    {
        PropertyKeys = propertyKeys;
    }

    /// <summary>
    /// Keys for the properties with an "only one" dependency.
    /// </summary>
    public IEnumerable<string> PropertyKeys { get; }

    /// <summary>
    /// Creates an IDL4AOS specification for an "only one" dependency between properties.
    /// </summary>
    /// <param name="propertyNames">The names of the properties for the specification.</param>
    /// <returns>The IDL4AOS "only one" specification.</returns>
    internal static OpenApiString CreateSpecification(params string[] propertyNames)
    {
        return new OpenApiString($"{OnlyOneKeyword}({string.Join(", ", propertyNames)});");
    }

    /// <summary>
    /// Tries to create an <see cref="OnlyOneDependency"/> from a specification in an
    /// <see cref="OpenApiSchema"/>.
    /// </summary>
    /// <param name="specification">Specification.</param>
    /// <param name="schema"><see cref="OpenApiSchema"/>.</param>
    /// <returns>
    /// An <see cref="OnlyOneDependency"/> if successful; <see cref="null"/>, otherwise.
    /// </returns>
    internal static OnlyOneDependency? TryCreate(string specification, OpenApiSchema schema)
    {
        return schema
            .IsParameterArraySpecificationValid(
                OnlyOneKeyword, specification, out string[]? propertyKeys) ?
            new(propertyKeys) :
            null;
    }
}

