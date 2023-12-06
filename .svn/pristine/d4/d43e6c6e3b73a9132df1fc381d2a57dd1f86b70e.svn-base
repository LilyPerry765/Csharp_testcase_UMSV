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

namespace Pendar.Ums.CompositeNodes.UserControls
{

    public partial class InfoTableSelector : TextBlock
    {
        public InfoTableSelector()
        {
            InitializeComponent();
        }

        public string SelectedInfoTableID
        {
            get
            {
                return (string)GetValue(SelectedInfoTableIDProperty);
            }
            set
            {
                SetValue(SelectedInfoTableIDProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedInfoTableIDProperty =
            DependencyProperty.Register("SelectedInfoTableID", typeof(string), typeof(InfoTableSelector), new FrameworkPropertyMetadata()
            {
                BindsTwoWayByDefault = true
            });

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            InfoTableDialog dlg = new InfoTableDialog(SelectedInfoTableID)
            {
                Owner = Window.GetWindow(this),
            };
            if (dlg.ShowDialog() == true)
            {
                SelectedInfoTableID = dlg.InfoTable.ID.ToString();
            }
        }


    }
}
