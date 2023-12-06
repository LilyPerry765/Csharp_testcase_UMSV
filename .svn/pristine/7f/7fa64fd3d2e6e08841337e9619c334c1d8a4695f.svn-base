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

namespace UMSV
{
    public partial class TalkSummaryReportForm : UserControl, IDataGridForm, IFolderForm
    {
        UmsDataContext dc = new UmsDataContext();
        List<User> SubUsers;
        public TalkSummaryReportForm()
        {
            InitializeComponent();
        }

        public void Initialize(FolderFormHelper helper)
        {
            var subUsersIDs = Folder.Utility.GetSubUsers();
            SubUsers = new FolderDataContext().Users.Where(u => subUsersIDs.Contains(u.ID)).OrderBy(o => o.Fullname).ToList();

            var operators = FillOperatorsList();

            using (UmsDataContext dc = new UmsDataContext())
            {
                var graphs = dc.Graphs.ToList();
                graphs.Insert(0, new Graph() {
                    ID = Guid.Empty,
                    Name = "",
                });
                ServiceColumnFilter.ItemsSource = graphs;
            }

            UserColumn.ItemsSource = OperatorComboxBox.ItemsSource = operators;
            OperatorComboxBox.SelectedIndex = 0;

            FromDate.SelectedDate = DateTime.Today;
        }

        private List<NameValue> FillOperatorsList()
        {

            var operators = SubUsers.Select(u => new NameValue() {
                Value = u.Username, Name = u.Fullname
            }).ToList();

            operators.Insert(0, new NameValue() {
                Name = "همه کاربران",
                Value = 0
            });

            return operators;
        }

        private string GetUserGroup(string username)
        {
            var user = (UserColumn.ItemsSource as List<NameValue>).FirstOrDefault(u => u.Value.ToString() == username);
            if (user == null)
                return string.Empty;
            else
                return user.Comment;
        }

        void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.Visibility = System.Windows.Visibility.Visible;
            chart.Visibility = System.Windows.Visibility.Collapsed;
            LoadData(dataGrid);
        }

        private void LoadData(UIElement element)
        {
            try
            {
                dc = new UmsDataContext();

                Folder.Console.ShowProgress();
                DateTime? from = FromDate.SelectedDate;
                DateTime? to = ToDate.SelectedDate;
                Guid selectedService = ServiceColumnFilter.SelectedValue == null ? Guid.Empty : (Guid)(Guid?)ServiceColumnFilter.SelectedValue;

                List<string> calleeID;
                if (OperatorComboxBox.SelectedIndex != 0)
                {
                    calleeID = new List<string>();
                    calleeID.Add(OperatorComboxBox.SelectedValue.ToString());
                    CalleeIDTextbox.Clear();
                }
                else
                {
                    if (String.IsNullOrEmpty(CalleeIDTextbox.Text))
                        calleeID = ((List<NameValue>)OperatorComboxBox.ItemsSource).Select(n => n.Value.ToString()).ToList();
                    else
                    {
                        calleeID = new List<string>();
                        calleeID.Add(CalleeIDTextbox.Text);
                    }
                }

                var value = dc.Calls.Where(c =>
                        (selectedService == Guid.Empty || (c.GraphID.HasValue && selectedService == (Guid)c.GraphID)) &&
                        (c.Type == (int)DialogType.CallTransfer) &&
                        (from == null || c.CallTime >= from) &&
                        calleeID.Contains(c.CalleeID) &&
                        (to == null || c.CallTime < to));

                var data = (from v in value
                            group v by v.CalleeID into g
                            select new {
                                Target = g.Key,
                                Group = GetUserGroup(g.Key),
                                Count = g.Count(),
                                AnswerCount = g.Count(i => i.AnswerTime.HasValue),
                                NoAnswerCount = g.Count(i => !i.AnswerTime.HasValue),
                                RejectedCount = g.Count(i => i.DisconnectCause == (int)DisconnectCause.CallRejected),
                            }).ToList();

                Folder.Console.HideProgress();
                if (element == dataGrid)
                    dataGrid.ItemsSource = data;
                else
                    CallAnswerCountsSeri.ItemsSource = CallCountsSeri.ItemsSource = data;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
            finally
            {
                Folder.Console.HideProgress();
            }
        }

        #region IDataGridForm Members

        public DataGrid DataGrid
        {
            get
            {
                return dataGrid;
            }
        }

        #endregion

        private void ChartButton_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.Visibility = System.Windows.Visibility.Collapsed;
            chart.Visibility = System.Windows.Visibility.Visible;
            LoadData(chart);
        }
    }
}
