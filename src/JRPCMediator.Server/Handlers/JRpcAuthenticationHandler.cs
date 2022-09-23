using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace JRpcMediator.Server.Handlers;

public class JRpcAuthenticationHandler
{
    public async Task<bool> Handle(HttpContext context, Type requestType)
    {
        var attributes = requestType.GetCustomAttributes<JRpcAuthorizeAttribute>();

        // no auth
        if (attributes.Any() is false) return true;

        // require authorized user
        if (context.User.Identity?.IsAuthenticated == true) return true;

        // try authentication for each attribute
        foreach (var attribute in attributes)
        {
            AuthenticateResult result;

            if (attribute.Scheme is null)
                result = await context.AuthenticateAsync();
            else
                result = await context.AuthenticateAsync(attribute.Scheme);

            if (result.Succeeded)
            {
                context.User = result.Principal;
                return true;
            }

            if (attribute.Challange)
            {
                await context.ChallengeAsync();
            }
        }

        return false;
    }
}
