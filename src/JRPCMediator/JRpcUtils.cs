using MediatR;
using System.Reflection;

namespace JRpcMediator;

public static class JRpcUtils
{
    public static string GetMethod(Type type) 
        => type
            .GetCustomAttribute<JRpcMethodAttribute>()
            ?.Method ?? type.Name;

    public static Type GetReturnType(Type type) 
        => type
            .GetInterfaces()
            .First(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRequest<>))
            .GetGenericArguments()[0];
}