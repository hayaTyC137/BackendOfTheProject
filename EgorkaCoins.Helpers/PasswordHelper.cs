using System.Security.Cryptography;
using System.Text;

namespace EgorkaCoins.Helpers
{
    public static class PasswordHelper
    {
        public static string Hash(string password)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            var originalBytes = Encoding.UTF8.GetBytes(password + "egorkaCoinsStore2026");
            var encodedBytes = md5.ComputeHash(originalBytes);
            return BitConverter.ToString(encodedBytes).Replace("-", "").ToLower();
        }

        public static bool Verify(string password, string hash)
        {
            return Hash(password) == hash;
        }
    }
}