namespace JRpcMediator;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class JRpcAuthorizeAttribute : Attribute
{
    public string[]? Policies { get; set; }
    public string[]? Roles { get; set; }
    public string[]? Schemas { get; set; }
}
