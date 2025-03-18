using Azure;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;

namespace API.Utils
{
    public static class AuthHelper
    {
        public class AuthUser
        {
            public string? id;
            public string? accessToken;
            public string? refreshToken;
        }

        public static AuthUser? GetCurrentUser(HttpRequest request)
        {
            request.Cookies.TryGetValue("accessToken", out string? accessToken);
            request.Cookies.TryGetValue("refreshToken", out string? refreshToken);

            if (accessToken == null)
            {
                return null;
            }

            var handler = new JwtSecurityTokenHandler();
            var decodedToken = handler.ReadJwtToken(accessToken);

            var id = decodedToken.Claims.First(c => c.Type == "uid").Value;

            return new AuthUser
            {
                id = id,
                accessToken = accessToken,
                refreshToken = refreshToken,
            };
        }

        public static void AppendToCookies(HttpResponse response, string key, string value)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            response.Cookies.Append(key, value, cookieOptions);
        }

        public static void DeleteFromCookies(HttpResponse response, string key)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(-7)
            };

            response.Cookies.Delete(key, cookieOptions);
        }
    }
}
