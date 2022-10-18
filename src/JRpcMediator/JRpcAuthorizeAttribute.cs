namespace JRpcMediator;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class JRpcAuthorizeAttribute : Attribute
{
    public string? Policy { get; init; }
    public string? Role { get; init; }
    public string? Scheme { get; init; }
    public bool Challange { get; init; } = false;
}
