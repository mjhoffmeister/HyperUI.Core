using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace HyperUI.Core.UnitTests;

public static class RequiresDependencyTest
{
    /// <summary>
    /// Tests that property values aren't set if they aren't provided.
    /// </summary>
    /// <param name="isTemporary">Temporary indicator.</param>
    /// <param name="expirationDate">Expiration date.</param>
    [Theory]
    [InlineData(null, null)]
    [InlineData(true, null)]
    [InlineData(true, "2025-01-01")]
    [InlineData(null, "2025-01-01")]
    public static void GetRequiresDependencies_WithPropertyValues_SetsPropertyValues(
        bool? isTemporary, string? expirationDate)
    {
        // Arrange

        OpenApiSchema schema = new()
        {
            Extensions = new Dictionary<string, IOpenApiExtension>()
            {
                [InterParameterDependencies.OpenApiExtensionName] = new OpenApiArray()
                {
                    RequiresDependency.CreateOpenApiSpecification(
                        "isTemporary", "expirationDate", isTemporary, expirationDate),
                }
            },
            Properties = new Dictionary<string, OpenApiSchema>()
            {
                ["isTemporary"] = new OpenApiSchema
                {
                    Title = "Temporary access",
                    Type = OpenApiDataType.Boolean
                },
                ["expirationDate"] = new OpenApiSchema
                {
                    Title = "Access expiration date",
                    Type = OpenApiDataType.String,
                    Format = OpenApiStringFormat.Date
                }
            }
        };

        // Act

        IEnumerable<RequiresDependency> requiresDependencies = schema.GetRequiresDependencies();

        // Assert

        Assert.All(requiresDependencies, requiresDependency =>
        {
            Assert.Equal(isTemporary, requiresDependency.PrerequisitePropertyValue);
            Assert.Equal(expirationDate, requiresDependency.DependantPropertyValue);
        });
    }
}
