using EgorkaCoins.DataAccess.Context;
using EgorkaCoins.Domain;
using EgorkaCoins.Helpers.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EgorkaCoins.BusinessLogic.Core
{
    public class PaymentActions
    {
        public List<Payment> GetByUser(int userId)
        {
            using var db = new AppDbContext();
            return db.Payments
                .Include(p => p.Items)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();
        }

        public PaymentCreationResult CreatePaid(int userId, CreatePaymentRequest request)
        {
            using var db = new AppDbContext();

            var method = request.Method.Trim().ToLowerInvariant();
            ValidateRequest(request, method);

            var packageIds = request.Items.Select(i => i.PackageId).Distinct().ToList();
            var packages = db.Packages
                .Include(p => p.Game)
                .Where(p => packageIds.Contains(p.Id))
                .ToDictionary(p => p.Id);

            if (packages.Count != packageIds.Count)
                throw new InvalidOperationException("Один или несколько пакетов не найдены");

            var items = request.Items.Select(item =>
            {
                var package = packages[item.PackageId];
                var game = package.Game;
                var unitPrice = package.Price;
                var totalPrice = unitPrice * item.Quantity;

                return new PaymentItem
                {
                    PackageId = package.Id,
                    GameName = game?.Name ?? package.GameId,
                    GameColor = game?.Color ?? "#B47AFF",
                    Item = package.Label,
                    Amount = $"{package.Amount} {game?.Abbr}".Trim(),
                    Quantity = item.Quantity,
                    UnitPrice = unitPrice,
                    TotalPrice = totalPrice
                };
            }).ToList();

            var now = DateTime.UtcNow;
            var payment = new Payment
            {
                UserId = userId,
                Method = method,
                Status = "paid",
                TotalPrice = items.Sum(i => i.TotalPrice),
                CreatedAt = now,
                PaidAt = now,
                Items = items
            };

            if (method == "card")
            {
                var digits = OnlyDigits(request.Card!.Number);
                payment.CardLast4 = digits[^4..];
            }
            else
            {
                payment.CryptoCurrency = request.Crypto!.Currency.Trim();
                payment.CryptoNetwork = request.Crypto.Network.Trim();
                payment.CryptoTxHash = request.Crypto.TxHash.Trim();
            }

            var orders = items.Select(item => new Order
            {
                UserId = userId,
                GameName = item.GameName,
                GameColor = item.GameColor,
                Item = item.Item,
                Amount = item.Amount,
                Price = item.TotalPrice,
                Status = "completed",
                CreatedAt = now
            }).ToList();

            var user = db.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                throw new InvalidOperationException("Пользователь не найден");

            user.TotalSpent += orders.Sum(o => o.Price);
            user.OrdersCount += orders.Count;
            AddExperience(user, payment.TotalPrice, request.Items.Sum(i => i.Quantity));

            db.Payments.Add(payment);
            db.Orders.AddRange(orders);
            db.SaveChanges();

            payment.Items = payment.Items.OrderBy(i => i.Id).ToList();

            return new PaymentCreationResult
            {
                Payment = payment,
                Orders = orders
            };
        }

        private static void ValidateRequest(CreatePaymentRequest request, string method)
        {
            if (request.Items.Count == 0)
                throw new InvalidOperationException("Корзина пуста");

            if (request.Items.Any(i => string.IsNullOrWhiteSpace(i.PackageId) || i.Quantity <= 0))
                throw new InvalidOperationException("Некорректные товары в корзине");

            if (method != "card" && method != "crypto")
                throw new InvalidOperationException("Неподдерживаемый способ оплаты");

            if (method == "card")
            {
                if (request.Card == null)
                    throw new InvalidOperationException("Не переданы данные карты");

                var digits = OnlyDigits(request.Card.Number);
                if (string.IsNullOrWhiteSpace(request.Card.Cardholder) ||
                    digits.Length < 12 ||
                    string.IsNullOrWhiteSpace(request.Card.Expiry) ||
                    string.IsNullOrWhiteSpace(request.Card.Cvc))
                    throw new InvalidOperationException("Заполните данные карты");
            }

            if (method == "crypto")
            {
                if (request.Crypto == null ||
                    string.IsNullOrWhiteSpace(request.Crypto.Currency) ||
                    string.IsNullOrWhiteSpace(request.Crypto.Network) ||
                    string.IsNullOrWhiteSpace(request.Crypto.TxHash))
                    throw new InvalidOperationException("Заполните данные crypto-платежа");
            }
        }

        private static string OnlyDigits(string value)
            => new(value.Where(char.IsDigit).ToArray());

        private static void AddExperience(User user, decimal totalPrice, int totalQuantity)
        {
            if (user.Level <= 0)
                user.Level = 1;

            if (user.XpToNext <= 0)
                user.XpToNext = 1000;

            var gainedXp = (int)Math.Floor(totalPrice * 10m + totalQuantity * 25m);
            if (gainedXp <= 0)
                return;

            user.Xp += gainedXp;

            while (user.Xp >= user.XpToNext)
            {
                user.Xp -= user.XpToNext;
                user.Level += 1;
                user.XpToNext += 250;
            }
        }
    }

    public class PaymentCreationResult
    {
        public Payment Payment { get; set; } = new();
        public List<Order> Orders { get; set; } = new();
    }
}
