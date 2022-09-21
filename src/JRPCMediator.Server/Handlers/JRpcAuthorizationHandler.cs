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
        var authAttribute = requestType.GetCustomAttribute<JRpcAuthorizeAttribute>();

        // no auth
        if (authAttribute is null || authorization is null) return true;

        // require authorized user
        if (context.User.Identity?.IsAuthenticated != true)
            return false;

        // if no roles and no policies defined then user is authorized
        if (authAttribute.Roles.Length == 0 && authAttribute.Policies.Length == 0)
            return true;

        // check roles
        if (authAttribute.Roles != null)
            foreach (var role in authAttribute.Roles)
                if (context.User.IsInRole(role))
                    return true;

        // check policies
        if (authAttribute.Policies != null)
            foreach (var policy in authAttribute.Policies)
                if ((await authorization.AuthorizeAsync(context.User, policy)).Succeeded)
                    return true;

        // default to false
        return false;
    }
}
