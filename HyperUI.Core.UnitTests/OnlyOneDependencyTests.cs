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
    [Fact]
    public static void GetOnlyOneDependencies_ThreeDependencies_ReturnsThreeOnlyOneDependencies()
    {
        // Arrange

        int expectedOnlyOneDependencyCount = 3;

        OpenApiSchema schema = new()
        {
            Extensions = new Dictionary<string, IOpenApiExtension>()
            {
                [InterParameterDependencies.OpenApiExtensionName] = new OpenApiArray()
                {
                    InterParameterDependencies.CreateOnlyOneSpecification("yes", "no"),
                    InterParameterDependencies.CreateOnlyOneSpecification("approve", "reject"),
                    InterParameterDependencies.CreateOnlyOneSpecification(
                        "state", "zip", "province")
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

        IEnumerable<OnlyOneDependency> onlyOneDependencies = schema.GetOnlyOneDependencies();

        // Assert

        Assert.Equal(expectedOnlyOneDependencyCount, onlyOneDependencies.Count());
    }

    /// <summary>
    /// Tests that a schema with one "only one" dependency returns a property group with the
    /// expected number of properties.
    /// </summary>
    [Fact]
    public static void GetOnlyOneDependencies_OneDependency_HasExpectedPropertyKeyCount()
    {
        // Arrange

        int expectedPropertyCount = 2;

        OpenApiSchema schema = new()
        {
            Extensions = new Dictionary<string, IOpenApiExtension>()
            {
                [InterParameterDependencies.OpenApiExtensionName] = new OpenApiArray()
                {
                    InterParameterDependencies.CreateOnlyOneSpecification("yes", "no"),
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

        IEnumerable<OnlyOneDependency> onlyOneProperties = schema.GetOnlyOneDependencies();

        // Assert

        Assert.Equal(expectedPropertyCount, onlyOneProperties.Single().PropertyKeys.Count());
    }
}
