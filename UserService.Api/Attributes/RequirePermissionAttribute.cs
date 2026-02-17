using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace UserService.Api.Attributes;

public class RequirePermissionAttribute : Attribute, IAuthorizationFilter
{
    private readonly int _requiredPermission;

    public RequirePermissionAttribute(int requiredPermission)
    {
        _requiredPermission = requiredPermission;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (!user.Identity!.IsAuthenticated)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var permissionsClaim = user.FindFirst("permissions");

        if (permissionsClaim == null)
        {
            context.Result = new ForbidResult();
            return;
        }

        var permissions = permissionsClaim.Value.Split(',');

        if (!permissions.Contains(_requiredPermission.ToString()))
        {
            context.Result = new ForbidResult();
        }
    }
}
