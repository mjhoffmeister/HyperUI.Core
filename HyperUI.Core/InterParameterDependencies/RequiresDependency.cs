using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.RegularExpressions;

namespace HyperUI.Core;

public class RequiresDependency
{
    // Regex group name for the dependant property
    private static readonly string _dependantGroupName = "dependant";

    // Regex group name for the dependant property value
    private static readonly string _dependantValueGroupName = "dependantValue";

    // Regex group name for the prerequisite property
    private static readonly string _prerequisiteGroupName = "prerequisite";

    // Regex group name for the prerequisite property value
    private static readonly string _prerequisiteValueGroupName = "prerequisiteValue";

    // Regular expression for the requires dependency
    private static readonly string _requiresExpression = 
        $"IF (?<{_prerequisiteGroupName}>.+)(?:(?:==)(?<{_prerequisiteValueGroupName}>.+))? " +
        $"THEN (?<{_dependantGroupName}>.+)(?:(?:==)(?<{_dependantValueGroupName}>.+))?;";

    /// <summary>
    /// Creates a new <see cref="RequiresDependency"/>.
    /// </summary>
    /// <param name="dependantProperty">Dependant property.</param>
    /// <param name="dependantPropertyValue">Dependant property value.</param>
    /// <param name="prerequisiteProperty">Prerequisite property.</param>
    /// <param name="prerequisitePropertyValue">Prerequisite property value.</param>
    private RequiresDependency(
        KeyValuePair<string, OpenApiSchema> dependantProperty,
        object? dependantPropertyValue,
        KeyValuePair<string, OpenApiSchema> prerequisiteProperty,
        object? prerequisitePropertyValue)
    {
        DependantProperty = dependantProperty;
        DependantPropertyValue = dependantPropertyValue;
        PrerequisiteProperty = prerequisiteProperty;
        PrerequisitePropertyValue = prerequisitePropertyValue;
    }

    /// <summary>
    /// Dependant property. That is, the consequent (after the THEN.)
    /// </summary>
    public KeyValuePair<string, OpenApiSchema> DependantProperty { get; }

    /// <summary>
    /// Dependant property value.
    /// </summary>
    public object? DependantPropertyValue { get; }

    /// <summary>
    /// Prerequisite property. That is, the antecedent (after the IF.)
    /// </summary>
    public KeyValuePair<string, OpenApiSchema> PrerequisiteProperty { get; }

    /// <summary>
    /// Prerequisite property value.
    /// </summary>
    public object? PrerequisitePropertyValue { get; }

    /// <summary>
    /// Creates the Open API specification for a "requires" inter-dependency.
    /// </summary>
    /// <param name="prerequisitePropertyName">Prerequisite property name.</param>
    /// <param name="dependantPropertyName">Dependant property name.</param>
    /// <param name="prerequisitePropertyValue">Prerequisite property value.</param>
    /// <param name="dependantPropertyValue">Dependant property value.</param>
    /// <returns></returns>
    public static OpenApiString CreateOpenApiSpecification(
        string prerequisitePropertyName,
        string dependantPropertyName,
        object? prerequisitePropertyValue = null,
        object? dependantPropertyValue = null)
    {
        StringBuilder openApiSpecificationBuilder = new($"IF {prerequisitePropertyName}");

        TryAddPropertyValueSpecification(prerequisitePropertyValue);

        openApiSpecificationBuilder.Append($" THEN {dependantPropertyName}");

        TryAddPropertyValueSpecification(dependantPropertyValue);

        openApiSpecificationBuilder.Append(';');

        return new OpenApiString(openApiSpecificationBuilder.ToString());

        // Tries to add a property value specification
        void TryAddPropertyValueSpecification(object? propertyValue)
        {
            // If no property value is given, don't add the specification
            if (propertyValue == null)
                return;

            // Add a property value specification with single quotes for strings and no quotes for
            // everything else
            if (propertyValue is string stringPropertyValue)
                openApiSpecificationBuilder.Append($"=='{stringPropertyValue}'");
            else
                openApiSpecificationBuilder.Append($"=={propertyValue}");
        }
    }

    /// <summary>
    /// Tries to create a <see cref="RequiresDependency"/> from a specification and an
    /// <see cref="OpenApiSchema"/>.
    /// </summary>
    /// <param name="specification">Specification.</param>
    /// <param name="schema"><see cref="OpenApiSchema"/>.</param>
    /// <returns>
    /// A <see cref="RequiresDependency"/> if successful; <see cref="null"/>, otherwise.
    /// </returns>
    public static RequiresDependency? TryCreate(string specification, OpenApiSchema schema)
    {
        Match match = Regex.Match(specification, _requiresExpression);

        if (!match.Success)
            return null;

        // Get property names
        string prerequisitePropertyName = match.Groups[_prerequisiteGroupName].Value;
        string dependantPropertyName = match.Groups[_dependantGroupName].Value;

        // Reference properties
        IDictionary<string, OpenApiSchema> properties = schema.Properties;

        // Return null if either the prerequisite or dependant property is missing
        if (!properties.ContainsKey(prerequisitePropertyName) || 
            !properties.ContainsKey(dependantPropertyName))
        {
            return null;
        }

        // Get the dependant property
        KeyValuePair<string, OpenApiSchema> dependantProperty =
            new(dependantPropertyName, properties[dependantPropertyName]);

        // Get the dependant property value object, if present
        object? dependantPropertyValue = TryGetPropertyValueObject(
            match.Groups[_dependantValueGroupName].Value, dependantProperty.Value);

        // Get the prerequisite property
        KeyValuePair<string, OpenApiSchema> prerequisiteProperty =
            new(dependantPropertyName, properties[prerequisitePropertyName]);

        // Get the prerequisite property value object, if present
         object? prerequisitePropertyValue = TryGetPropertyValueObject(
            match.Groups[_prerequisiteValueGroupName].Value, dependantProperty.Value);

        return new RequiresDependency(
            dependantProperty,
            dependantPropertyValue,
            prerequisiteProperty,
            prerequisitePropertyValue);
    }

    /// <summary>
    /// Converts a string value into an object.
    /// </summary>
    /// <param name="propertyValue">Property value.</param>
    /// <param name="propertySchema">Property schema.</param>
    /// <returns><see cref="object"/>.</returns>
    private static object? TryGetPropertyValueObject(
        string? propertyValue, OpenApiSchema propertySchema)
    {
        return propertySchema.Type switch
        {
            OpenApiDataType.Boolean => bool.TryParse(propertyValue, out bool @bool) ? @bool : null,
            OpenApiDataType.String => string.IsNullOrWhiteSpace(propertyValue) ?
                null :
                propertyValue.Trim('\''),
            OpenApiDataType.Integer or OpenApiDataType.Number => TryGetNumericValue(propertyValue),
            _ => null
        };

        // Gets converts a string representation of a numeric value into an object
        static object? TryGetNumericValue(string? propertyValue)
        {
            // Try to convert to int or double rather than micro-optimizing
            if (int.TryParse(propertyValue, out int integerValue))
                return integerValue;

            if (double.TryParse(propertyValue, out double doubleValue))
                return doubleValue;

            return null;
        }
    }
}
