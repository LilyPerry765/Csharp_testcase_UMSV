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
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.IO;
using Folder.Audio;

namespace Pendar.Ums.CompositeNodes.UserControls
{

    public partial class CodeStatusDialog : Window
    {
        private UMSV.UmsDataContext db = new UMSV.UmsDataContext();

        public CodeStatusDialog(UMSV.Schema.InvokeNode invokeNode)
        {
            InitializeComponent();
            DataContext = invokeNode;
            SetDataSources();
        }

        private UMSV.CodeStatus codeStatus;
        private UMSV.CodeStatus CodeStatus
        {
            get
            {
                if (codeStatus == null)
                {
                    codeStatus = db.CodeStatus.FirstOrDefault(cs => cs.ID.ToString() == InvokeNode.Arg[0].Value);
                    if (codeStatus == null)
                        db.CodeStatus.InsertOnSubmit(codeStatus = new UMSV.CodeStatus());
                }
                return codeStatus;
            }
        }

        private void SetDataSources()
        {
            recordsDataGrid.ItemsSource = CodeStatus.CodeStatusRecords;
            voiceDataGrid.ItemsSource = CodeStatus.CodeStatusVoiceMessages;
        }

        public UMSV.Schema.InvokeNode InvokeNode
        {
            get
            {
                return DataContext as UMSV.Schema.InvokeNode;
            }
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            db.SubmitChanges();
            InvokeNode.Arg[0].Value = CodeStatus.ID.ToString();
            DialogResult = true;
        }

        private void SelectVoice(object sender, RoutedEventArgs e)
        {
            Button voiceSelectButton = (sender as Button);
            if (voiceSelectButton.DataContext is UMSV.CodeStatusVoiceMessage)
            {
                CodeStatusVoiceDialog dlg = new CodeStatusVoiceDialog()
                                        {
                                            Owner = Window.GetWindow(this),
                                            VoiceData = (byte[])voiceSelectButton.Tag
                                        };
                if (dlg.ShowDialog() == true)
                {
                    voiceSelectButton.Tag = dlg.VoiceData;
                }
            }
            else
                Folder.MessageBox.ShowError("لطفا قبل از انتخاب پيام صوتی، عنوان وضعيت را مشخص نماييد.");
        }

        private void importCsvMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog()
            {
                Filter = "Comma Separated Values Files (*.csv;*.txt)|*.csv;*.txt"
            };

            if (dlg.ShowDialog() == true)
            {
                using (var txt = File.OpenText(dlg.FileName))
                {
                    ImportToCodeStatusRecords(txt.ReadToEnd());
                }
                RefreshRecordsDataGrid();
            }
        }

        private void RefreshRecordsDataGrid()
        {
            recordsDataGrid.ItemsSource = null;
            recordsDataGrid.ItemsSource = CodeStatus.CodeStatusRecords.ToArray();
        }


        private void RefreshVoiceDataGrid()
        {
            voiceDataGrid.ItemsSource = null;
            voiceDataGrid.ItemsSource = CodeStatus.CodeStatusVoiceMessages.ToArray();
        }


        private void ImportToCodeStatusRecords(string csvText)
        {
            string pattern = @"(?<Code>\d+)[,;](?<Status>.+)";
            var matches = Regex.Matches(csvText, pattern);
            foreach (Match match in matches)
            {
                UMSV.CodeStatusRecord r = new UMSV.CodeStatusRecord()
                {
                    CodeStatus = CodeStatus.ID,
                    Code = match.Groups["Code"].Value,
                    Status = match.Groups["Status"].Value.Trim()
                };
                CodeStatus.CodeStatusRecords.Add(r);
            }
        }

        private void deleteRecordsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            foreach (UMSV.CodeStatusRecord item in recordsDataGrid.SelectedItems)
            {
                CodeStatus.CodeStatusRecords.Remove(item);
            }
            RefreshRecordsDataGrid();
        }

        private void deleteVoiceMenuItem_Click(object sender, RoutedEventArgs e)
        {
            foreach (UMSV.CodeStatusVoiceMessage item in voiceDataGrid.SelectedItems)
            {
                CodeStatus.CodeStatusVoiceMessages.Remove(item);
            }
            RefreshVoiceDataGrid();
        }

        private void importVoiceMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog()
            {
                Filter = "ALaw Voice Files (*.wav;*.msg)|*.wav;*.msg",
                DefaultExt = "wav",
                Multiselect = true
            };
            if (dlg.ShowDialog() == true)
            {
                foreach (string fileName in dlg.FileNames)
                {
                    CodeStatus.CodeStatusVoiceMessages.Add(new UMSV.CodeStatusVoiceMessage()
                    {
                        CodeStatus = CodeStatus.ID,
                        Status = System.IO.Path.GetFileNameWithoutExtension(fileName),
                        Voice = System.IO.File.ReadAllBytes(fileName)
                    });
                }
                RefreshVoiceDataGrid();
            }
        }

        private void voiceDataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            (e.NewItem as UMSV.CodeStatusVoiceMessage).Voice = new byte[] { };
        }
    }
}
