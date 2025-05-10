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
using System.Windows;
using System.Windows.Controls;
using static MoneyManagerGavr.View.LoginPage;
using MessageBox = System.Windows.MessageBox;

namespace MoneyManagerGavr.View
{
    public partial class AccountPage : Page
    {
        private readonly DatabaseHelper _dbHelper;

        public AccountPage()
        {
            InitializeComponent();
            _dbHelper = new DatabaseHelper("localhost", "postgres", "WraiDexYT1", "MoneyManager");
            LoadAccountData();
        }

        private void LoadAccountData()
        {
            if (!AppContext.IsLoggedIn) return;

            // Загрузка данных пользователя из БД
            var userData = _dbHelper.GetUserData(AppContext.CurrentUser);
            if (userData != null)
            {
                NameText.Text = userData.FirstName;
                SurnameText.Text = userData.LastName;
                IdText.Text = $"ID: {userData.Id}";
            }
        }

        private void EditAccount_Click(object sender, RoutedEventArgs e)
        {
            var editDialog = new EditAccountDialog(
                NameText.Text,
                SurnameText.Text);

            if (editDialog.ShowDialog() == true)
            {
                // Обновляем данные в БД
                bool success = _dbHelper.UpdateUserAccount(
                    AppContext.CurrentUser,
                    editDialog.NewFirstName,
                    editDialog.NewLastName);

                if (success)
                {
                    MessageBox.Show("Данные успешно обновлены!");
                    LoadAccountData(); // Обновляем отображение
                }
                else
                {
                    MessageBox.Show("Ошибка при обновлении данных");
                }
            }
        }

        private void DeleteAccount_Click(object sender, RoutedEventArgs e)
        {
            var confirmResult = MessageBox.Show(
                "Вы точно хотите удалить аккаунт? Это действие нельзя отменить!",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirmResult == MessageBoxResult.Yes)
            {
                bool success = _dbHelper.DeleteUserAccount(AppContext.CurrentUser);

                if (success)
                {
                    MessageBox.Show("Аккаунт успешно удалён");
                    AppContext.Logout();
                    NavigationService?.Navigate(new LoginPage(new LoginWindow()));
                }
                else
                {
                    MessageBox.Show("Ошибка при удалении аккаунта");
                }
            }
        }
    }
}
