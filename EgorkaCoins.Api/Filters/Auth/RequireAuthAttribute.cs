using EgorkaCoins.DataAccess.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace EgorkaCoins.Api.Filters.Auth
{
    public class RequireAuthAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userIdClaim = context.HttpContext.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            {
                context.Result = new JsonResult(new { message = "Не авторизован" })
                { StatusCode = 401 };
                return;
            }

            using var db = new AppDbContext();
            var user = db.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null || user.IsBanned)
            {
                context.Result = new JsonResult(new { message = "Доступ запрещён" })
                { StatusCode = 403 };
                return;
            }

            // Передаём id дальше
            context.HttpContext.Items["userId"] = userId;

            base.OnActionExecuting(context);
        }
    }
}
