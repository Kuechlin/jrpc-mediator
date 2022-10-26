using Example.Contract.Models;
using Spectre.Console;
using Spectre.Console.Rendering;

static class Renderer
{
    public static string Markup(this TodoState state) => state switch
    {
        TodoState.New => "[silver]New[/]",
        TodoState.InProgress => "[blue]In Progress[/]",
        TodoState.Done => "[green]Done[/]",
        _ => "[red]Invalid State[/]"
    };

    public static IRenderable Markup(this TodoModel model)
    {
        var table = new Table();

        table.AddColumns("Key", "Value");
        table.AddRow("Id", model!.Id.ToString());
        table.AddRow("State", model!.State.Markup());
        table.AddRow("Name", model!.Name);
        table.AddRow("Description", model!.Description);

        return table;
    }

    public static IRenderable Markup(this TodoModel[] models)
    {
        var table = new Table();
        table.AddColumns("Id", "State", "Name", "Description");

        foreach (var model in models)
        {
            table.AddRow(
                model.Id.ToString(),
                model.State.Markup(),
                model.Name,
                model.Description
            );
        }

        return table;
    }
}