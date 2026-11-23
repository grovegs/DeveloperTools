
namespace GroveGames.DeveloperTools.Tests;

public sealed class DeveloperOptionsRegistryTests
{
    [Fact]
    public void Add_Options_KeepsRegistrationOrderAndIncreasesVersion()
    {
        var registry = new DeveloperOptionsRegistry();
        var first = new TestDeveloperOptions();
        var second = new TestDeveloperOptions();

        registry.Add(first);
        registry.Add(second);

        Assert.Equal(2, registry.Count);
        Assert.Same(first, registry.Get(0));
        Assert.Same(second, registry.Get(1));
        Assert.Equal(2, registry.Version);
    }

    [Fact]
    public void Add_SameOptionsTwice_KeepsOneEntry()
    {
        var registry = new DeveloperOptionsRegistry();
        var options = new TestDeveloperOptions();

        registry.Add(options);
        registry.Add(options);

        Assert.Equal(1, registry.Count);
        Assert.Equal(1, registry.Version);
    }

    [Fact]
    public void Remove_RegisteredOptions_RemovesEntryAndIncreasesVersion()
    {
        var registry = new DeveloperOptionsRegistry();
        var options = new TestDeveloperOptions();
        registry.Add(options);

        registry.Remove(options);

        Assert.Equal(0, registry.Count);
        Assert.Equal(2, registry.Version);
    }

    [Fact]
    public void Remove_UnknownOptions_KeepsVersion()
    {
        var registry = new DeveloperOptionsRegistry();

        registry.Remove(new TestDeveloperOptions());

        Assert.Equal(0, registry.Version);
    }

    private sealed class TestDeveloperOptions : IDeveloperOptions
    {
        public string Category => "Sample";
    }
}
