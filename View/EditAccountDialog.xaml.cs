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
    /// <summary>
    /// Логика взаимодействия для EditAccountDialog.xaml
    /// </summary>
    public partial class EditAccountDialog : Window
    {
        public string NewFirstName { get; private set; }
        public string NewLastName { get; private set; }

        public EditAccountDialog(string currentFirstName, string currentLastName)
        {
            InitializeComponent();
            FirstNameBox.Text = currentFirstName;
            LastNameBox.Text = currentLastName;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            NewFirstName = FirstNameBox.Text;
            NewLastName = LastNameBox.Text;
            DialogResult = true;
            Close();
        }
    }
}
