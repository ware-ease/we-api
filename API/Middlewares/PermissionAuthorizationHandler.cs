using BusinessLogicLayer.IService;
using Microsoft.AspNetCore.Authorization;

namespace API.Middlewares
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<IAuthorizationRequirement>
    {
        private readonly IAccountService _userService;

        public PermissionAuthorizationHandler(IAccountService userService)
        {
            _userService = userService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            IAuthorizationRequirement requirement)
        {
            var userId = context.User.FindFirst("sub")?.Value;
            if (userId == null)
            {
                return;
            }

            // 🔥 Lấy quyền từ database thay vì token
            //var userPermissions = await _userService.GetUserPermissionsAsync(userId);
            //var requiredPermission = requirement.ToString().Replace("Permission:", "");

            //if (userPermissions.Contains(requiredPermission))
            //{
            //    context.Succeed(requirement);
            //}
        }
    }

}
