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
using Folder;
using UMSV;

namespace UMSV
{
    /// <summary>
    /// Interaction logic for VoicesForm.xaml
    /// </summary>
    public partial class UmsVoicesForm : UserControl, IFolderForm, IDataGridForm, IEditableForm
    {
        public UmsVoicesForm()
        {
            InitializeComponent();
        }

        #region IDataGridForm Members

        public DataGrid DataGrid
        {
            get
            {
                return this.dataGrid;
            }
        }

        #endregion

        UmsDataContext dc;

        #region IFolderForm Members

        public void Initialize(FolderFormHelper helper)
        {
            dc = new UmsDataContext();
            var groups = Folder.Utility.CreateEnumDataSource(typeof(VoiceGroup), typeof(byte));
            groups.Insert(0, new NameValue()
            {
                Name = string.Empty,
                Value = null
            });
            Group.ItemsSource = groups;
        }

        #endregion

        #region IEditableForm Members

        public bool Save()
        {
            dc.SubmitChanges();
            return true;
        }

        #endregion

        private void dataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var voice = dataGrid.CurrentCellItem<Voice>();
            if (voice == null)
            {
                VoicePanel.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                VoicePanel.Visibility = System.Windows.Visibility.Visible;
                SelectedVoiceName.Content = voice.Name;

                if (voice.Data == null)
                    soundControl.Voice = null;
                else
                    soundControl.Voice = voice.Data.ToArray();
            }
        }

        private void soundControl_VoiceChanged(object sender, Folder.Audio.VoiceChangedEventArgs e)
        {
            var voice = dataGrid.CurrentCellItem<Voice>();
            voice.Data = new System.Data.Linq.Binary(soundControl.Voice);
        }

        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.ItemsSource = dc.Voices.Where(v =>
                (Group.SelectedValue == null || (byte?)Group.SelectedValue == v.VoiceGroup) &&
                (string.IsNullOrEmpty(Title.Text) || v.Name.ToLower().Contains(Title.Text.ToLower())) &&
                (string.IsNullOrEmpty(Comment.Text) || v.Description.ToLower().Contains(Comment.Text.ToLower()))
                );
        }

        private void mitClearDB_Click(object sender, RoutedEventArgs e)
        {
            var result = Folder.MessageBox.ShowQuestion("  این پروسه ممکن است زمان زیادی به طول بیانجامد ، لطفا صبور باشد. آیا مایل به ادامه هستید؟");
            int counter = 0;
            try
            {

                if (result == MessageBoxResult.OK)
                {
                    var Voices = dc.Voices.Where(p => p.VoiceGroup == 5);
                    foreach (Voice voice in Voices)
                    {
                        bool isUsable = false;
                        foreach (Graph graph in dc.Graphs)
                        {
                            string xml = graph.Data.ToString();

                            if (xml.Contains(voice.ID.ToString()))
                            {
                                isUsable = true;
                                break;
                            }
                        }

                        if (!isUsable)
                        {
                            dc.Voices.DeleteOnSubmit(voice);
                            counter++;
                        }
                    }
                    dc.SubmitChanges();
                    Folder.MessageBox.ShowInfo("عملیات پاکسازی به اتمام رسید و {0 رکورد حذف گردید.}", counter);
                }
            }
            catch (Exception ex)
            {
                Folder.MessageBox.ShowError("خطای زیر رخ داده {0}", ex.Message);
                throw;
            }
        }
    }
}
