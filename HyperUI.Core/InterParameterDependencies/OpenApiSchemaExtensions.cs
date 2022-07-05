using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace HyperUI.Core;

public static class OpenApiSchemaExtensions
{
    /// <summary>
    /// Gets "requires" dependencies.
    /// </summary>
    /// <param name="schema"><see cref="OpenApiSchema"/>.</param>
    /// <returns>Requires dependencies.</returns>
    public static IEnumerable<RequiresDependency> GetRequiresDependencies(this OpenApiSchema schema)
    {
        // Try to get dependencies
        _ = schema.Extensions.TryGetValue(
                InterParameterDependencies.OpenApiExtensionName,
                out IOpenApiExtension? idlExtension);

        // Get "requires" dependencies
        return ((OpenApiArray?)idlExtension)?
            .OfType<OpenApiString>()
            .Where(specification => specification.Value.StartsWith("IF"))
            .Select(specification => RequiresDependency.TryCreate(specification.Value, schema))
            .Where(requiresDependency => requiresDependency != null)!;
    }

    /// <summary>
    /// Gets groups of properties with inter-dependencies.
    /// </summary>
    /// <param name="schema">Schema.</param>
    /// <param name="interDependencyKeyword">Inter-dependency keyword.</param>
    /// <param name="openApiDataType">OpenAPI data type. See <see cref="OpenApiDataType"/>.</param>
    /// <returns>Property groups.</returns>
    internal static IEnumerable<Dictionary<string, OpenApiSchema>> GetInterDependencyPropertyGroups(
        this OpenApiSchema schema,
        string interDependencyKeyword,
        string openApiDataType)
    {
        // Create a list of property groups
        List<Dictionary<string, OpenApiSchema>> propertyGroups = new();

        // Try to get dependencies
        _ = schema.Extensions.TryGetValue(
                InterParameterDependencies.OpenApiExtensionName,
                out IOpenApiExtension? idlExtension);

        // Get dependencies that match the inter-dependency language (IDL) keyword
        IEnumerable<OpenApiString>? dependencies = ((OpenApiArray?)idlExtension)?
            .OfType<OpenApiString>()
            .Where(dependency => 
                dependency?.Value?.StartsWith(interDependencyKeyword) == true);

        // Return the empty list if there were no matching dependencies
        if (dependencies?.Any() != true)
            return propertyGroups;

        // Get property groups
        foreach (OpenApiString dependency in dependencies)
        {
            string dependencyString = dependency.Value;

            // Get the OpenApi schemas for the properties in the dependency definition
            IEnumerable<KeyValuePair<string, OpenApiSchema>>? properties = dependencyString
                .Remove(0, interDependencyKeyword.Length)
                .TrimEnd(';')
                .Trim('(', ')')?
                .Split(',', StringSplitOptions.TrimEntries)
                .Join(
                    schema.Properties,
                    propertyName => propertyName,
                    propertyItem => propertyItem.Key,
                    (_, propertyItem) => propertyItem);

            // If properties were found and they all match the data type, add them to the list
            if (properties?.All(property => property.Value.Type == openApiDataType) == true)
                propertyGroups.Add(new Dictionary<string, OpenApiSchema>(properties!));
        }

        // Return property groups
        return propertyGroups;
    }
}