using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace HyperUI.Core.UnitTests;

/// <summary>
/// Tests the <see cref="OnlyOneDependency"/> class.
/// </summary>
public static class OnlyOneDependencyTests
{
    /// <summary>
    /// Tests that the expected number of property groups are returned by the GetOnlyOneProperties
    /// method.
    /// </summary>
    /// <param name="dataType">Data type.</param>
    /// <param name="expectedGroupCount">Expected group type.</param>
    [Theory]
    [InlineData("boolean", 2)]
    [InlineData("string", 1)]
    [InlineData("integer", 0)]
    public static void GetOnlyOnePropertyGroups_MultipleDataTypes_ReturnsExpectedGroupCount(
        string dataType, int expectedGroupCount)
    {
        // Arrange

        OpenApiSchema schema = new()
        {
            Extensions = new Dictionary<string, IOpenApiExtension>()
            {
                [InterParameterDependencies.OpenApiExtensionName] = new OpenApiArray()
                {
                    OnlyOneDependency.CreateOpenApiSpecification("yes", "no"),
                    OnlyOneDependency.CreateOpenApiSpecification("approve", "reject"),
                    OnlyOneDependency.CreateOpenApiSpecification("state", "zip", "province")
                }
            },
            Properties = new Dictionary<string, OpenApiSchema>()
            {
                ["yes"] = new OpenApiSchema
                {
                    Type = OpenApiDataType.Boolean,
                },
                ["no"] = new OpenApiSchema
                {
                    Type = OpenApiDataType.Boolean
                },
                ["approve"] = new OpenApiSchema
                {
                    Type = OpenApiDataType.Boolean,
                },
                ["reject"] = new OpenApiSchema
                {
                    Type = OpenApiDataType.Boolean
                },
                ["state"] = new OpenApiSchema
                {
                    Type = OpenApiDataType.String
                },
                ["zip"] = new OpenApiSchema
                {
                    Type = OpenApiDataType.String
                },
                ["province"] = new OpenApiSchema
                {
                    Type = OpenApiDataType.String
                }
            }
        };

        // Act

        IEnumerable<Dictionary<string, OpenApiSchema>> onlyOneProperties = schema
            .GetOnlyOnePropertyGroups(dataType);

        // Assert

        Assert.Equal(expectedGroupCount, onlyOneProperties.Count());
    }

    /// <summary>
    /// Tests that a schema with one "only one" dependency returns a property group with the
    /// expected number of properties.
    /// </summary>
    [Fact]
    public static void GetOnlyOnePropertyGroups_OneDependency_GroupHasExpectedPropertyCount()
    {
        // Arrange

        string dataType = OpenApiDataType.Boolean;

        int expectedPropertyCount = 2;

        OpenApiSchema schema = new()
        {
            Extensions = new Dictionary<string, IOpenApiExtension>()
            {
                [InterParameterDependencies.OpenApiExtensionName] = new OpenApiArray()
                {
                    OnlyOneDependency.CreateOpenApiSpecification("yes", "no"),
                }
            },
            Properties = new Dictionary<string, OpenApiSchema>()
            {
                ["yes"] = new OpenApiSchema
                {
                    Type = OpenApiDataType.Boolean,
                },
                ["no"] = new OpenApiSchema
                {
                    Type = OpenApiDataType.Boolean
                }
            }
        };

        // Act

        IEnumerable<Dictionary<string, OpenApiSchema>> onlyOneProperties = schema
            .GetOnlyOnePropertyGroups(dataType);

        // Assert

        Assert.Equal(expectedPropertyCount, onlyOneProperties.Single().Count);
    }
}
