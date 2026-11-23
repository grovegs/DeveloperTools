
namespace GroveGames.DeveloperTools.Tests;

public sealed class DeveloperToolsPageRegistryTests
{
    [Fact]
    public void Add_Pages_KeepsRegistrationOrderAndIncreasesVersion()
    {
        var registry = new DeveloperToolsPageRegistry();
        var first = new TestPage();
        var second = new TestPage();

        registry.Add(first);
        registry.Add(second);

        Assert.Equal(2, registry.Count);
        Assert.Same(first, registry.Get(0));
        Assert.Same(second, registry.Get(1));
        Assert.Equal(2, registry.Version);
    }

    [Fact]
    public void Add_PagesWithOrder_SortsByOrder()
    {
        var registry = new DeveloperToolsPageRegistry();
        var later = new TestPage();
        var custom = new TestPage();
        var first = new TestPage();

        registry.Add(later, 200);
        registry.Add(custom);
        registry.Add(first, 100);

        Assert.Same(first, registry.Get(0));
        Assert.Same(later, registry.Get(1));
        Assert.Same(custom, registry.Get(2));
    }

    [Fact]
    public void Add_PagesWithSameOrder_KeepsRegistrationOrder()
    {
        var registry = new DeveloperToolsPageRegistry();
        var first = new TestPage();
        var second = new TestPage();

        registry.Add(first, 100);
        registry.Add(second, 100);

        Assert.Same(first, registry.Get(0));
        Assert.Same(second, registry.Get(1));
    }

    [Fact]
    public void Remove_PageWithOrder_KeepsOrderOfOthers()
    {
        var registry = new DeveloperToolsPageRegistry();
        var first = new TestPage();
        var second = new TestPage();
        var third = new TestPage();
        registry.Add(first, 100);
        registry.Add(second, 200);
        registry.Add(third, 300);

        registry.Remove(second);
        registry.Add(second, 250);

        Assert.Same(first, registry.Get(0));
        Assert.Same(second, registry.Get(1));
        Assert.Same(third, registry.Get(2));
    }

    [Fact]
    public void Add_SamePageTwice_KeepsOneEntry()
    {
        var registry = new DeveloperToolsPageRegistry();
        var page = new TestPage();

        registry.Add(page);
        registry.Add(page);

        Assert.Equal(1, registry.Count);
        Assert.Equal(1, registry.Version);
    }

    [Fact]
    public void Remove_RegisteredPage_RemovesEntryAndIncreasesVersion()
    {
        var registry = new DeveloperToolsPageRegistry();
        var page = new TestPage();
        registry.Add(page);

        registry.Remove(page);

        Assert.Equal(0, registry.Count);
        Assert.Equal(2, registry.Version);
    }

    [Fact]
    public void Remove_UnknownPage_KeepsVersion()
    {
        var registry = new DeveloperToolsPageRegistry();

        registry.Remove(new TestPage());

        Assert.Equal(0, registry.Version);
    }

    private sealed class TestPage : IDeveloperToolsPage
    {
        public string Title => "Sample";

        public void Refresh(bool force)
        {
        }
    }
}
