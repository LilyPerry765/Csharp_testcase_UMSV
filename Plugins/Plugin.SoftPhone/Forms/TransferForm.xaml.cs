using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UMSV
{
    /// <summary>
    /// Interaction logic for TransferForm.xaml
    /// </summary>
    public partial class TransferForm : Window
    {
        public TransferForm()
        {
            InitializeComponent();
        }

        private static string Number;

        private void TransferButton_Click(object sender, RoutedEventArgs e)
        {
            Number = PhoneNumberTextbox.Text;
            DialogResult = true;
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PhoneNumberTextbox.Text = Number;
            PhoneNumberTextbox.Focus();
        }
    }
}
