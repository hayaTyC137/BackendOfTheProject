using EgorkaCoins.DataAccess.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EgorkaCoins.Api.Filters
{
    public class RequireAuthAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userId = context.HttpContext.Session.GetInt32("userId");

            if (userId == null)
            {
                context.Result = new JsonResult(new { message = "Не авторизован" })
                {
                    StatusCode = 401
                };
                return;
            }

            // Проверяем что пользователь существует и не забанен
            using var db = new AppDbContext();
            var user = db.Users.FirstOrDefault(u => u.Id == userId.Value);

            if (user == null || user.IsBanned)
            {
                context.HttpContext.Session.Clear();
                context.Result = new JsonResult(new { message = "Доступ запрещён" })
                {
                    StatusCode = 403
                };
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}