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
using Pendar.Ums.Model;
using Microsoft.Win32;
using System.IO;
using UMSV;

namespace Pendar.Ums.CompositeNodes.UserControls
{

    public partial class InfoTableDialog : Window
    {
        private UmsDataContext db = new UmsDataContext();

        public InfoTableDialog(string infoTableID)
        {
            InitializeComponent();
            infoTable = db.InfoTables.FirstOrDefault(it => it.ID.ToString() == infoTableID);
            RefreshDataGrid();   
        }

        private InfoTable infoTable;
        public InfoTable InfoTable
        {
            get
            {
                if (infoTable == null)
                {
                    db.InfoTables.InsertOnSubmit(infoTable = new InfoTable()
                    {
                        ID = Guid.NewGuid(),
                        Description = "جدول اطلاعات گویا"
                    });
                }
                return infoTable;
            }
        }

        private void RefreshDataGrid()
        {
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = InfoTable.InfoTableRecords.ToArray();
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            db.SubmitChanges();
            DialogResult = true;
        }

        private void importButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog()
            {
                Filter = "Comma Separated Values Files (*.csv;*.txt)|*.csv;*.txt"
            };

            if (dlg.ShowDialog() == true)
            {
                using (var txt = File.OpenText(dlg.FileName))
                {
                    ImportToInfoTable(txt.ReadToEnd());
                }
               RefreshDataGrid();
            }
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            InfoTable.InfoTableRecords.Clear();
            RefreshDataGrid();
        }

        private void ImportToInfoTable(string csvText)
        {
            string pattern = @"(?<ID>\d+)[;,](?<Data>.+)";
            var matches = Regex.Matches(csvText, pattern);
            InfoTable.InfoTableRecords.Clear();
            foreach (Match match in matches)
            {
                InfoTableRecord r = new InfoTableRecord()
                {
                    InfoTable = InfoTable.ID,
                    ID = match.Groups["ID"].Value,
                    Data = match.Groups["Data"].Value.Trim()
                };
                InfoTable.InfoTableRecords.Add(r);
            }
        }
    }
}
