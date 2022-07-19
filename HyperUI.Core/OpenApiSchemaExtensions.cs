using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using System.Diagnostics.CodeAnalysis;

namespace HyperUI.Core;

/// <summary>
/// Extensions methods for the <see cref="OpenApiSchema"/> class.
/// </summary>
public static class OpenApiSchemaExtensions
{
    /// <summary>
    /// Gets property groups.
    /// </summary>
    /// <param name="schema"><see cref="OpenApiSchema"/>.</param>
    /// <param name="createIndependentPropertyGroups">
    /// If <see langword="true"/>, creates groups of one for properties not included in a group
    /// specification.
    /// </param>
    /// <returns>Property groups.</returns>
    public static IEnumerable<PropertyGroup> GetPropertyGroups(
        this OpenApiSchema schema,
        bool createIndependentPropertyGroups = false)
    {
        // Try to get property groups
        _ = schema.Extensions.TryGetValue(
                PropertyGroups.OpenApiExtensionName,
                out IOpenApiExtension? propertyGroupsExtension);

        // Parse requests for creating property groups
        IEnumerable<CreatePropertyGroupRequest>? createRequests =
            ((OpenApiObject?)propertyGroupsExtension)?
                .Select(specification => new CreatePropertyGroupRequest(
                    specification.Key, (specification.Value as OpenApiString)?.Value));

        return PropertyGroup.CreateMany(createRequests, schema, createIndependentPropertyGroups);
    }

    public static IEnumerable<OnlyOneDependency> GetOnlyOneDependencies(this OpenApiSchema schema)
    {
        // Try to get dependencies
        _ = schema.Extensions.TryGetValue(
                InterParameterDependencies.OpenApiExtensionName,
                out IOpenApiExtension? idlExtension);

        // Get "only one" dependencies
        return ((OpenApiArray?)idlExtension)?
            .OfType<OpenApiString>()
            .Where(specification =>
                specification.Value.StartsWith(OnlyOneDependency.OnlyOneKeyword))
            .Select(specification => OnlyOneDependency.TryCreate(specification.Value, schema))
            .Where(onlyOneDependency => onlyOneDependency != null)!;
    }

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
    /// Checks if a parameter array specification with the expected format of
    /// "[<paramref name="specificationKeyword"/>](param1, param2...)" is valid.
    /// </summary>
    /// <param name="schema"><see cref="OpenApiSchema"/>.</param>
    /// <param name="specificationKeyword">Specification keyword.</param>
    /// <param name="specification">Specification.</param>
    /// <param name="parameterKeys">Validated parameter (property) keys.</param>
    /// <returns>True if the specification is valid; false, otherwise.</returns>
    internal static bool IsParameterArraySpecificationValid(
        this OpenApiSchema schema,
        string specificationKeyword,
        string? specification,
        [NotNullWhen(true)] out string[]? parameterKeys)
    {
        parameterKeys = null;

        // Return false if the specification doesn't start with the expected keyword
        if (specification?.StartsWith(specificationKeyword) != true)
            return false;

        // Get the property keys included in the specification
        parameterKeys = specification
            .Remove(0, specificationKeyword.Length)
            .TrimEnd(';')
            .Trim('(', ')')
            .Split(',', StringSplitOptions.TrimEntries);

        // Reference properties for validation
        IDictionary<string, OpenApiSchema> properties = schema.Properties;

        // Return null if any property keys aren't present in the schema
        if (!parameterKeys.All(propertyKey => properties.ContainsKey(propertyKey)))
            return false;

        // Validation passed
        return true;
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