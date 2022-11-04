using Example.Contract;
using Example.Contract.Models;
using JRpcMediator.Client;
using Spectre.Console;
using System.Net.Http.Headers;
using System.Text.Json;
using static Spectre.Console.AnsiConsole;


var handler = new HttpClientHandler();

var client = new JRpcClient(new JRpcClientOptions
{
    Url = "http://localhost:5000/execute",
    JsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    }
});

do
{
    var method = Prompt(
        new SelectionPrompt<string>()
            .Title("Selcet Method?")
            .AddChoices(new[]
            {
                "login",
                "create/todo",
                "update/todo",
                "delete/todo",
                "get/todo",
                "query/todo",
                "error"
            })
        );

    MarkupLine($"Run [yellow underline]{method}[/]");

    try
    {
        int id;
        string name;
        string description;
        TodoState state;
        TodoModel? model;
        switch (method)
        {
            case "login":
                var username = Ask<string>("Username?");
                var password = Prompt(new TextPrompt<string>("Password?").Secret());

                var token = await client.Send(new LoginRequest(username, password));

                client.Configure(http =>
                {
                    http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                });
                break;
            case "create/todo":
                name = Ask<string>("Name?");
                description = Prompt(new TextPrompt<string>("Description?").AllowEmpty());

                model = await client.Send(new CreateTodoRequest(name, description));

                Write(model!.Markup());
                break;

            case "update/todo":
                id = Ask<int>("Id?");

                model = await client.Send(new GetTodoRequest(id));

                name = Prompt(new TextPrompt<string>("Name?")
                    .DefaultValue(model!.Name)
                    .AllowEmpty());
                description = Prompt(new TextPrompt<string>("Description?")
                    .DefaultValue(model!.Description)
                    .AllowEmpty());
                state = Prompt(new SelectionPrompt<TodoState>()
                    .Title("State?")
                    .AddChoices(TodoState.New, TodoState.InProgress, TodoState.Done));

                model = await client.Send(new UpdateTodoRequest(new TodoModel
                {
                    Id = id,
                    Name = name,
                    Description = description,
                    State = state
                }));

                Write(model!.Markup());
                break;
            case "delete/todo":
                id = Ask<int>("Id?");

                await client.Send(new DeleteTodoRequest(id));

                MarkupLine($"[green]Todo {id} deleted[/]");
                break;
            case "get/todo":
                id = Ask<int>("Id?");

                model = await client.Send(new GetTodoRequest(id));

                Write(model!.Markup());
                break;
            case "query/todo":
                var list = await client.Send(new QueryTodosRequest());

                Write(list!.Markup());
                break;
            case "error":
                var shouldThrow = Ask<bool>("Should throw?");

                await client.Send(new ErrorRequest(shouldThrow, "some error"));
                break;
        }
    }
    catch (Exception e)
    {
        WriteException(e, ExceptionFormats.ShortenEverything);
    }
}
while (true);
