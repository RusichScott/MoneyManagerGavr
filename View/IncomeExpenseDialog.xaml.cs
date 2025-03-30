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
using System.Windows.Shapes;

namespace MoneyManagerGavr.View
{
    public partial class IncomeExpenseDialog : Window
    {
        public decimal Amount { get; private set; }

        public IncomeExpenseDialog(string title)
        {
            InitializeComponent();
            Title = title;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(AmountTextBox.Text, out decimal amount))
            {
                Amount = amount;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Введите корректную сумму");
            }
        }
    }
}
