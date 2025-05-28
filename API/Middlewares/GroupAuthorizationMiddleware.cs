using API.Utils;
using BusinessLogicLayer.IServices;

namespace API.Middlewares
{
    public class GroupAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public GroupAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAuthService authService)
        {
            var endpoint = context.GetEndpoint();
            var groupAttribute = endpoint?.Metadata.GetMetadata<AuthorizeGroupAttribute>();

            if (groupAttribute == null)
            {
                // Không có phân quyền, cho qua
                await _next(context);
                return;
            }

            var token = context.Request.Cookies["accessToken"];
            if (string.IsNullOrEmpty(token))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Không tìm thấy accesstoken");
                return;
            }

            var authUser = AuthHelper.GetCurrentUser(context.Request);

            var userId = authUser?.id;
            if (userId == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Token sai, không thể tìm thấy uid trong token");
                return;
            }

            // Lấy group từ user
            var userGroups = await authService.GetUserGroups(userId);

            // Kiểm tra nếu có ít nhất một group khớp
            bool isAuthorized = groupAttribute.Groups.Any(required => userGroups.Contains(required, StringComparer.OrdinalIgnoreCase));

            if (!isAuthorized)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Forbidden: insufficient group access");
                return;
            }

            // Cho qua nếu hợp lệ
            await _next(context);
        }
    }

}
