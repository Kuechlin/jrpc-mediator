namespace JRpcMediator;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class JRpcMethodAttribute : Attribute
{
    public string Method { get; set; }

    public JRpcMethodAttribute(string method)
    {
        Method = method;
    }
}
