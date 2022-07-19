using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace HyperUI.Core;

/// <summary>
/// Creates and manages inter-parameter dependencies defined by IDL4AOS.
/// </summary>
public class InterParameterDependencies
{
    /// <summary>
    /// OpenApi extension name for inter-parameter dependencies.
    /// </summary>
    public static readonly string OpenApiExtensionName = "x-dependencies";

    public InterParameterDependencies(OpenApiSchema schema)
    {
        IEnumerable<string>? interParameterDependencySpecifications =
            TryGetInterParameterDependencySpecifications(schema);

        RequiresDependencies = TryGetRequiresDependencies(
            schema, interParameterDependencySpecifications);
    }


    //public IEnumerable<OnlyOneDependency>? OnlyOneDependencies { get; }

    /// <summary>
    /// "Requires" dependencies.
    /// </summary>
    public IEnumerable<RequiresDependency>? RequiresDependencies { get; }

    /// <summary>
    /// Creates an IDL4AOS specification for an "only one" dependency between properties.
    /// </summary>
    /// <param name="propertyNames">The names of the properties for the specification.</param>
    /// <returns>The IDL4AOS "only one" specification.</returns>
    public static OpenApiString CreateOnlyOneSpecification(params string[] propertyNames)
    {
        return OnlyOneDependency.CreateSpecification(propertyNames);
    }

    /// <summary>
    /// Creates an IDL4AOS specification for a "requires" dependency between properties.
    /// </summary>
    /// <param name="prerequisitePropertyName">Prerequisite property name.</param>
    /// <param name="dependantPropertyName">Dependant property name.</param>
    /// <param name="prerequisitePropertyValue">Prerequisite property value.</param>
    /// <param name="dependantPropertyValue">Dependant property value.</param>
    /// <returns>The IDL4AOS "requires" specification.</returns>
    public static OpenApiString CreateRequiresSpecification(
        string prerequisitePropertyName,
        string dependantPropertyName,
        object? prerequisitePropertyValue = null,
        object? dependantPropertyValue = null)
    {
        return RequiresDependency.CreateOpenApiSpecification(
            prerequisitePropertyName,
            dependantPropertyName,
            prerequisitePropertyValue,
            dependantPropertyValue);
    }

    private static IEnumerable<string>? TryGetInterParameterDependencySpecifications(
        OpenApiSchema schema)
    {
        // Try to get dependency specifications
        _ = schema.Extensions.TryGetValue(
                OpenApiExtensionName,
                out IOpenApiExtension? idlExtension);

        // Try to get string values of the specifications
        return ((OpenApiArray?)idlExtension)?
            .OfType<OpenApiString>()
            .Select(openApiString => openApiString.Value);
    }

    /// <summary>
    /// Tries to get requires dependencies from an OpenAPI schema.
    /// </summary>
    /// <param name="schema">OpenAPI schema.</param>
    /// <param name="interParameterDependencySpecifications">Specifications.</param>
    /// <returns>Requires dependencies if any could be retrieved.</returns>
    private static IEnumerable<RequiresDependency>? TryGetRequiresDependencies(
        OpenApiSchema schema, IEnumerable<string>? interParameterDependencySpecifications)
    {
        return interParameterDependencySpecifications?
            .Where(specification => specification.StartsWith("IF"))
            .Select(specification => RequiresDependency.TryCreate(specification, schema))
            .Where(requiresDependency => requiresDependency != null)!;
    }
}
