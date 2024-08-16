using EventManagementSystem.Api.Common.AppSettings;
using EventManagementSystem.Api.Data.Models;
using EventManagementSystem.Api.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime;
using System.Security.Claims;
using System.Text;

namespace EventManagementSystem.Api.Services.Implementations
{
    public class TokenService: ITokenService
    {
        private readonly JwtDetails _jwtDetails;

        public TokenService(IOptions<JwtDetails> jwtDetails)
        {
            _jwtDetails = jwtDetails.Value;
        }

        public string GenerateAccessToken(User user, List<string> roles)
        {
            var signInCredentials = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtDetails.SecretKey)),
                SecurityAlgorithms.HmacSha256
            );

            var claims = new List<Claim>()
            {
                new Claim("Email", user.Email),
                new Claim("UserId", user.Id.ToString()),               
            };
            foreach (var role in roles)
            {
                var claim = new Claim("Role", role);
                claims.Add(claim);
            }

            var securityToken = new JwtSecurityToken(
            issuer: _jwtDetails.Issuer,
            audience: _jwtDetails.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_jwtDetails.ExpiryMinutes),
            signingCredentials: signInCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }
    }
}
