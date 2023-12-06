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
    [DefaultEvent("Click")]
    public partial class BlackButton : UserControl
    {
        public BlackButton()
        {
            InitializeComponent();
        }

        public event RoutedEventHandler Click;

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

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Click != null)
                Click(this, new RoutedEventArgs());
        }
    }
}
