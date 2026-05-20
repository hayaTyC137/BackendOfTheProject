using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace EgorkaCoins.Api.Filters
{
    public class AdminModAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = context.HttpContext.User
                .FindFirst(ClaimTypes.Role)?.Value;

            if (role != "admin" && role != "moderator")
            {
                context.Result = new JsonResult(
                    new { message = "Недостаточно прав. Требуется роль admin или moderator" })
                { StatusCode = 403 };
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}