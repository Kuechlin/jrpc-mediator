namespace JRpcMediator;

[AttributeUsage(AttributeTargets.Class)]
public class JRpcMethodAttribute : Attribute
{
    public string Method { get; set; }

    public JRpcMethodAttribute(string method)
    {
        Method = method;
    }
}
