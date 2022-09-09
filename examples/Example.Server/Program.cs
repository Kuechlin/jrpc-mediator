using Example.Contract;
using Example.Server.Handlers;
using JRpcMediator.Server;
using Microsoft.AspNetCore.Authentication.Negotiate;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
   .AddNegotiate();

builder.Services.AddAuthorization(cfg =>
{
    cfg.AddPolicy("dev", p => p.RequireAuthenticatedUser());
});

builder.Services.AddJRpcServer(typeof(DemoRequest).Assembly, typeof(DemoRequestHandler).Assembly);

builder.Services.AddHttpContextAccessor();

builder.Services.AddSpaStaticFiles(cfg => cfg.RootPath = "wwwroot");

var app = builder.Build();

app.UseDeveloperExceptionPage();
app.UseStaticFiles();
app.UseSpaStaticFiles();

app.UseRouting();
app.UseAuthentication();

app.UseEndpoints(endpoints =>
{
    endpoints.MapJRpc("/execute");
});


app.UseSpa(spa =>
{
    spa.UseProxyToSpaDevelopmentServer("http://localhost:3000/");
});


app.Run();

public partial class Program { }