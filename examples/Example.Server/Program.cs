using Example.Contract;
using Example.Server.Data;
using Example.Server.Handlers;
using JRpcMediator.Server;
using JRpcMediator.Server.Dashboard;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// add authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
   .AddJwtBearer(options =>
   {
       options.TokenValidationParameters = new TokenValidationParameters
       {
           ValidateIssuer = true,
           ValidateAudience = true,
           ValidateLifetime = true,
           ValidateIssuerSigningKey = true,

           ValidIssuer = "me",
           ValidAudience = "me",
           IssuerSigningKey = LoginRequestHandler.GetTokenSigningKey(),
       };
   });

// add authorization
builder.Services.AddAuthorization();

// add db context
builder.Services.AddDbContext<TodoContext>(options => options.UseInMemoryDatabase("local"));

// add jrpc mediator
builder.Services
    .AddJRpcMediator(new[] { typeof(CreateTodoRequest).Assembly, typeof(CreateTodoRequestHandler).Assembly }, options =>
    {
        options.Route = "/execute";
        options.JsonOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.AddHttpContextAccessor();

builder.Services.AddSpaStaticFiles(cfg => cfg.RootPath = "wwwroot");

var app = builder.Build();

app.UseDeveloperExceptionPage();
app.UseStaticFiles();
app.UseSpaStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseJRpcMediator();
app.UseJRpcDashboard();

app.UseSpa(spa =>
{
    spa.UseProxyToSpaDevelopmentServer("http://localhost:3000/");
});


app.Run();

public partial class Program { }