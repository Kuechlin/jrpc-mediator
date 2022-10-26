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
using static JRpcMediator.Utils.JRpcUtils;
using Microsoft.AspNetCore.Authorization;
using JRpcMediator.Server.Handlers;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace JRpcMediator.Server
{
    public static class JRpcServerExtensions
    {
        public static void AddJRpcMediator(this IServiceCollection services, Assembly[] assemblies, Action<JRpcServerOptions>? setupAction = null)
        {
            // add configurators
            if (setupAction != null) services.Configure(setupAction);

            // add MediatR
            services.AddMediatR(assemblies);

            services.AddTransient<JRpcAuthenticationHandler>();
            services.AddTransient<JRpcAuthorizationHandler>();
            services.AddTransient<JRpcRequestHandler>();
            services.AddTransient<JRpcNotificationHandler>();

            foreach (var type in assemblies.SelectMany(a => a.DefinedTypes).Where(IsRequest))
            {
                JRpcMethods.Instance.TryAdd(GetMethod(type), type);
            }
        }

        public static void AddJRpcMediator(this IServiceCollection services, params Assembly[] assemblies)
            => AddJRpcMediator(services, assemblies);

        public static void AddJRpcMediator(this IServiceCollection services, Assembly assembly, Action<JRpcServerOptions>? setupAction = null)
            => AddJRpcMediator(services, new[] { assembly }, setupAction);

        public static IApplicationBuilder UseJRpcMediator(this IApplicationBuilder app)
        {
            return app.UseMiddleware<JRpcMiddleware>();
        }
    }
}
