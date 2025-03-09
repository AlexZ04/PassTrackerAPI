
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PassTrackerAPI.Constants;
using PassTrackerAPI.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace PassTrackerAPI.Services.ServisesImplementations
{
    public class TokenServiceImpl : ITokenService
    {
        private JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        private readonly DataContext _context;

        public TokenServiceImpl(DataContext context)
        {
            _context = context;
        }

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

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }

        public async Task HandleTokens(Guid userId, Guid tokenId)
        {
            await _context.RefreshTokens
                .Include(r => r.User)
                .Where(r => r.User.Id == userId && r.Id != tokenId)
                .ExecuteDeleteAsync();
        }
    }
}
