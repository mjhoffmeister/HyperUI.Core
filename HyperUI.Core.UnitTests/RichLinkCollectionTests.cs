using System.Text.Json;

namespace HyperUI.Core.UnitTests;

/// <summary>
/// Tests the <see cref="RichLinkCollection"/> class.
/// </summary>
public static class RichLinkCollectionTests
{
    /// <summary>
    /// Tests that a <see cref="RichLinkCollection"/> created without a 
    /// <see cref="RichLinkCollectionRenderMode"/> specified has the default render mode set.
    /// </summary>
    [Fact]
    public static void Create_NoRenderMode_CreatesLinkCollectionWithDefaultRenderMode()
    {
        // Arrange

        IEnumerable<RichLink> links = new[] { new RichLink("https://api.example.com/items/1") };

        // Act

        RichLinkCollection linkCollection = new(links);

        // Assert

        Assert.Equal(RichLinkCollectionRenderMode.Default, linkCollection.RenderMode);
    }

    /// <summary>
    /// Tests that a <see cref="RichLinkCollection"/> create with a "list"
    /// <see cref="RichLinkCollectionRenderMode"/> specified has the "list" render mode set.
    /// </summary>
    [Fact]
    public static void Create_WithListRenderMode_CreatesLinkCollectionWithListRenderMode()
    {
        // Arrange

        IEnumerable<RichLink> links = new[] { new RichLink("https://api.example.com/items/1") };

        // Act

        RichLinkCollection linkCollection = new(links, RichLinkCollectionRenderMode.List);

        // Assert

        Assert.Equal(RichLinkCollectionRenderMode.List, linkCollection.RenderMode);
    }

    /// <summary>
    /// Tests that a <see cref="RichLinkCollection"/> created with two links results in a link
    /// collection with two links set.
    /// </summary>
    [Fact]
    public static void Create_WithTwoLink_CreatesLinkCollectionWithTwoLinks()
    {
        // Arrange

        IEnumerable<RichLink> links = new[]
        {
            new RichLink("https://api.example.com/items/1"),
            new RichLink("https://api.example.com/items/2"),
        };

        // Act

        RichLinkCollection linkCollection = new(links);

        // Assert

        Assert.Equal(2, linkCollection.Links!.Count());
    }

    /// <summary>
    /// Tests that valid JSON for a link collection deserializes into a 
    /// <see cref="RichLinkCollection"/> object with the expected
    /// <see cref="RichLinkCollectionRenderMode"/>.
    /// </summary>
    [Fact]
    public static void Deserialize_ValidJson_SetsExpectedRenderMode()
    {
        // Arrange

        string json =
            "{ \"links\": [ { \"@id\": \"https://api.example.com/1\", " +
            "\"description\": \"This is a link\" } ], \"renderMode\": \"list\" }";

        var expectedRenderMode = RichLinkCollectionRenderMode.List;

        // Act

        RichLinkCollection? linkCollection = JsonSerializer.Deserialize<RichLinkCollection>(json);

        // Assert

        Assert.Equal(expectedRenderMode, linkCollection!.RenderMode);
    }

    /// <summary>
    /// Tests that valid JSON for a link collection deserializes into a not-null 
    /// <see cref="RichLinkCollection"/> object.
    /// </summary>
    [Fact]
    public static void Deserialize_ValidJson_ReturnsNotNullLinkCollection()
    {
        // Arrange

        string json = 
            "{ \"links\": [ { \"@id\": \"https://api.example.com/1\", " +
            "\"description\": \"This is a link\" } ], \"renderMode\": \"grid\" }";

        // Act

        RichLinkCollection? linkCollection = JsonSerializer.Deserialize<RichLinkCollection>(json);

        // Assert

        Assert.NotNull(linkCollection);
    }
}