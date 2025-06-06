using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AlcoHelper.Filters
{
    public class AdminAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = context.HttpContext.Session.GetString("Role");

            if (role != "Admin")
            {
                // Zwróć 403 Forbidden zamiast przekierowania
                context.Result = new ForbidResult();
            }

            base.OnActionExecuting(context);
        }
    }
}
