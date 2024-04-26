using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

namespace Hair_Salon_Web_ASP.NET.UserFilters
{
    public enum UserRole
    {
        Admin,
        Client,
        Employee
    }

    public class UserRoleAttribute : ActionFilterAttribute
    {
        private UserRole[] _allowedRoles;

        public UserRoleAttribute(params UserRole[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string userRole = context.HttpContext.Session.GetString("role");

            if (!CheckUserRole(userRole))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Error", null);
            }
        }

        private bool CheckUserRole(string userRole)
        {
            foreach (var role in _allowedRoles)
            {
                if (role.ToString() == userRole)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
