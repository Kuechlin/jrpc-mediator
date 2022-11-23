using System.Reflection;

namespace JRpcMediator.Tools.SchemaGen.Models;

public class TypeSchema
{
    public string Name { get; set; }

    public Dictionary<string, string> Properties { get; set; } = new();
    public string IdType { get; set; } = string.Empty;

    public TypeSchema(string name)
    {
        Name = name;
    }

    public override string ToString()
    {

        var model = $"export interface {Name}";
        if (IdType.Length > 0) model += " extends IEntity";
        model += " = {\n";
        foreach (var (name, type) in Properties)
        {
            model += $"\t{name}: {type};\n";
        }
        model += "}";
        return model;
    }
}
