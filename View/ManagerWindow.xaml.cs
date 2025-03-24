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
    /// Логика взаимодействия для ManagerWindow.xaml
    /// </summary>
    public partial class ManagerWindow : Window
    {
        public ManagerWindow()
        {
            InitializeComponent();
        }

        private void Home_Image_Click(object sender, MouseButtonEventArgs e)
        {
            ManagerFrame.Content = new HomePage();
        }

        private void Income_Image_Click(object sender, MouseButtonEventArgs e)
        {
            ManagerFrame.Content = new IncomePage();
        }

        private void Spendings_Image_Click(object sender, MouseButtonEventArgs e)
        {
            ManagerFrame.Content = new SpendingsPage();
        }

        private void Account_Image_Click(object sender, MouseButtonEventArgs e)
        {
            string currentUserLogin = "логин_пользователя";

            // Просто открываем новое окно
            var accountWindow = new AccountPage(currentUserLogin);
            ManagerFrame.Content = accountWindow;
        }

        private void Settings_Image_Click(object sender, MouseButtonEventArgs e)
        {
            ManagerFrame.Content = new SettingsPage();
        }
    }
}
