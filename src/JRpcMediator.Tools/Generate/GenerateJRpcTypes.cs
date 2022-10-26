using JRpcMediator.Tools.Generate.Models;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using static JRpcMediator.Utils.JRpcUtils;

namespace JRpcMediator.Tools.Generate;

public class GenerateJRpcTypes
{
    private readonly bool lowerFirst;
    public Dictionary<string, TypeModel> Types { get; } = new();
    public Dictionary<string, EnumModel> Enums { get; } = new();
    public Dictionary<string, RequestModel> Requests { get; } = new();

    private GenerateJRpcTypes(bool lowerFirst = true)
    {
        this.lowerFirst = lowerFirst;
    }

    public static GenerateJRpcTypes Generate(IEnumerable<Type> requests, bool lowerFirst = true)
    {
        var types = new GenerateJRpcTypes(lowerFirst);
        foreach (var request in requests)
        {
            types.ToRequest(request);
        }
        return types;
    }

    private string ToType(Type type)
    {
        var name = GetName(type);

        if (Types.ContainsKey(name)) return name;

        if (NumberTypes.Contains(type)) return "number";
        else if (StringTypes.Contains(type)) return "string";
        else if (BoolTypes.Contains(type)) return "boolean";
        else if (type.IsEnum)
        {
            var model = new EnumModel(name);

            foreach (var item in Enum.GetValues(type))
            {
                model.Values.Add(Enum.GetName(type, item)!, (int)item);
            }

            Enums.Add(name, model);
            return name;
        }
        else if (IsArray(type, out var itemType))
        {
            return $"{ToType(itemType)}[]";
        }
        else
        {
            var model = new TypeModel(name);

            foreach (var property in type.GetProperties().Where(x => x.CanWrite && x.CanRead))
            {
                model.Properties.Add(GetPropertyName(property), ToType(property.PropertyType));
            }

            Types.Add(name, model);
            return name;
        }
    }

    private void ToRequest(Type type)
    {
        var name = GetName(type);
        var model = new RequestModel(name, GetMethod(type), ToType(GetReturnType(type)));

        foreach (var property in type.GetProperties().Where(x => x.CanWrite && x.CanRead))
        {
            model.Properties.Add(GetPropertyName(property), ToType(property.PropertyType));
        }

        Requests.Add(name, model);
    }

    private string GetPropertyName(PropertyInfo property)
        => lowerFirst
        ? property.Name[..1].ToLower() + property.Name[1..]
        : property.Name;

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

    private static string GetName(Type t)
    {
        var index = t.Name.IndexOf('`');
        return index == -1 ? t.Name : t.Name.Substring(0, index);
    }
}
