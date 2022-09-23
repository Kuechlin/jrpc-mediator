using Example.Contract;
using JRpcMediator;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Example.Server.Handlers;

public class LoginRequestHandler : IRequestHandler<LoginRequest, string>
{
    public static SymmetricSecurityKey GetTokenSigningKey() => new SymmetricSecurityKey(Encoding.UTF8.GetBytes("super secret sentence"));

    public Task<string> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        if (request.Name != "admin" || request.Pass != "root")
        {
            throw new Exception("unauthorized");
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, request.Name) }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(GetTokenSigningKey(), SecurityAlgorithms.HmacSha256),
            Audience = "me",
            Issuer = "me"
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        // Create Token
        var token = tokenHandler.CreateToken(tokenDescriptor);
        // Convert Token to string
        return Task.FromResult(tokenHandler.WriteToken(token));
    }
}