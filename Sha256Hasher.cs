using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManagerGavr
{
    class Sha256Hasher
    {
        public static string HashPassword(string password)
        {
            // Создаем объект SHA-256
            using (SHA256 sha256 = SHA256.Create())
            {
                // Конвертируем пароль в байты
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                // Вычисляем хеш
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);

                // Конвертируем хеш в шестнадцатеричную строку
                StringBuilder builder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    builder.Append(b.ToString("x2")); // "x2" - формат двух hex-символов
                }

                return builder.ToString();
            }
        }
    }
}
