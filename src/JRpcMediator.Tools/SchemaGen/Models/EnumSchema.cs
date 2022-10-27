namespace JRpcMediator.Tools.SchemaGen.Models;

public class EnumSchema
{
    public string Name { get; set; }

    public Dictionary<string, int> Values { get; set; } = new();

    public EnumSchema(string name)
    {
        Name = name;
    }

    public override string ToString()
    {
        var model = $"export enum {Name} {{\n";
        foreach (var (name, value) in Values)
        {
            model += $"\t{name} = {value},\n";
        }
        model += "}";
        return model;
    }
}
