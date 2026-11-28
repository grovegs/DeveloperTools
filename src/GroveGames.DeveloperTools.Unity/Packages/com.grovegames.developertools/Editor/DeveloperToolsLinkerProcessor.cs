using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.UnityLinker;

namespace GroveGames.DeveloperTools.Unity.Editor
{
    internal sealed class DeveloperToolsLinkerProcessor : IUnityLinkerProcessor
    {
        public const string FileName = "GroveGames.DeveloperTools.link.xml";

        private const BindingFlags MemberFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
        private const string ValidateMethodName = "OnValidate";
        private const string TryParseMethodName = "TryParse";

        public int callbackOrder => 0;

        public string GenerateAdditionalLinkXmlFile(BuildReport report, UnityLinkerBuildPipelineData data)
        {
            var path = Path.Combine(data.inputDirectory, FileName);
            File.WriteAllText(path, BuildLinkXml(GetMembers()));
            return path;
        }

        public static List<MemberInfo> GetMembers()
        {
            var members = new List<MemberInfo>();

            foreach (var field in TypeCache.GetFieldsWithAttribute<DeveloperOptionAttribute>())
            {
                members.Add(field);
                AddTryParse(members, field.FieldType);
            }

            foreach (var method in TypeCache.GetMethodsWithAttribute<DeveloperOptionAttribute>())
            {
                members.Add(method);
            }

            var visited = new HashSet<Type>();

            foreach (var type in TypeCache.GetTypesDerivedFrom<IDeveloperOptions>())
            {
                for (var current = type; current != null && current != typeof(object) && visited.Add(current); current = current.BaseType)
                {
                    var validate = current.GetMethod(ValidateMethodName, MemberFlags, null, Type.EmptyTypes, null);

                    if (validate != null)
                    {
                        members.Add(validate);
                    }
                }
            }

            return members;
        }

        public static string BuildLinkXml(IEnumerable<MemberInfo> members)
        {
            var assemblies = new SortedDictionary<string, SortedDictionary<string, (SortedSet<string> Fields, SortedSet<string> Methods)>>(StringComparer.Ordinal);

            foreach (var member in members)
            {
                var type = member.DeclaringType;

                if (type == null || type.FullName == null)
                {
                    continue;
                }

                var assemblyName = type.Assembly.GetName().Name;

                if (!assemblies.TryGetValue(assemblyName, out var types))
                {
                    types = new SortedDictionary<string, (SortedSet<string> Fields, SortedSet<string> Methods)>(StringComparer.Ordinal);
                    assemblies.Add(assemblyName, types);
                }

                var typeName = type.FullName.Replace('+', '/');

                if (!types.TryGetValue(typeName, out var names))
                {
                    names = (new SortedSet<string>(StringComparer.Ordinal), new SortedSet<string>(StringComparer.Ordinal));
                    types.Add(typeName, names);
                }

                if (member is FieldInfo)
                {
                    names.Fields.Add(member.Name);
                }
                else
                {
                    names.Methods.Add(member.Name);
                }
            }

            var linker = new XElement("linker");

            foreach (var assembly in assemblies)
            {
                var assemblyElement = new XElement("assembly", new XAttribute("fullname", assembly.Key), new XAttribute("ignoreIfMissing", "1"));

                foreach (var type in assembly.Value)
                {
                    var typeElement = new XElement("type", new XAttribute("fullname", type.Key), new XAttribute("preserve", "nothing"));

                    foreach (var fieldName in type.Value.Fields)
                    {
                        typeElement.Add(new XElement("field", new XAttribute("name", fieldName)));
                    }

                    foreach (var methodName in type.Value.Methods)
                    {
                        typeElement.Add(new XElement("method", new XAttribute("name", methodName)));
                    }

                    assemblyElement.Add(typeElement);
                }

                linker.Add(assemblyElement);
            }

            return linker.ToString();
        }

        private static void AddTryParse(List<MemberInfo> members, Type type)
        {
            if (type == typeof(bool) || type == typeof(int) || type == typeof(float) || type == typeof(string) || type.IsEnum)
            {
                return;
            }

            var tryParse = type.GetMethod(TryParseMethodName, BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string), type.MakeByRefType() }, null);

            if (tryParse != null && tryParse.ReturnType == typeof(bool))
            {
                members.Add(tryParse);
            }
        }
    }
}
