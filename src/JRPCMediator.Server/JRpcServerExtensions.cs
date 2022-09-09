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
using Microsoft.AspNetCore.Authorization;

namespace JRpcMediator.Server
{
    public static class JRpcServerExtensions
    {
        private static bool IsRequst(Type type) => type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRequest<>));

        public static void AddJRpcServer(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddMediatR(assemblies);

            services.AddTransient<JRpcHandler>();

            foreach (var type in assemblies.SelectMany(a => a.DefinedTypes).Where(IsRequst))
            {
                JRpcHandler.Methods.TryAdd(GetMethod(type), type);
            }
        }

        public static void MapJRpc(this IEndpointRouteBuilder app, string route)
        {
            app.MapPost(route, (ctx) => JRpcHandler.CreateHandler(ctx, app.ServiceProvider.CreateScope().ServiceProvider).InvokeAsync());
        }
    }
}
