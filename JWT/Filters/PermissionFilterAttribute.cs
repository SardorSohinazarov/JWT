using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace JWT.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class PermissionFilterAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _permission;
        private readonly string _key;

        public PermissionFilterAttribute(string permission, string key = "permission")
        {
            _permission = permission;
            _key = key;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var permission = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == _key && x.Value == _permission);

            if (permission == null)
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}
