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
using Enterprise;
using System.Linq.Expressions;
using Folder;
using Folder.Audio;
using System.IO;


namespace UMSV.Forms
{
    /// <summary>
    /// Interaction logic for TotalCallReport.xaml
    /// </summary>
    public partial class TotalCallReport : UserControl, IFolderForm
    {
        UmsDataContext dc = new UmsDataContext();
        public TotalCallReport()
        {
            InitializeComponent();
        }

        public void Initialize(FolderFormHelper helper)
        {
            FromDate.SelectedDate = DateTime.Today;
        }


        void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            var totalCall=dc.Calls.Count(a=>a.Type==1);
            TotalCall.Text = totalCall.ToString();

            var answeredCall = dc.Calls.Count(a => a.AnswerTime.HasValue && a.Type == 3);
            TotalAnswerd.Text = answeredCall.ToString();

            var disconnectedCall = dc.Calls.Count(a => !a.AnswerTime.HasValue && a.Type == 1);
            DisconnectedCall.Text = disconnectedCall.ToString();

            var waitedCall = dc.Calls.Where(a => (a.AnswerTime.Value-a.CallTime).TotalSeconds > 5).Count();
            WaitedCall.Text = waitedCall.ToString();

            var avgWaitedCall = dc.Calls.Where(a => a.AnswerTime != null && (a.Type == 3 && (a.AnswerTime.Value - a.CallTime).TotalSeconds > 5))
                                      .Sum(a => ((DateTime)a.AnswerTime - a.CallTime).TotalSeconds);
            avgWaitedCall /= waitedCall;
            AvgWaitedCall.Text =string.Format("{0} ثانیه", (int)avgWaitedCall);
            
            var maxWaitedCall=dc.Calls.Where(a => a.AnswerTime != null && a.Type == 3)
                                      .Max(a =>a.AnswerTime.Value-a.CallTime);
            MaxWaitedCall.Text = maxWaitedCall.ToString();

            var avgAnswerdCall = dc.Calls.Where(a => a.Type == 3 && a.AnswerTime.HasValue && a.DisconnectTime!=null)
                                       .Sum(a=>(a.DisconnectTime-(DateTime)a.AnswerTime).TotalSeconds);
            avgAnswerdCall /= answeredCall;
            AVGAnswerdCall.Text = string.Format("{0} ثانیه", (int)avgAnswerdCall);

            var maxCallTime = dc.Calls.Where(a => a.AnswerTime != null && a.Type == 3 && a.DisconnectTime!=null)
                                      .Max(a => a.DisconnectTime-a.AnswerTime);
            MaxCallTime.Text = maxCallTime.ToString();




        }
    }
}

