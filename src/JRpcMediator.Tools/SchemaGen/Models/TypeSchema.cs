namespace JRpcMediator.Tools.SchemaGen.Models;

public class TypeSchema
{
    public string Name { get; set; }

    public Dictionary<string, string> Properties { get; set; } = new();

    public TypeSchema(string name)
    {
        Name = name;
    }

    public override string ToString()
    {
        var model = $"export type {Name} = {{\n";
        foreach (var (name, type) in Properties)
        {
            model += $"\t{name}: {type};\n";
        }
        model += "}";
        return model;
    }
}
