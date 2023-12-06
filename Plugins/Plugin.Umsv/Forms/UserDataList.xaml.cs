using Folder;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace UMSV
{
    public partial class UserDataList : UserControl
    {
        public UserDataList()
        {
            InitializeComponent();

            FromDate.SelectedDate = DateTime.Now;
            ToDate.SelectedDate = DateTime.Now;

        }

        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            if (FromDate.SelectedDate == null || ToDate.SelectedDate == null)
            {
                Folder.MessageBox.ShowInfo("لطفا تاریخ شروع و پایان را وارد کنید");
                return;
            }

            UMSV.UmsDataContext dc = new UmsDataContext();

            dataGrid.ItemsSource = dc.UserDatas.Where(u => (!FromDate.SelectedDate.HasValue || u.CallTime >= FromDate.SelectedDate.Value) &&
                                                           (!ToDate.SelectedDate.HasValue || u.CallTime <= ToDate.SelectedDate.Value)).OrderBy(u => u.CallTime).ToList();
            
        }



        private void ExportMenu_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog()
            {
                Filter = "Comma Separated Values Files (*.csv)|*.csv",
                AddExtension = true,
                DefaultExt = "csv",
            };
            if (dlg.ShowDialog() == true)
            {

                UMSV.UmsDataContext dc = new UmsDataContext();

             var userDataList=dc.UserDatas.Where(u => (!FromDate.SelectedDate.HasValue || u.CallTime >= FromDate.SelectedDate.Value) &&
                                                               (!ToDate.SelectedDate.HasValue || u.CallTime <= ToDate.SelectedDate.Value)).OrderBy(u => u.CallTime).ToList();


                StringBuilder sb = new StringBuilder();
                sb.AppendLine("سرویس\tشماره مبدا\tتاریخ تماس\tزمان تماس\tاطلاعات وارد شده");
                foreach (var ir in userDataList)
                {
                    string date = new PersianDateTime(ir.CallTime).ToString("yyyy/MM/dd");
                    string time = new PersianDateTime(ir.CallTime).ToString("HH:mm");
                    sb.AppendFormat("{1}{0}{2}{0}{3}{0}{4}{0}{5}\r\n", "\t", ir.Graph.Name, ir.CallerID, date, time, ir.Data);
                }

                try
                {
                    using (StreamWriter wr = new StreamWriter(dlg.FileName, false, Encoding.Unicode))
                    {
                        wr.Write(sb);
                    }
                }
                catch (IOException)
                {
                    Folder.MessageBox.ShowError("نرم افزار ديگری در حال استفاده از اين فايل می باشد. لطفا ابتدا ساير نرم افزار ها را ببنديد و مجددا اقدام نماييد.");
                }
            }
        }
    }
}
