using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace JRpcMediator.Server.Handlers;

public class JRpcAuthorizationHandler
{
    private readonly IAuthorizationService? authorization;
    public JRpcAuthorizationHandler(IServiceProvider provider)
    {
        authorization = provider.GetService<IAuthorizationService>();
    }

    public async Task<bool> Handle(HttpContext context, Type requestType)
    {
        var attributes = requestType.GetCustomAttributes<JRpcAuthorizeAttribute>();

        // if no roles and no policies defined then user is authorized
        if (attributes.Any(x => x.Role != null || x.Policy != null) is false || authorization is null) return true;

        // no identity
        if (context.User.Identity?.IsAuthenticated is false) return false;

        foreach (var attribute in attributes)
        {
            // check role
            if (attribute.Role != null)
            {
                if (context.User.IsInRole(attribute.Role))
                    return true;
            }
            // check policy
            if (attribute.Policy != null)
            {
                var result = await authorization.AuthorizeAsync(context.User, attribute.Policy);
                if (result.Succeeded)
                    return true;
            }
        }

        // default to false
        return false;
    }
}
