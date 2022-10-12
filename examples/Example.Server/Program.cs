using Example.Contract;
using Example.Server.Handlers;
using JRpcMediator.Server;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
   .AddNegotiate(NegotiateDefaults.AuthenticationScheme, options => { })
   .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
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

builder.Services.AddAuthorization();

builder.Services
    .AddJRpcServer(typeof(DemoRequest).Assembly, typeof(DemoRequestHandler).Assembly)
    .AddJsonOptions(options => options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

builder.Services.AddHttpContextAccessor();

builder.Services.AddSpaStaticFiles(cfg => cfg.RootPath = "wwwroot");

var app = builder.Build();

app.UseDeveloperExceptionPage();
app.UseStaticFiles();
app.UseSpaStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    app.MapJRpc("/execute");
});


app.UseSpa(spa =>
{
    spa.UseProxyToSpaDevelopmentServer("http://localhost:3000/");
});


app.Run();

public partial class Program { }