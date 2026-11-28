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

            var names = members.Where(member => member.DeclaringType == typeof(TestOptions) || member.DeclaringType == typeof(TestBaseOptions) || member.DeclaringType == typeof(Version)).Select(member => member.DeclaringType!.Name + "." + member.Name).OrderBy(name => name, StringComparer.Ordinal);

            CollectionAssert.AreEqual(new[] { "TestBaseOptions.OnValidate", "TestOptions.Run", "TestOptions._amount", "TestOptions._version", "Version.TryParse" }, names);
        }

        [Test]
        public void BuildLinkXml_FieldsAndMethods_PreservesThemByDeclaringType()
        {
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var members = new MemberInfo[] { typeof(TestOptions).GetField("_amount", flags)!, typeof(TestOptions).GetMethod("Run", flags)! };

            var linker = XElement.Parse(DeveloperToolsLinkerProcessor.BuildLinkXml(members));

            var assembly = linker.Elements("assembly").Single();
            var type = assembly.Elements("type").Single();
            Assert.AreEqual(typeof(TestOptions).Assembly.GetName().Name, (string)assembly.Attribute("fullname"));
            Assert.AreEqual("1", (string)assembly.Attribute("ignoreIfMissing"));
            Assert.AreEqual(typeof(TestOptions).FullName!.Replace('+', '/'), (string)type.Attribute("fullname"));
            Assert.AreEqual("nothing", (string)type.Attribute("preserve"));
            CollectionAssert.AreEqual(new[] { "_amount" }, type.Elements("field").Select(field => (string)field.Attribute("name")));
            CollectionAssert.AreEqual(new[] { "Run" }, type.Elements("method").Select(method => (string)method.Attribute("name")));
        }

        [Test]
        public void BuildLinkXml_NoMembers_ReturnsEmptyLinker()
        {
            var linker = XElement.Parse(DeveloperToolsLinkerProcessor.BuildLinkXml(Enumerable.Empty<MemberInfo>()));

            Assert.AreEqual("linker", linker.Name.LocalName);
            Assert.IsFalse(linker.HasElements);
        }

#pragma warning disable IDE0044, IDE0051, CS0649
        private class TestBaseOptions : IDeveloperOptions
        {
            public string Category => "Test";

            private void OnValidate()
            {
            }
        }

        private sealed class TestOptions : TestBaseOptions
        {
            [DeveloperOption] private int _amount;
            [DeveloperOption] private Version _version = new(1, 0);

            [DeveloperOption]
            private void Run()
            {
            }
        }
#pragma warning restore IDE0044, IDE0051, CS0649
    }
}
