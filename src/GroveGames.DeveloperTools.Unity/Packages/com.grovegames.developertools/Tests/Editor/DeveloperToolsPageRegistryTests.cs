using NUnit.Framework;

namespace GroveGames.DeveloperTools.Unity.Editor.Tests
{
    public sealed class DeveloperToolsPageRegistryTests
    {
        [Test]
        public void Add_Pages_KeepsRegistrationOrderAndIncreasesVersion()
        {
            var registry = new DeveloperToolsPageRegistry();
            var first = new SampleDeveloperToolsPage();
            var second = new SampleDeveloperToolsPage();

            registry.Add(first);
            registry.Add(second);

            Assert.That(registry.Count, Is.EqualTo(2));
            Assert.That(registry.Get(0), Is.SameAs(first));
            Assert.That(registry.Get(1), Is.SameAs(second));
            Assert.That(registry.Version, Is.EqualTo(2));
        }

        [Test]
        public void Add_PagesWithOrder_SortsByOrder()
        {
            var registry = new DeveloperToolsPageRegistry();
            var later = new SampleDeveloperToolsPage();
            var custom = new SampleDeveloperToolsPage();
            var first = new SampleDeveloperToolsPage();

            registry.Add(later, 200);
            registry.Add(custom);
            registry.Add(first, 100);

            Assert.That(registry.Get(0), Is.SameAs(first));
            Assert.That(registry.Get(1), Is.SameAs(later));
            Assert.That(registry.Get(2), Is.SameAs(custom));
        }

        [Test]
        public void Add_PagesWithSameOrder_KeepsRegistrationOrder()
        {
            var registry = new DeveloperToolsPageRegistry();
            var first = new SampleDeveloperToolsPage();
            var second = new SampleDeveloperToolsPage();

            registry.Add(first, 100);
            registry.Add(second, 100);

            Assert.That(registry.Get(0), Is.SameAs(first));
            Assert.That(registry.Get(1), Is.SameAs(second));
        }

        [Test]
        public void Remove_PageWithOrder_KeepsOrderOfOthers()
        {
            var registry = new DeveloperToolsPageRegistry();
            var first = new SampleDeveloperToolsPage();
            var second = new SampleDeveloperToolsPage();
            var third = new SampleDeveloperToolsPage();
            registry.Add(first, 100);
            registry.Add(second, 200);
            registry.Add(third, 300);

            registry.Remove(second);
            registry.Add(second, 250);

            Assert.That(registry.Get(0), Is.SameAs(first));
            Assert.That(registry.Get(1), Is.SameAs(second));
            Assert.That(registry.Get(2), Is.SameAs(third));
        }

        [Test]
        public void Add_SamePageTwice_KeepsOneEntry()
        {
            var registry = new DeveloperToolsPageRegistry();
            var page = new SampleDeveloperToolsPage();

            registry.Add(page);
            registry.Add(page);

            Assert.That(registry.Count, Is.EqualTo(1));
            Assert.That(registry.Version, Is.EqualTo(1));
        }

        [Test]
        public void Remove_RegisteredPage_RemovesEntryAndIncreasesVersion()
        {
            var registry = new DeveloperToolsPageRegistry();
            var page = new SampleDeveloperToolsPage();
            registry.Add(page);

            registry.Remove(page);

            Assert.That(registry.Count, Is.EqualTo(0));
            Assert.That(registry.Version, Is.EqualTo(2));
        }

        [Test]
        public void Remove_UnknownPage_KeepsVersion()
        {
            var registry = new DeveloperToolsPageRegistry();

            registry.Remove(new SampleDeveloperToolsPage());

            Assert.That(registry.Version, Is.EqualTo(0));
        }
    }
}
