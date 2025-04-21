using API.Utils;
using BusinessLogicLayer.IServices;

namespace API.Middlewares
{
    public class PermissionMiddleware
    {
        private readonly RequestDelegate _next;

        public PermissionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAuthService permissionService)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint == null)
            {
                await _next(context);
                return;
            }

            // Lấy thông tin permission từ attribute
            var permissionAttribute = endpoint.Metadata.GetMetadata<HasPermissionAttribute>();
            if (permissionAttribute == null)
            {
                await _next(context);
                return;
            }

            var permissionKey = permissionAttribute.PermissionKey;

            // Lấy UserId
            string userId;
            var authUser = AuthHelper.GetCurrentUser(context.Request);
            if (authUser != null)
            {
                userId = authUser.id!;
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized: Missing userId");
                return;
            }

            if (userId == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized: Missing userId");
                return;
            }
            // Check permission
            var hasPermission = await permissionService.HasPermissionAsync(userId, permissionKey);
            if (!hasPermission)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync($"Forbidden: Missing permission '{permissionKey}'");
                return;
            }

            await _next(context);
        }
    }

}
