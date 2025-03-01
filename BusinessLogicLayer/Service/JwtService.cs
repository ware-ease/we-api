using BusinessLogicLayer.IService;
using DotNetEnv;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BusinessLogicLayer.Service
{
    public class JwtService : IJwtService
    {
        private readonly SymmetricSecurityKey _authenKey;

        public JwtService()
        {
            Env.Load(); // Load biến môi trường từ file .env
            string secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? "default-secret-key";
            _authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        }

        public string? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _authenKey,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var userIdClaim = principal.FindFirst("id"); // Lấy userId từ claim "id"
                return userIdClaim?.Value;
            }
            catch
            {
                return null; // Token không hợp lệ
            }
        }
    }
}