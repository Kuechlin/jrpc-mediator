using System.Reflection;
using static JRpcMediator.Utils.JRpcUtils;

namespace JRpcMediator.Tools.Generate;

public static class GenerateCommand
{
    public static void Execute(string assemblyPath, string outpath, bool lowerFirst)
    {
        if (File.Exists(assemblyPath) is false)
            throw new ArgumentException("Assembly not found", nameof(assemblyPath));

        var assembly = Assembly.LoadFrom(assemblyPath);

        var requests = assembly.DefinedTypes.Where(IsRequest).ToArray();

        var types = GenerateJRpcTypes.Generate(requests, lowerFirst);

        var file = "/////////////////////////////////////\n"
                 + "// automatically generated content //\n"
                 + "/////////////////////////////////////\n"
                 + "import { IRequest, JRpcMethod } from '@jrpc-mediator/core';\n\n";

        file += string.Join("\n\n", types.Types.Values);
        file += "\n\n";
        file += string.Join("\n\n", types.Enums.Values);
        file += "\n\n";
        file += string.Join("\n\n", types.Requests.Values);

        File.WriteAllText(outpath, file);
    }
}
