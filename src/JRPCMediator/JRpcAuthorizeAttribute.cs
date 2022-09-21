namespace JRpcMediator;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class JRpcAuthorizeAttribute : Attribute
{
    public string[] Policies { get; set; } = Array.Empty<string>();
    public string[] Roles { get; set; } = Array.Empty<string>();
    public string Policy
    {
        get
        {
            return Policies.FirstOrDefault("");
        }
        set
        {
            Policies = new[] { value };
        }
    }
    public string Role
    {
        get
        {
            return Roles.FirstOrDefault("");
        }
        set
        {
            Roles = new[] { value };
        }
    }
}
