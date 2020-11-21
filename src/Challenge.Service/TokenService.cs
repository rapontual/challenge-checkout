using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Challenge.Core.Model;
using Challenge.Core.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Challenge.Service
{
    public class TokenService : ITokenService
    {
        private readonly IOptions<ChallengeSettings> settings;

        public TokenService(IOptions<ChallengeSettings> settings)
        {
            this.settings = settings;
        }

        public string GenerateToken(Merchant merchant)
        {
            var role = merchant.IsAdmin ? "admin" : "user";
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(settings.Value.SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, merchant.Login),
                    new Claim(ClaimTypes.Role, role),
                    new Claim(ClaimTypes.UserData, merchant.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
