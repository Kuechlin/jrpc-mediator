using MediatR;
using System;
using System.Linq;
using System.Reflection;

namespace JRpcMediator.Utils
{
    public static class JRpcUtils
    {
        public static bool IsRequest(Type type) => type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRequest<>));
        public static bool IsNotification(Type type) => type.GetInterfaces().Any(x => x == typeof(INotification));

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
}