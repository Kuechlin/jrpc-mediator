using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JRpcMediator.Server;

public class JRpcServerOptions
{
    public string Route { get; set; } = "/jrpc";

    public JsonSerializerOptions JsonOptions { get; set; } = new();
}