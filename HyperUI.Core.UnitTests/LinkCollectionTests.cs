using System.Text.Json;

namespace HyperUI.Core.UnitTests;

/// <summary>
/// Tests the <see cref="LinkCollection"/> class.
/// </summary>
public static class LinkCollectionTests
{
    /// <summary>
    /// Tests that a <see cref="LinkCollection"/> created without a 
    /// <see cref="LinkCollectionRenderMode"/> specified has the default render mode set.
    /// </summary>
    [Fact]
    public static void Create_NoRenderMode_CreatesLinkCollectionWithDefaultRenderMode()
    {
        // Arrange

        IEnumerable<Link> links = new[] { new Link("https://api.example.com/items/1") };

        // Act

        LinkCollection linkCollection = new(links);

        // Assert

        Assert.Equal(LinkCollectionRenderMode.Default, linkCollection.RenderMode);
    }

    /// <summary>
    /// Tests that a <see cref="LinkCollection"/> create with a "list"
    /// <see cref="LinkCollectionRenderMode"/> specified has the "list" render mode set.
    /// </summary>
    [Fact]
    public static void Create_WithListRenderMode_CreatesLinkCollectionWithListRenderMode()
    {
        // Arrange

        IEnumerable<Link> links = new[] { new Link("https://api.example.com/items/1") };

        // Act

        LinkCollection linkCollection = new(links, LinkCollectionRenderMode.List);

        // Assert

        Assert.Equal(LinkCollectionRenderMode.List, linkCollection.RenderMode);
    }

    /// <summary>
    /// Tests that a <see cref="LinkCollection"/> created with two links results in a link
    /// collection with two links set.
    /// </summary>
    [Fact]
    public static void Create_WithTwoLink_CreatesLinkCollectionWithTwoLinks()
    {
        // Arrange

        IEnumerable<Link> links = new[]
        {
            new Link("https://api.example.com/items/1"),
            new Link("https://api.example.com/items/2"),
        };

        // Act

        LinkCollection linkCollection = new(links);

        // Assert

        Assert.Equal(2, linkCollection.Links!.Count());
    }

    /// <summary>
    /// Tests that valid JSON for a link collection deserializes into a <see cref="LinkCollection"/>
    /// object with the expected <see cref="LinkCollectionRenderMode"/>.
    /// </summary>
    [Fact]
    public static void Deserialize_ValidJson_SetsExpectedRenderMode()
    {
        // Arrange

        string json =
            "{ \"links\": [ { \"@id\": \"https://api.example.com/1\", " +
            "\"description\": \"This is a link\" } ], \"renderMode\": \"list\" }";

        var expectedRenderMode = LinkCollectionRenderMode.List;

        // Act

        LinkCollection? linkCollection = JsonSerializer.Deserialize<LinkCollection>(json);

        // Assert

        Assert.Equal(expectedRenderMode, linkCollection!.RenderMode);
    }

    /// <summary>
    /// Tests that valid JSON for a link collection deserializes into a not-null 
    /// <see cref="LinkCollection"/> object.
    /// </summary>
    [Fact]
    public static void Deserialize_ValidJson_ReturnsNotNullLinkCollection()
    {
        // Arrange

        string json = 
            "{ \"links\": [ { \"@id\": \"https://api.example.com/1\", " +
            "\"description\": \"This is a link\" } ], \"renderMode\": \"grid\" }";

        // Act

        LinkCollection? linkCollection = JsonSerializer.Deserialize<LinkCollection>(json);

        // Assert

        Assert.NotNull(linkCollection);
    }
}