using EgorkaCoins.DataAccess.Context;
using EgorkaCoins.Domain;
using EgorkaCoins.Helpers.DTOs;

namespace EgorkaCoins.BusinessLogic.Core
{
    public class OrderActions
    {
        // READ ALL orders of a user
        public List<Order> GetByUser(int userId)
        {
            using var db = new AppDbContext();
            return db.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();
        }

        // READ ALL orders (admin)
        public List<Order> GetAll()
        {
            using var db = new AppDbContext();
            return db.Orders
                .OrderByDescending(o => o.CreatedAt)
                .ToList();
        }

        // READ ONE
        public Order? GetById(int id)
        {
            using var db = new AppDbContext();
            return db.Orders.FirstOrDefault(o => o.Id == id);
        }

        // CREATE — принимает список заказов из корзины
        public List<Order> Create(int userId, List<CreateOrderRequest> items)
        {
            using var db = new AppDbContext();

            var orders = items.Select(item => new Order
            {
                UserId = userId,
                GameName = item.GameName,
                GameColor = item.GameColor,
                Item = item.Item,
                Amount = item.Amount,
                Price = item.Price,
                Status = "completed",
                CreatedAt = DateTime.UtcNow
            }).ToList();

            db.Orders.AddRange(orders);

            // Обновляем статистику пользователя
            var user = db.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.TotalSpent += orders.Sum(o => o.Price);
                user.OrdersCount += orders.Count;
            }

            db.SaveChanges();
            return orders;
        }

        // UPDATE status
        public Order? UpdateStatus(int id, string status)
        {
            using var db = new AppDbContext();

            var order = db.Orders.FirstOrDefault(o => o.Id == id);
            if (order == null) return null;

            order.Status = status;
            db.SaveChanges();
            return order;
        }

        // DELETE
        public bool Delete(int id)
        {
            using var db = new AppDbContext();

            var order = db.Orders.FirstOrDefault(o => o.Id == id);
            if (order == null) return false;

            db.Orders.Remove(order);
            db.SaveChanges();
            return true;
        }
    }
}