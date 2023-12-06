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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace UMSV
{
    /// <summary>
    /// Interaction logic for BlackButton.xaml
    /// </summary>
    [DefaultEvent("Changed")]
    public partial class BlackCheckBox : UserControl
    {
        public BlackCheckBox()
        {
            InitializeComponent();
        }

        public string Text
        {
            get
            {
                return (string)Label.Content;
            }
            set
            {
                Label.Content = value;
            }
        }

        public bool IsChecked
        {
            get
            {
                return Control.IsChecked.Value;
            }
            set
            {
                Control.IsChecked = value;
            }
        }

        public event RoutedEventHandler Changed;

        private void Control_Checked(object sender, RoutedEventArgs e)
        {
            if (Changed != null)
                Changed(this, e);
        }
    }
}
