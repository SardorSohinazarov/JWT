using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace JWT.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class PermissionFilterAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _permission;

        public PermissionFilterAttribute(string permission)
            => _permission = permission;

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var permission = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "permission" && x.Value == _permission);

            if (permission == null)
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}
