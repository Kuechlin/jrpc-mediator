using JRpcMediator.Tools.Generate;
using Spectre.Console.Cli;

var app = new CommandApp();

app.Configure(config =>
{
    config.AddCommand<GenerateCommand>("generate")
        .WithAlias("gen")
        .WithDescription("Generate Typescript Types for JRpcMediator")
        .WithExample(new[] { "generate", "./MyJRpcContracts.dll", "./my_contracts.ts" });
});

await app.RunAsync(args);