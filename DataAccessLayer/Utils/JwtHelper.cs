using Data.Entity;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DataAccessLayer.Utils
{
    public class JwtHelper
    {

        public static async Task<Token> GenerateAccessTokenAsync(Account user, WaseEaseDbContext _context, IConfiguration _configuration)
        {
            var authClaims = new List<Claim>
            {
                new Claim("Id", user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            Env.Load();
            var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET_KEY")));

            var token = new JwtSecurityToken(
                issuer: Environment.GetEnvironmentVariable("JWT_VALID_ISSUER"),
                audience: Environment.GetEnvironmentVariable("JWT_VALID_AUDIENCE"),
                expires: DateTime.UtcNow.AddMinutes(30),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha512Signature)
            );

            var accesstoken = new JwtSecurityTokenHandler().WriteToken(token);
            //var refreshToken = GenerateRefreshToken();

            //var refreshTokenEntity = new RefreshToken
            //{
            //    RefreshTokenCode = Guid.NewGuid().ToString(),
            //    UserId = user.UserId,
            //    RefreshTokenValue = refreshToken,
            //    JwtId = token.Id,
            //    IsUsed = false,
            //    IsRevoked = false,
            //    CreatedAt = DateTime.UtcNow,
            //    ExpiresAt = DateTime.UtcNow.AddHours(1),
            //};
            //await _context.AddAsync(refreshTokenEntity);
            //await _context.SaveChangesAsync();

            return new Token
            {
                AccessToken = accesstoken,
                //RefreshToken = refreshToken
            };
        }

        //private static string GenerateRefreshToken()
        //{
        //    var random = new byte[32];
        //    using (var rng = RandomNumberGenerator.Create())
        //    {
        //        rng.GetBytes(random);
        //        return Convert.ToBase64String(random);
        //    }
        //}
    }
}
