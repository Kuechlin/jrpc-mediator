namespace JRpcMediator.Tools.SchemaGen.Models;

public class RequestSchema : TypeSchema
{
    public string Method { get; set; }
    public string ReturnType { get; set; }

    public RequestSchema(string name, string method, string returnType) : base(name)
    {
        Method = method;
        ReturnType = returnType;
    }

    public override string ToString()
    {
        var model = $"@JRpcMethod('{Method}')\n"
            + $"export class {Name} implements IRequest<{ReturnType}> {{\n"
            + $"\tresponse?: {ReturnType};\n"
            + $"\tconstructor(";
        if (Properties.Count > 0) model += "\n";
        for (int i = 0; i < Properties.Count; i++)
        {
            var (name, type) = Properties.ElementAt(i);
            var isLast = i == Properties.Count - 1;
            var suffix = isLast ? "\n\t" : ",\n";

            model += $"\t\tpublic {name}: {type}{suffix}";
        }
        model += ") {}\n}";
        return model;
    }
}
