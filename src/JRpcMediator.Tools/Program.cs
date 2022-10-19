using JRpcMediator.Tools.Commands;
using System.CommandLine;

var rootCmd = new RootCommand("JRpc.Mediator Tools");

// generate typescript
var generateCmd = new Command("generate", "Generate Typescript Types");

var assemblyOption = new Option<string>(new[] { "-a", "--assembly" }, "Assembly Path");
assemblyOption.IsRequired = true;
generateCmd.AddOption(assemblyOption);

var outputOption = new Option<string>(new[] { "-o", "--output" }, "Output Path");
outputOption.IsRequired = true;
generateCmd.AddOption(outputOption);

generateCmd.SetHandler(GenerateCommand.Execute, assemblyOption, outputOption);

rootCmd.Add(generateCmd);

// invoke root
await rootCmd.InvokeAsync(args);