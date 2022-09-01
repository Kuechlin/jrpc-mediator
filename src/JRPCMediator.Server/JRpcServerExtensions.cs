using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using static JRpcMediator.JRpcUtils;

namespace JRpcMediator.Server
{
    public static class JRpcServerExtensions
    {
        private static bool IsRequst(Type type) => type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRequest<>));

        public static void AddJRpcServer(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddMediatR(assemblies);

            foreach (var type in assemblies.SelectMany(a => a.DefinedTypes).Where(IsRequst))
            {
                JRpcHandler.Methods.TryAdd(GetMethod(type), type);
            }

            var builder = services.AddControllers();

            foreach (var assembly in assemblies)
            {
                builder.AddApplicationPart(assembly);
            }
        }

        public static void MapJRpc(this WebApplication app, string route)
        {
            app.MapPost(route, new JRpcHandler(app.Services.GetRequiredService<IMediator>()).InvokeAsync);
        }
    }
}
