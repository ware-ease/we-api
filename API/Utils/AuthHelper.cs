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

        public static AuthUser GetCurrentUser(HttpRequest request)
        {
            request.Cookies.TryGetValue("accessToken", out var accessToken);
            request.Cookies.TryGetValue("refreshToken", out var refreshToken);

            var handler = new JwtSecurityTokenHandler();
            var decodedToken = handler.ReadJwtToken(accessToken);

            var id = decodedToken.Claims.First(c => c.Type == "Id").Value;

            return new AuthUser
            {
                id = id,
                accessToken = accessToken,
                refreshToken = refreshToken,
            };
        }
    }
}
