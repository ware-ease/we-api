using Data.Entity;
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
            // Lấy danh sách nhóm của user
            var userGroups = await _context.AccountGroups
                                           .Where(ag => ag.AccountId == user.Id)
                                           .Include(ag => ag.Group)
                                           .Select(ag => ag.Group.Id)
                                           .ToListAsync();

            foreach (var group in userGroups)
            {
                authClaims.Add(new Claim("Group", group));
            }

            // Lấy danh sách quyền của user
            var userPermissions = await _context.AccountPermissions
                                                .Where(ap => ap.AccountId == user.Id)
                                                .Include(ap => ap.Permission)
                                                .Select(ap => ap.Permission.Url)
                                                .ToListAsync();

            foreach (var permission in userPermissions)
            {
                authClaims.Add(new Claim("Permission", permission));
            }

            // Lấy danh sách Warehouse của user
            var userWarehouses = await _context.AccountWarehouses
                                               .Where(aw => aw.AccountId == user.Id)
                                               .Include(aw => aw.Warehouse)
                                               .Select(aw => aw.Warehouse.Id)
                                               .ToListAsync();

            foreach (var warehouse in userWarehouses)
            {
                authClaims.Add(new Claim("Warehouse", warehouse));
            }



            var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
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
