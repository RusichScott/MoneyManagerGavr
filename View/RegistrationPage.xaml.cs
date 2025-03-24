using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Npgsql;
using BCrypt.Net;

namespace MoneyManagerGavr.View
{
    /// <summary>
    /// Логика взаимодействия для RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : Page
    {
        public RegistrationPage()
        {
            InitializeComponent();
        }

        private void SaveUserToDatabase(string firstName, string lastName, string username, string hashedPassword)
        {
            // Строка подключения к PostgreSQL (замените параметры на свои)
            string connectionString = "Host=localhost;Username=postgres;Password=WraiDexYT1;Database=MoneyManager";

            // SQL-запрос для вставки данных
            string query = @"
        INSERT INTO registration (first_name, last_name, username, password) 
        VALUES (@firstName, @lastName, @username, @password)";

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        // Добавляем параметры для защиты от SQL-инъекций
                        cmd.Parameters.AddWithValue("firstName", firstName);
                        cmd.Parameters.AddWithValue("lastName", lastName);
                        cmd.Parameters.AddWithValue("username", username);
                        cmd.Parameters.AddWithValue("password", hashedPassword);

                        // Выполняем запрос
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Регистрация успешна!", "Успех",
                                         MessageBoxButton.OK, MessageBoxImage.Information);

                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show($"Ошибка базы данных: {ex.Message}", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Неожиданная ошибка: {ex.Message}", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Registration_Button_Click(object sender, RoutedEventArgs e)
        {
            string firstName = FirstNameTextBox.Text;
            string lastName = LastNameTextBox.Text;
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            // Хешируем пароль
            string hashedPassword = Sha256Hasher.HashPassword(password);
            Console.WriteLine($"Длина хеша: {hashedPassword.Length}"); // Всегда 64 символа

            // Сохраняем в базу данных
            SaveUserToDatabase(firstName, lastName, username, hashedPassword);
        }

        private void Go_Back_Label_Click(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}

