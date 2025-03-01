using BusinessLogicLayer.IService;

namespace API.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory;

        public JwtMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _scopeFactory = scopeFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var jwtService = scope.ServiceProvider.GetRequiredService<IJwtService>();
                var token = context.Request.Cookies["AuthToken"]; // 🔥 Lấy token từ cookie
                if (!string.IsNullOrEmpty(token))
                {
                    var userId = jwtService.ValidateToken(token);
                    if (userId != null)
                    {
                        context.Items["Id"] = userId;
                    }
                }
            }
            await _next(context);
        }
    }

}
