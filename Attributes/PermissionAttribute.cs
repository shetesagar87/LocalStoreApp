using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CleanMvcApp.Attributes;

public class PermissionAttribute : TypeFilterAttribute
{
    public PermissionAttribute(string permission) : base(typeof(PermissionFilter))
    {
        Arguments = new object[] { permission };
    }
}

public class PermissionFilter : IAuthorizationFilter
{
    private readonly string _permission;

    public PermissionFilter(string permission)
    {
        _permission = permission;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (!user.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
            return;
        }

        var hasPermission = user.Claims.Any(c => c.Type == "Permission" && c.Value == _permission);

        if (!hasPermission)
        {
            context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
        }
    }
}
