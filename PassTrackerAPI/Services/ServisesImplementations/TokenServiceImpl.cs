
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.IdentityModel.Tokens;
using PassTrackerAPI.Constants;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PassTrackerAPI.Services.ServisesImplementations
{
    public class TokenServiceImpl : ITokenService
    {
        private JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

        public string CreateAccessTokenById(Guid id)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                }),

                Issuer = AuthOptions.ISSUER,
                Audience = AuthOptions.AUDIENCE,
                Expires = DateTime.UtcNow.AddMinutes(AuthOptions.LIFETIME_MINUTES),
                SigningCredentials = new(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
