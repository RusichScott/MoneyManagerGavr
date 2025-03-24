using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace MoneyManagerGavr
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(string host, string username, string password, string database)
        {
            _connectionString = $"Host={host};Username={username};Password={password};Database={database}";
        }

        public bool ValidateCredentials(string username, string inputPassword)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    // 1. Получаем хеш пароля из БД по логину
                    const string query = @"
                        SELECT password 
                        FROM login
                        WHERE username = @username";

                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("username", username);

                        // 2. Если пользователь не найден - возвращаем false
                        var storedHash = cmd.ExecuteScalar() as string;
                        if (storedHash == null) return false;

                        // 3. Хешируем введенный пароль и сравниваем
                        string inputHash = ComputeSha256Hash(inputPassword);
                        return string.Equals(inputHash, storedHash, StringComparison.OrdinalIgnoreCase);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Ошибка] {ex.Message}");
                    return false;
                }
            }
        }

        // Статический метод для вычисления SHA-256 хеша
        public static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2")); // x2 = hex в нижнем регистре
                }

                return builder.ToString();
            }
        }
    }
}
