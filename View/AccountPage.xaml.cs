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

namespace MoneyManagerGavr.View
{
    /// <summary>
    /// Логика взаимодействия для AccountPage.xaml
    /// </summary>
    public partial class AccountPage : Page
    {
        public AccountPage(string username)
        {
            InitializeComponent();
            LoadUserData(username);
        }

        private void LoadUserData(string username)
        {
            string connectionString = "Host=localhost;Username=postgres;Password=WraiDexYT1;Database=MoneyManager";

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                string sql = "SELECT first_name, last_name, id FROM registration WHERE username = @username";

                using (var cmd = new NpgsqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@username", username);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            NameText.Text = reader.GetString(0);      // Имя
                            SurnameText.Text = reader.GetString(1);   // Фамилия
                            IdText.Text = $"ID: {reader.GetInt32(2)}"; // ID
                        }
                        else
                        {
                            NameText.Text = "Пользователь";
                            SurnameText.Text = "не найден";
                            IdText.Text = "ID: 0";
                        }
                    }
                }
            }
        }
    }
}
