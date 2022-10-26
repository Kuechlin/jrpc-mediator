using JRpcMediator.Tools.Generate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace JRpcMediator.Server.Dashboard;

public class JRpcDashboardMiddleware
{
    private const string Namespace = "JRpcMediator.Server.Dashboard.wwwroot";

    private readonly RequestDelegate next;
    private readonly JRpcServerOptions options;
    private readonly StaticFileMiddleware staticFiles;

    public JRpcDashboardMiddleware(RequestDelegate next, IOptions<JRpcServerOptions> options, IWebHostEnvironment hostingEnv, ILoggerFactory loggerFactory)
    {
        this.next = next;
        this.options = options.Value;

        var staticFileOptions = new StaticFileOptions
        {
            RequestPath = this.options.Route,
            FileProvider = new EmbeddedFileProvider(typeof(JRpcDashboardMiddleware).Assembly, Namespace)
        };

        staticFiles = new StaticFileMiddleware(next, hostingEnv, Options.Create(staticFileOptions), loggerFactory);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Method != "GET" || context.Request.Path.Value?.StartsWith(options.Route) is not true)
        {
            await next(context);
            return;
        }

        if (context.Request.Path.Value == options.Route)
        {
            await RespondWithIndexHtml(context.Response);
            return;
        }
        else if (context.Request.Path.Value == options.Route + "/types")
        {
            await RespondWithTypes(context.Response);
            return;
        }

        await staticFiles.Invoke(context);
    }

    private async Task RespondWithIndexHtml(HttpResponse response)
    {
        response.StatusCode = 200;
        response.ContentType = "text/html;charset=utf-8";

        using var stream = typeof(JRpcDashboardMiddleware).Assembly.GetManifestResourceStream($"{Namespace}.index.html");
        using var reader = new StreamReader(stream!);

        var html = await reader.ReadToEndAsync();
        
        await response.WriteAsync(html.Replace("/base", options.Route), Encoding.UTF8);
    }

    private async Task RespondWithTypes(HttpResponse response)
    {
        var types = GenerateJRpcTypes.Generate(JRpcMethods.Instance.Values, options.JsonOptions.PropertyNamingPolicy == JsonNamingPolicy.CamelCase);

        await response.WriteAsJsonAsync(types);
    }
}