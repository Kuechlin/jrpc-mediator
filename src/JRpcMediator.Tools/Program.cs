using JRpcMediator.Tools.Generate;
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

var lowerOption = new Option<bool>(new[] { "-l", "--lowerFirst" }, "Lower First Letter of Propertynames");
outputOption.SetDefaultValue(true);
generateCmd.AddOption(lowerOption);

generateCmd.SetHandler(GenerateCommand.Execute, assemblyOption, outputOption, lowerOption);

rootCmd.Add(generateCmd);

// invoke root
await rootCmd.InvokeAsync(args);