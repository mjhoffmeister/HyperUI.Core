using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace HyperUI.Core;

/// <summary>
/// Defines a property group.
/// </summary>
public class PropertyGroup
{
    /// <summary>
    /// Keyword for the related property group extension.
    /// </summary>
    private static readonly string PropertyGroupKeyword = "Group";

    /// <summary>
    /// Creates a new <see cref="PropertyGroup"/> instance.
    /// </summary>
    /// <param name="name">Name.</param>
    /// <param name="propertyKeys">Property keys.</param>
    private PropertyGroup(string name, IEnumerable<string> propertyKeys)
    {
        Name = name;
        PropertyKeys = propertyKeys;
    }

    /// <summary>
    /// Group name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Property keys.
    /// </summary>
    public IEnumerable<string> PropertyKeys { get; }

    /// <summary>
    /// Creates an OpenAPI specification for a related property group.
    /// </summary>
    /// <param name="properties">Related property names.</param>
    /// <returns>The OpenAPI specification.</returns>
    public static OpenApiString CreateOpenApiSpecification(params string[] properties)
    {
        return new OpenApiString(
            $"{PropertyGroupKeyword}({string.Join(", ", properties)});");
    }

    /// <summary>
    /// Creates property groups from create requests.
    /// </summary>
    /// <param name="createRequests">Create requests.</param>
    /// <param name="schema">Object schema.</param>
    /// <param name="createIndependentPropertyGroups">
    /// If <see langword="true"/>, creates groups of one for properties not included in a group
    /// specification.
    /// </param>
    /// <returns>Property groups.</returns>
    public static IEnumerable<PropertyGroup> CreateMany(
        IEnumerable<CreatePropertyGroupRequest>? createRequests,
        OpenApiSchema schema,
        bool createIndependentPropertyGroups)
    {
        List<PropertyGroup> propertyGroups = new();

        // HashSet for tracking which property keys have been evaluated
        HashSet<string> evaluatedPropertyKeys = new();

        // Map property keys to property groups
        Dictionary<string, PropertyGroup>? propertyKeyToPropertyGroupMapping =
            MapPropertyKeysToPropertyGroups(createRequests, schema);

        // Loop through property schemas to preserve ordering
        foreach ((string propertyKey, OpenApiSchema propertySchema) in schema.Properties)
        {
            // Evaluate property key
            if (!evaluatedPropertyKeys.Contains(propertyKey))
            {
                PropertyGroup? propertyGroup = null;

                bool isPropertyInPropertyGroup = propertyKeyToPropertyGroupMapping?
                    .TryGetValue(propertyKey, out propertyGroup) == true;

                // The property is in a property group
                if (isPropertyInPropertyGroup)
                {
                    // Add the property group to the list
                    propertyGroups.Add(propertyGroup!);

                    // Set all property keys in the group to evaluated
                    foreach (string groupPropertyKey in propertyGroup!.PropertyKeys)
                    {
                        evaluatedPropertyKeys.Add(groupPropertyKey);
                    }
                }
                // The property isn't in a property group
                else
                {
                    if (createIndependentPropertyGroups)
                    {
                        string groupName = propertySchema.Title ?? propertyKey;

                        propertyGroups.Add(
                            new PropertyGroup(groupName, new[] { propertyKey }));
                    }

                    evaluatedPropertyKeys.Add(propertyKey);
                }
            }
        }

        return propertyGroups;
    }

    /// <summary>
    /// Maps property keys to property groups.
    /// </summary>
    /// <param name="createRequests">Create requests.</param>
    /// <param name="schema">Object schema.</param>
    /// <returns>Mapping of property keys to property groups.</returns>
    private static Dictionary<string, PropertyGroup>? MapPropertyKeysToPropertyGroups(
        IEnumerable<CreatePropertyGroupRequest>? createRequests, OpenApiSchema schema)
    {
        if (createRequests == null)
            return null;

        // HashSet used for tracking property keys that have been groups
        HashSet<string> groupedPropertyKeys = new();

        // Dictionary used for mappings
        Dictionary<string, PropertyGroup> propertyKeyToPropertyGroupMapping = new();

        foreach (CreatePropertyGroupRequest request in createRequests)
        {
            // Check if the specification is valid
            bool isSpecificationValid = schema
                .IsParameterArraySpecificationValid(
                    PropertyGroupKeyword, request.Specification, out string[]? propertyKeys);

            // Check if the property keys are ungrouped (not a part of any other group)
            bool arePropertyKeysUngrouped = propertyKeys!
                .All(propertyKey => groupedPropertyKeys.Add(propertyKey));

            // If valid, map the property keys to their group
            if (isSpecificationValid && arePropertyKeysUngrouped)
            {
                PropertyGroup propertyGroup = new(request.GroupName, propertyKeys!);

                foreach (string propertyKey in propertyKeys!)
                {
                    propertyKeyToPropertyGroupMapping.Add(propertyKey, propertyGroup);
                }
            }
        }

        return propertyKeyToPropertyGroupMapping;
    }
}

