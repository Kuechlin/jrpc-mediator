using Example.Contract;
using Example.Server.Handlers;
using JRpcMediator.Server;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddJRpcServer(typeof(DemoRequest).Assembly, typeof(DemoRequestHandler).Assembly);

var app = builder.Build();

app.MapJRpc("/execute");

app.Run();

public partial class Program { }