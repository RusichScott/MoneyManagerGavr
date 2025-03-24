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

namespace MoneyManagerGavr.View
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        Window loginWindow;

        public LoginPage(Window loginWindow)
        {
            InitializeComponent();
            this.loginWindow = loginWindow;
        }
        
        private void Regestration_Click(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new RegistrationPage());
        }

        private void Login_Button_Click(object sender, RoutedEventArgs e)
        {
            string username = usernameTextBox.Text;
            string password = passwordBox.Password;

            var dbHelper = new DatabaseHelper("localhost", "postgres", "WraiDexYT1", "MoneyManager");

            if (dbHelper.ValidateCredentials(username, password))
            {
                MessageBox.Show("Вход выполнен успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                ManagerWindow managerWindow = new ManagerWindow();
                MessageBox.Show("Вы успешно вошли в систему");
                managerWindow.Show();
                loginWindow.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
