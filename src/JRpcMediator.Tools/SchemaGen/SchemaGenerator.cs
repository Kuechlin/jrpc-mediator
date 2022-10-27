﻿using JRpcMediator.Tools.SchemaGen.Models;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
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

        if (Types.ContainsKey(name)) return name;

        if (NumberTypes.Contains(type)) return "number";
        else if (StringTypes.Contains(type)) return "string";
        else if (BoolTypes.Contains(type)) return "boolean";
        else if (type.IsEnum)
        {
            var model = new EnumSchema(name);

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
            var model = new TypeSchema(name);

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
        var model = new RequestSchema(name, GetMethod(type), ToType(GetReturnType(type)));

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