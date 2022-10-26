namespace JRpcMediator.Tools.Generate.Models;

public class TypeModel
{
    public string Name { get; set; }

    public Dictionary<string, string> Properties { get; set; } = new();

    public TypeModel(string name)
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
