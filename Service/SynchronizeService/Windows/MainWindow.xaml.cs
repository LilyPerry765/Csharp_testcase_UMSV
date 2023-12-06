using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using Enterprise;

namespace SynchronizeService.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        Codes.SyncTimer syncTimer = new Codes.SyncTimer();

        private void btnStart_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
				Logger.WriteEnd("Start to be click");
                btnStart.IsEnabled = false;
                btnStop.IsEnabled = true;

                syncTimer.Start(pickerStartDate.SelectedDate, pickerToDate.SelectedDate);
            }
            catch (Exception ex)
            {
                Logger.WriteError(ex.ToString());
            }
        }

        private void btnStop_Click_1(object sender, RoutedEventArgs e)
        {
            syncTimer.Stop();

            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;
        }
    }
}
