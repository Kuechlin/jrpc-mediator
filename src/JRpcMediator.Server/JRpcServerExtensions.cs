﻿using MediatR;
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
using JRpcMediator.Server.Handlers;
using System.Text.Json;

namespace JRpcMediator.Server
{
    public class JRpcServerBuilder
    {
        internal readonly IServiceCollection services;
        public JRpcServerBuilder(IServiceCollection services)
        {
            this.services = services;
        }
    }

    public static class JRpcServerExtensions
    {
        public static JRpcServerBuilder AddJRpcServer(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddMediatR(assemblies);

            services.AddTransient<JRpcHandler>();
            services.AddTransient<JRpcAuthenticationHandler>();
            services.AddTransient<JRpcAuthorizationHandler>();
            services.AddTransient<JRpcRequestHandler>(); 
            services.AddTransient<JRpcNotificationHandler>();
            services.AddTransient<JRpcBatchRequestHandler>();

            foreach (var type in assemblies.SelectMany(a => a.DefinedTypes).Where(IsRequest))
            {
                JRpcHandler.Methods.TryAdd(GetMethod(type), type);
            }

            return new JRpcServerBuilder(services);
        }

        public static JRpcServerBuilder AddJsonOptions(this JRpcServerBuilder builder, Action<JsonSerializerOptions> configure)
        {
            configure(JRpcHandler.JsonOptions);
            return builder;
        }

        public static void MapJRpc(this IEndpointRouteBuilder app, string route)
        {
            app.MapPost(route, (ctx) => app.ServiceProvider.CreateScope().ServiceProvider.GetRequiredService<JRpcHandler>().InvokeAsync(ctx));
        }
    }
}