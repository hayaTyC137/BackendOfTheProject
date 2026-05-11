using EgorkaCoins.DataAccess.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EgorkaCoins.Api.Filters
{
    // AdminMod выполняется ПОСЛЕ RequireAuth — порядок важен!
    public class AdminModAttribute : ActionFilterAttribute
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

            using var db = new AppDbContext();
            var user = db.Users.FirstOrDefault(u => u.Id == userId.Value);

            if (user == null || user.IsBanned)
            {
                context.Result = new JsonResult(new { message = "Доступ запрещён" })
                {
                    StatusCode = 403
                };
                return;
            }

            // Проверяем роль
            if (user.Role != "admin" && user.Role != "moderator")
            {
                context.Result = new JsonResult(new { message = "Недостаточно прав. Требуется роль admin или moderator" })
                {
                    StatusCode = 403
                };
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}