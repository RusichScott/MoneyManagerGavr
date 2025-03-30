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
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    // Создание таблицы счетов
                    var cmdAccounts = new NpgsqlCommand(@"
                CREATE TABLE IF NOT EXISTS accounts (
                    id SERIAL PRIMARY KEY,
                    username VARCHAR(100) NOT NULL,
                    balance DECIMAL(10, 2) NOT NULL DEFAULT 0.0,
                    created_at TIMESTAMP DEFAULT NOW()
                )", connection);
                    cmdAccounts.ExecuteNonQuery();

                    // Создание таблицы доходов
                    var cmdIncomes = new NpgsqlCommand(@"
                CREATE TABLE IF NOT EXISTS incomes (
                    id SERIAL PRIMARY KEY,
                    username VARCHAR(100) NOT NULL,
                    amount DECIMAL(10, 2) NOT NULL,
                    created_at TIMESTAMP DEFAULT NOW()
                )", connection);
                    cmdIncomes.ExecuteNonQuery();

                    // Создание таблицы расходов
                    var cmdExpenses = new NpgsqlCommand(@"
                CREATE TABLE IF NOT EXISTS expenses (
                    id SERIAL PRIMARY KEY,
                    username VARCHAR(100) NOT NULL,
                    amount DECIMAL(10, 2) NOT NULL,
                    created_at TIMESTAMP DEFAULT NOW()
                )", connection);
                    cmdExpenses.ExecuteNonQuery();
                }
                catch (NpgsqlException ex)
                {
                    Console.WriteLine($"Ошибка при создании таблиц: {ex.Message}");
                    throw; // Перебрасываем исключение для обработки выше
                }
            }
        }

        public decimal GetAccountBalance(string username)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var cmd = new NpgsqlCommand("SELECT balance FROM accounts WHERE username = @username", connection);
                cmd.Parameters.AddWithValue("username", username);
                var result = cmd.ExecuteScalar();
                return result != null ? Convert.ToDecimal(result) : 0.0m;
            }
        }

        public void CreateAccount(string username)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var cmd = new NpgsqlCommand("INSERT INTO accounts (username, balance) VALUES (@username, 0.0)", connection);
                cmd.Parameters.AddWithValue("username", username);
                cmd.ExecuteNonQuery();
            }
        }

        public void AddTransaction(string username, decimal amount, bool isIncome)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                // Обновляем баланс счёта
                var updateCmd = new NpgsqlCommand(
                    "UPDATE accounts SET balance = balance + @amount WHERE username = @username",
                    connection);
                updateCmd.Parameters.AddWithValue("amount", isIncome ? amount : -amount);
                updateCmd.Parameters.AddWithValue("username", username);
                updateCmd.ExecuteNonQuery();

                // Добавляем запись о транзакции
                var tableName = isIncome ? "incomes" : "expenses";
                var insertCmd = new NpgsqlCommand(
                    $"INSERT INTO {tableName} (username, amount) VALUES (@username, @amount)",
                    connection);
                insertCmd.Parameters.AddWithValue("username", username);
                insertCmd.Parameters.AddWithValue("amount", amount);
                insertCmd.ExecuteNonQuery();
            }
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

        public class UserData
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        public UserData GetUserData(string username)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var cmd = new NpgsqlCommand(
                    "SELECT id, first_name, last_name FROM registration WHERE username = @username",
                    connection);
                cmd.Parameters.AddWithValue("username", username);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new UserData
                        {
                            Id = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2)
                        };
                    }
                }
            }
            return null;
        }

        public bool UpdateUserAccount(string username, string newFirstName, string newLastName)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var cmd = new NpgsqlCommand(
                    "UPDATE registration SET first_name = @firstName, last_name = @lastName WHERE username = @username",
                    connection);
                cmd.Parameters.AddWithValue("firstName", newFirstName);
                cmd.Parameters.AddWithValue("lastName", newLastName);
                cmd.Parameters.AddWithValue("username", username);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool DeleteUserAccount(string username)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                // Удаляем все связанные данные пользователя
                var transaction = connection.BeginTransaction();
                try
                {
                    // Удаляем транзакции
                    new NpgsqlCommand($"DELETE FROM incomes WHERE username = '{username}'", connection).ExecuteNonQuery();
                    new NpgsqlCommand($"DELETE FROM expenses WHERE username = '{username}'", connection).ExecuteNonQuery();

                    // Удаляем счета
                    new NpgsqlCommand($"DELETE FROM accounts WHERE username = '{username}'", connection).ExecuteNonQuery();

                    // Удаляем пользователя
                    new NpgsqlCommand($"DELETE FROM login WHERE username = '{username}'", connection).ExecuteNonQuery();

                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

    }
}
