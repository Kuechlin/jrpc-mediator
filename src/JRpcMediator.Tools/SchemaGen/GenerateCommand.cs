using JRpcMediator.Tools.SchemaGen;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using static JRpcMediator.Utils.JRpcUtils;

namespace JRpcMediator.Tools.Generate;

internal sealed class GenerateCommand : Command<GenerateCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<assembly>")]
        [Description("Assembly Path")]
        public string AssemblyPath { get; set; }

        [CommandArgument(1, "<output>")]
        [Description("Output FileName")]
        public string OutputPath { get; set; }

        [CommandOption("-l|--lower")]
        [Description("Lower First Letter of Propertynames")]
        public bool LowerFirstLetter { get; set; }

        public override ValidationResult Validate()
        {
            if (File.Exists(AssemblyPath) is false)
            {
                return ValidationResult.Error("Assembly not found");
            }

            return ValidationResult.Success();
        }
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        try
        {
            AnsiConsole.MarkupLine($"[grey]LOG:[/] Loading Assembly [blue]{Path.GetFileName(settings.AssemblyPath)}[/]");

            var assembly = Assembly.LoadFrom(settings.AssemblyPath);

            var requests = assembly.DefinedTypes.Where(IsRequest).ToArray();

            AnsiConsole.MarkupLine("[grey]LOG:[/] Generation JRpc Schema");
            var types = SchemaGenerator.Generate(requests, settings.LowerFirstLetter);

            var file = "/////////////////////////////////////\n"
                     + "// automatically generated content //\n"
                     + "/////////////////////////////////////\n"
                     + "import { IRequest, JRpcMethod } from '@jrpc-mediator/core';\n\n";

            file += string.Join("\n\n", types.Types.Values);
            file += "\n\n";
            file += string.Join("\n\n", types.Enums.Values);
            file += "\n\n";
            file += string.Join("\n\n", types.Requests.Values);

            AnsiConsole.MarkupLine($"[grey]LOG:[/] Writing Output File [blue]{Path.GetFileName(settings.OutputPath)}[/]");
            File.WriteAllText(settings.OutputPath, file);

            AnsiConsole.MarkupLine("[green]Success[/]");
        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLine($"[bold red]Exception[/]: {e.Message}");
        }
        return 0;
    }
}
