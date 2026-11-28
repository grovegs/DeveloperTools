using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using NUnit.Framework;

namespace GroveGames.DeveloperTools.Unity.Editor.Tests
{
    public sealed class DeveloperToolsLinkerProcessorTests
    {
        [Test]
        public void GetMembers_OptionsClass_ReturnsOptionsValidateAndTryParse()
        {
            var members = DeveloperToolsLinkerProcessor.GetMembers();

            var names = members.Where(member => member.DeclaringType == typeof(FakeOptions) || member.DeclaringType == typeof(FakeBaseOptions) || member.DeclaringType == typeof(Version)).Select(member => member.DeclaringType!.Name + "." + member.Name).Distinct().OrderBy(name => name, StringComparer.Ordinal);

            CollectionAssert.AreEqual(new[] { "FakeBaseOptions.OnValidate", "FakeOptions.Run", "FakeOptions._amount", "FakeOptions._version", "Version.TryParse" }, names);
        }

        [Test]
        public void BuildLinkXml_FieldsAndMethods_PreservesThemByDeclaringType()
        {
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var members = new MemberInfo[] { typeof(FakeOptions).GetField("_amount", flags)!, typeof(FakeOptions).GetMethod("Run", flags)! };

            var linker = XElement.Parse(DeveloperToolsLinkerProcessor.BuildLinkXml(members));

            var assembly = linker.Elements("assembly").Single();
            var type = assembly.Elements("type").Single();
            Assert.AreEqual(typeof(FakeOptions).Assembly.GetName().Name, (string)assembly.Attribute("fullname"));
            Assert.AreEqual("1", (string)assembly.Attribute("ignoreIfMissing"));
            Assert.AreEqual(typeof(FakeOptions).FullName!.Replace('+', '/'), (string)type.Attribute("fullname"));
            Assert.AreEqual("nothing", (string)type.Attribute("preserve"));
            CollectionAssert.AreEqual(new[] { "_amount" }, type.Elements("field").Select(field => (string)field.Attribute("name")));
            CollectionAssert.AreEqual(new[] { "Run" }, type.Elements("method").Select(method => (string)method.Attribute("name")));
        }

        [Test]
        public void BuildLinkXml_GenericBaseOnValidate_UsesTheGenericDefinition()
        {
            var members = DeveloperToolsLinkerProcessor.GetMembers().Where(member => member.DeclaringType == typeof(FakeGenericBase<int>));

            var linker = XElement.Parse(DeveloperToolsLinkerProcessor.BuildLinkXml(members));

            var type = linker.Elements("assembly").Single().Elements("type").Single();
            Assert.AreEqual(typeof(FakeGenericBase<>).FullName!.Replace('+', '/'), (string)type.Attribute("fullname"));
            CollectionAssert.AreEqual(new[] { "OnValidate" }, type.Elements("method").Select(method => (string)method.Attribute("name")));
        }

        [Test]
        public void BuildLinkXml_NoMembers_ReturnsEmptyLinker()
        {
            var linker = XElement.Parse(DeveloperToolsLinkerProcessor.BuildLinkXml(Enumerable.Empty<MemberInfo>()));

            Assert.AreEqual("linker", linker.Name.LocalName);
            Assert.IsFalse(linker.HasElements);
        }

#pragma warning disable IDE0044, IDE0051, CS0649
        private class FakeBaseOptions : IDeveloperOptions
        {
            public string Category => "Test";

            private void OnValidate()
            {
            }
        }

        private sealed class FakeOptions : FakeBaseOptions
        {
            [DeveloperOption] private int _amount;
            [DeveloperOption] private Version _version = new(1, 0);

            [DeveloperOption]
            private void Run()
            {
            }
        }
        private class FakeGenericBase<T>
        {
            private void OnValidate()
            {
            }
        }

        private sealed class FakeGenericOptions : FakeGenericBase<int>, IDeveloperOptions
        {
            public string Category => "Generic";
        }
#pragma warning restore IDE0044, IDE0051, CS0649
    }
}
