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
    /// Логика взаимодействия для SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void ThemeToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var newTheme = new Uri("Themes/DarkTheme.xaml", UriKind.Relative);

            // Если уже тёмная тема - переключаем на светлую
            if (Application.Current.Resources.MergedDictionaries.Any(d =>
                d.Source != null && d.Source.OriginalString.Contains("DarkTheme")))
            {
                newTheme = new Uri("Themes/LightTheme.xaml", UriKind.Relative);
                ThemeToggleButton.Content = "Включить тёмную тему";
            }
            else
            {
                ThemeToggleButton.Content = "Включить светлую тему";
            }

            // Применяем новую тему
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(
                new ResourceDictionary() { Source = newTheme });

            // Сохраняем настройки
            Properties.Settings.Default.DarkTheme = newTheme.OriginalString.Contains("Dark");
            Properties.Settings.Default.Save();
        }
    }
}
