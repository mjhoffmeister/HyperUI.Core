using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace HyperUI.Core;

/// <summary>
/// "Only one" dependency.
/// </summary>
public static class OnlyOneDependency
{
    /// <summary>
    /// Keyword for the "only one" property inter-dependency.
    /// </summary>
    private static readonly string OnlyOneKeyword = "OnlyOne";

    /// <summary>
    /// Creates
    /// </summary>
    /// <param name="properties"></param>
    /// <returns></returns>
    public static OpenApiString CreateOpenApiSpecification(params string[] properties)
    {
        return new OpenApiString($"{OnlyOneKeyword}({string.Join(", ", properties)});");
    }

    public static IEnumerable<Dictionary<string, OpenApiSchema>> GetOnlyOnePropertyGroups(
        this OpenApiSchema schema,
        string openApiDataType)
    {
        return schema.GetInterDependencyPropertyGroups(OnlyOneKeyword, openApiDataType);
    }
}

