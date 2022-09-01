using System.Reflection;

namespace JRpcMediator;

public static class JRpcUtils
{
    public static string GetMethod(Type type) => type.GetCustomAttribute<JRpcMethodAttribute>()?.Method ?? type.Name;
}