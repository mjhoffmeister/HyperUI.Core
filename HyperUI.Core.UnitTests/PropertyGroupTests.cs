using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace HyperUI.Core.UnitTests;

public static class PropertyGroupTests
{
    [Theory]
    [InlineData(false, 2)]
    [InlineData(true, 3)]
    public static void GetPropertyGroups_OneIndependentPropertyAndTwoGroups_ReturnsExpectedGroups(
        bool createIndependentPropertyGroups, int expectedGroupCount)
    {
        // Arrange

        OpenApiSchema schema = new()
        {
            Extensions = new Dictionary<string, IOpenApiExtension>()
            {
                [PropertyGroups.OpenApiExtensionName] = new OpenApiObject()
                {
                    ["Group 1"] = PropertyGroup.CreateOpenApiSpecification("option1", "option2"),
                    ["Group 2"] = PropertyGroup.CreateOpenApiSpecification("optionA", "optionB"),
                }
            },
            Properties = new Dictionary<string, OpenApiSchema>()
            {
                ["independantProperty"] = new OpenApiSchema
                {
                    Title = "Independent",
                    Type = OpenApiDataType.String,
                },
                ["option1"] = new OpenApiSchema
                {
                    Type = OpenApiDataType.Boolean,
                },
                ["option2"] = new OpenApiSchema
                {
                    Type = OpenApiDataType.Boolean
                },
                ["optionA"] = new OpenApiSchema
                {
                    Type = OpenApiDataType.Boolean,
                },
                ["optionB"] = new OpenApiSchema
                {
                    Type = OpenApiDataType.Boolean
                },
            }
        };

        // Act

        IEnumerable<PropertyGroup> propertyGroups = schema.GetPropertyGroups(
            createIndependentPropertyGroups);

        // Assert

        Assert.Equal(expectedGroupCount, propertyGroups.Count());
    }

    [Fact]
    public static void GetPropertyGroups_OneGroupWithTwoProperties_HasTwoPropertyKeys()
    {
        // Arrange

        int expectedPropertyKeyCount = 2;

        OpenApiSchema schema = new()
        {
            Extensions = new Dictionary<string, IOpenApiExtension>()
            {
                [PropertyGroups.OpenApiExtensionName] = new OpenApiObject()
                {
                    ["Group 1"] = PropertyGroup.CreateOpenApiSpecification("option1", "option2"),
                }
            },
            Properties = new Dictionary<string, OpenApiSchema>()
            {
                ["option1"] = new OpenApiSchema
                {
                    Type = OpenApiDataType.Boolean,
                },
                ["option2"] = new OpenApiSchema
                {
                    Type = OpenApiDataType.Boolean
                },
            }
        };

        // Act

        IEnumerable<PropertyGroup> propertyGroups = schema.GetPropertyGroups();

        // Assert

        Assert.Equal(expectedPropertyKeyCount, propertyGroups.Single().PropertyKeys.Count());
    }
}
