namespace API.Middlewares
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class HasPermissionAttribute : Attribute
    {
        public string? PermissionKey { get; }

        public HasPermissionAttribute(string? permissionKey)
        {
            PermissionKey = permissionKey;
        }
    }

}
