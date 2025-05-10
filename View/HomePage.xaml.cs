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
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Media;

namespace MoneyManagerGavr.View
{
    public partial class HomePage : Page
    {
        private readonly DatabaseHelper _dbHelper;

        public SeriesCollection IncomeSeries { get; set; }
        public SeriesCollection ExpenseSeries { get; set; }

        public HomePage()
        {
            InitializeComponent();
            _dbHelper = new DatabaseHelper("localhost", "postgres", "WraiDexYT1", "MoneyManager");
            LoadAccountBalance();

        }

        private void LoadAccountBalance()
        {
            if (!AppContext.IsLoggedIn) return;

            try
            {
                var balance = _dbHelper.GetAccountBalance(AppContext.CurrentUser);
                MainBalance.Text = $"{balance:0.0}р";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки баланса: {ex.Message}");
            }
        }

        // Кнопка "Добавить доход"
        private void AddIncome_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new IncomeExpenseDialog("Добавить доход");
            if (dialog.ShowDialog() == true)
            {
                _dbHelper.AddTransaction(AppContext.CurrentUser, dialog.Amount, true);
                LoadAccountBalance();
            }
        }

        // Кнопка "Добавить расход"
        private void AddExpense_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new IncomeExpenseDialog("Добавить расход");
            if (dialog.ShowDialog() == true)
            {
                _dbHelper.AddTransaction(AppContext.CurrentUser, dialog.Amount, false);
                LoadAccountBalance();
            }
        }

        // Кнопка "Создать счёт"
        private void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            _dbHelper.CreateAccount(AppContext.CurrentUser);
            LoadAccountBalance();
        }
    }
}