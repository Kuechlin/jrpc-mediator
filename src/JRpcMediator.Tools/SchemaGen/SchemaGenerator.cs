using JRpcMediator.Tools.SchemaGen.Models;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json.Serialization;
using static JRpcMediator.Utils.JRpcUtils;

namespace JRpcMediator.Tools.SchemaGen;

public class SchemaGenerator
{
    private readonly bool lowerFirst;
    public Dictionary<string, TypeSchema> Types { get; } = new();
    public Dictionary<string, EnumSchema> Enums { get; } = new();
    public Dictionary<string, RequestSchema> Requests { get; } = new();

    private SchemaGenerator(bool lowerFirst = true)
    {
        this.lowerFirst = lowerFirst;
    }

    public static JRpcMediatorSchema Generate(IEnumerable<Type> requests, bool lowerFirst = true)
    {
        var generator = new SchemaGenerator(lowerFirst);
        foreach (var request in requests)
        {
            generator.ToRequest(request);
        }
        return new JRpcMediatorSchema(generator.Requests, generator.Types, generator.Enums);
    }

    private string ToType(Type type)
    {
        var name = GetName(type);

        if (Types.ContainsKey(name) || Enums.ContainsKey(name) || Requests.ContainsKey(name)) return name;
        else if (NumberTypes.Contains(type)) return "number";
        else if (NullabelNumberTypes.Contains(type)) return "number | null";
        else if (StringTypes.Contains(type)) return "string";
        else if (NullabelStringTypes.Contains(type)) return "string | null";
        else if (type == typeof(bool)) return "boolean";
        else if (type == typeof(bool?)) return "boolean | null";
        else if (type.IsEnum)
        {
            var model = new EnumSchema(name);

            if (Enums.TryAdd(name, model))
            {
                foreach (var item in Enum.GetValues(type))
                {
                    model.Values.Add(Enum.GetName(type, item)!, Convert.ToInt32(item));
                }
            }

            return name;
        }
        else if (IsArray(type, out var itemType))
        {
            return $"{ToType(itemType)}[]";
        }
        else
        {
            var model = new TypeSchema(name);
            if (Types.TryAdd(name, model))
            {
                foreach (var property in type.GetProperties().Where(IsProperty))
                {
                    if (new string[] { "id", "Id" }.Contains(GetPropertyName(property)))
                    {
                        model.IdType = ToType(property.PropertyType);
                        continue;
                    }
                    model.Properties.TryAdd(GetPropertyName(property), ToType(property.PropertyType));
                }
            }
            return name;
        }
    }

    private void ToRequest(Type type)
    {
        var name = GetName(type);
        var model = new RequestSchema(name, GetMethod(type), ToType(GetReturnType(type)));

        if (Requests.TryAdd(name, model))
        {
            foreach (var property in type.GetProperties().Where(IsProperty))
            {
                model.Properties.TryAdd(GetPropertyName(property), ToType(property.PropertyType));
            }
        }
    }

    private string GetPropertyName(PropertyInfo property)
        => lowerFirst
        ? property.Name[..1].ToLower() + property.Name[1..]
        : property.Name;

    private static readonly Type[] NumberTypes = new[]
    {
        typeof(short),
        typeof(int),
        typeof(long),
        typeof(float),
        typeof(double),
        typeof(decimal),
    };

    private static readonly Type[] NullabelNumberTypes = new[]
    {
        typeof(short?),
        typeof(int?),
        typeof(long?),
        typeof(float?),
        typeof(double?),
        typeof(decimal?),
    };

    private static readonly Type[] StringTypes = new[]
    {
        typeof(string),
        typeof(Guid),
        typeof(DateTime),
    };
    private static readonly Type[] NullabelStringTypes = new[]
    {
        typeof(Guid?),
        typeof(DateTime?),
    };

    private static bool IsProperty(PropertyInfo property)
    {
        if (!property.CanWrite || !property.CanRead) return false;
        if (property.GetCustomAttribute<JsonIgnoreAttribute>() != null) return false;
        return true;
    }

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

    private static string GetName(Type t)
    {
        // check DisplayName Attribute
        var attr = t.GetCustomAttribute<DisplayNameAttribute>();
        if (attr != null) return attr.DisplayName;
        // get Name from Type Name
        var index = t.Name.IndexOf('`');
        return index == -1 ? t.Name : t.Name.Substring(0, index);
    }
}
