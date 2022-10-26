using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;

namespace JRpcMediator.Server.Dashboard
{
    public static class JRpcDashboardExtensions
    {
        public static void UseJRpcDashboard(this IApplicationBuilder app)
        {
            app.UseMiddleware<JRpcDashboardMiddleware>();
        }
    }
}