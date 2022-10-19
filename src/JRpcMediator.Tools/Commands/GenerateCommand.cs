using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static JRpcMediator.JRpcUtils;

namespace JRpcMediator.Tools.Commands
{
    public static class GenerateCommand
    {
        public static void Execute(string assemblyPath, string outpath)
        {
            if (File.Exists(assemblyPath) is false)
                throw new ArgumentException("Assembly not found", nameof(assemblyPath));

            var assembly = Assembly.LoadFrom(assemblyPath);

            var models = new Dictionary<string, string>();

            var requests = assembly
                .DefinedTypes
                .Where(IsRequest)
                .Select(ToRequest(models))
                .ToDictionary(x => x.Key, x => x.Value);

            var file = "/////////////////////////////////////\n"
                     + "// automatically generated content //\n"
                     + "/////////////////////////////////////\n";

            file += string.Join("\n\n", models.Values);
            file += "\n\n";
            file += string.Join("\n\n", requests.Values);

            File.WriteAllText(outpath, file);
        }

        private static readonly Type[] NumberTypes = new[]
        {
            typeof(short),      typeof(short?),
            typeof(int),        typeof(int?),
            typeof(long),       typeof(long?),
            typeof(float),      typeof(float?),
            typeof(double),     typeof(double?),
            typeof(decimal),    typeof(decimal?),
        };

        private static readonly Type[] StringTypes = new[]
        {
            typeof(string),
            typeof(Guid),       typeof(Guid?),
            typeof(DateTime),   typeof(DateTime?),
        };

        private static readonly Type[] BoolTypes = new[]
        {
            typeof(bool),       typeof(bool?),
        };

        private static bool IsArray(Type type, [NotNullWhen(true)] out Type? itemType)
        {
            if (type.IsArray)
            {
                itemType = type.GetElementType();
            }
            else
            {
                itemType = type
                    .GetInterfaces()
                    .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    ?.GetGenericArguments()[0];
            }

            return itemType != default;
        }

        private static string LowerFirst(string value) => value[..1].ToLower() + value[1..];

        private static string GetName(this Type t)
        {
            var index = t.Name.IndexOf('`');
            return index == -1 ? t.Name : t.Name.Substring(0, index);
        }

        private static string ToType(Dictionary<string, string> models, Type type)
        {
            var name = type.GetName();

            if (models.ContainsKey(name)) return name;

            if (NumberTypes.Contains(type))
                return "number";
            else if (StringTypes.Contains(type))
                return "string";
            else if (BoolTypes.Contains(type))
                return "boolean";
            else if (type.IsEnum)
            {
                var model = $"export enum {name} {{\n";
                foreach (var item in Enum.GetValues(type))
                {
                    model += $"\t{Enum.GetName(type, item)} = {(int)item},\n";
                }
                model += "}";
                models.Add(name, model);
                return $"{type.Name}";
            }
            else if (IsArray(type, out var itemType))
            {
                return $"{ToType(models, itemType)}[]";
            }
            else
            {
                var model = $"export type {name} = {{\n";
                foreach (var property in type.GetProperties().Where(x => x.CanWrite && x.CanRead))
                {
                    model += $"\t{LowerFirst(property.Name)}: {ToType(models, property.PropertyType)};\n";
                }
                model += "}";
                models.Add(name, model);
                return name;
            }
        }

        private static Func<Type, KeyValuePair<string, string>> ToRequest(Dictionary<string, string> models) => delegate (Type type)
        {
            var name = type.Name;
            var method = GetMethod(type);
            var returnType = ToType(models, GetReturnType(type));

            var model = $"@JRpcMethod('{method}')\n"
                + $"export class {name} implements IRequest<{returnType}> {{\n"
                + $"\tresponse?: {returnType};\n"
                + $"\tconstructor(";
            var properties = type.GetProperties();
            if (properties.Any())
                model += "\n";
            for (int i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                var isLast = i == properties.Length - 1;
                var suffix = isLast ? "" : ",\n";

                model += $"\t\tpublic {LowerFirst(property.Name)}: {ToType(models, property.PropertyType)}{suffix}";
            }
            model += "\n\t) {}\n}";

            return KeyValuePair.Create(name, model);
        };
    }
}
