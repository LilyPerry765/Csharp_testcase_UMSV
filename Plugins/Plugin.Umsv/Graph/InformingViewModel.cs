using System.Linq;
using System.Text;
using Folder.Commands;
using System.IO;
using Enterprise;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using UMSV;
using Folder;
using UMSV.Schema;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;

namespace UMSV.ViewModels
{
    public abstract class InformingViewModel : DataDrivenViewModel
    {
        protected bool IsNew;
        protected bool CancelImport;

        public int SelectedTabIndex
        {
            get;
            set;
        }

        protected void Refresh()
        {
            if (!IsNew)
            {
                using (UmsDataContext DB = new UmsDataContext())
                {
                    this.Informing = DB.Informings.Single(i => i == Informing);
                    SendPropertyChanged(null);
                }
            }
        }

        private Informing informing;
        public Informing Informing
        {
            get
            {
                return informing;
            }
            set
            {
                informing = value;
                informing.InformingRecords.ListChanged += (s, e) => SendPropertyChanged("CounterStatus");
            }
        }

        //public List<InformingRecord> InformingRecords
        //{
        //    get
        //    {
        //        return new List<InformingRecord>(Informing.InformingRecords);
        //    }
        //}

        public ListWrapper<InformingRecord> InformingRecords
        {
            get
            {
                return new ListWrapper<InformingRecord>(Informing.InformingRecords);
            }
        }

        protected void RemoveInvalidRecords()
        {
            var invalidRecors = Informing.InformingRecords.Where(ir => ir.Phone == null);
            while (invalidRecors.Any())
            {
                Informing.InformingRecords.Remove(invalidRecors.First());
            }
            SendPropertyChanged(null);
        }

        #region Status

        public string CounterStatus
        {
            get
            {
                string sep = new string(' ', 15);
                return "تعداد کل تماسها: "
                    + Informing.InformingRecords.Count + sep
                    + StatusCounter(InformingStatus.Queued) + sep
                    + StatusCounter(InformingStatus.Done) + sep
                    + StatusCounter(InformingStatus.UnsuccessfulCall) + sep
                    + StatusCounter(InformingStatus.Connected) + sep
                    + StatusCounter(InformingStatus.InProgress);
            }
        }

        private string StatusCounter(InformingStatus status)
        {
            if (status == InformingStatus.InProgress)
                return string.Format("{0}: {1}"
                      , Folder.Utility.GetEnumDescription(status)
                      , Informing.InformingRecords.Count(r => r.Status == (byte)InformingStatus.InProgress || r.Status == (byte)InformingStatus.SetupProgress));

            return string.Format("{0}: {1}"
                  , Folder.Utility.GetEnumDescription(status)
                  , Informing.InformingRecords.Count(r => r.Status == (byte)status));
        }

        #endregion


        #region Commands

        private DelegateCommand _ResetStatusCommand;
        public DelegateCommand ResetStatusCommand
        {
            get
            {
                if (_ResetStatusCommand == null)
                {
                    _ResetStatusCommand = new DelegateCommand(delegate
                    {
                        const string msg = "ریست کردن وضعیتها، وضعیت تمامی آیتمهای تماس را به حالت منتظر برده، آخرین زمان تماس را پاک کرده و تعداد تماسها را صفر میکند.\n ريست کردن وضعيت ها انجام شود؟";
                        if (Folder.MessageBox.ShowQuestion(msg) == System.Windows.MessageBoxResult.Yes)
                        {
                            Logger.WriteInfo("Reseting informing status...");
                            foreach (var item in Informing.InformingRecords)
                            {
                                item.Status = (byte)InformingStatus.Queued;
                                item.CallCount = 0;
                                item.LastCallTime = null;
                                item.LastDisconnectedTime = null;
                            }
                        }
                        SendPropertyChanged("InformingRecords");
                    }, () => Informing.InformingRecords.Any());
                }
                return _ResetStatusCommand;
            }
        }

        private DelegateCommand _RebuildCommand;
        public DelegateCommand RebuildCommand
        {
            get
            {
                if (_RebuildCommand == null)
                {
                    _RebuildCommand = new DelegateCommand(delegate
                    {
                        const string msg = "بازسازی  وضعیتها، وضعیت تمامی آیتمهای برقرار و در حال تماس را به حالت منتظر بر می گرداند و همچنین قبل از آن تمام ارسال ها متوقف خواهد شد.\n بازسازی وضعيت ها انجام شود؟";
                        if (Folder.MessageBox.ShowQuestion(msg) == System.Windows.MessageBoxResult.Yes)
                        {
                            VoIPServiceClient_Plugin_UMSV.Default.StopScheduledOutcall();

                            Logger.WriteInfo("Rebuilding informing status...");
                            foreach (var item in Informing.InformingRecords)
                            {
                                if (item.Status == (byte)InformingStatus.SetupProgress || item.Status == (byte)InformingStatus.InProgress || item.Status == (byte)InformingStatus.Connected)
                                    item.Status = (byte)InformingStatus.Queued;
                            }
                        }
                        SendPropertyChanged("InformingRecords");
                    }, () => Informing.InformingRecords.Any());
                }
                return _RebuildCommand;
            }
        }

        private DelegateCommand _RefreshCommand;
        public DelegateCommand RefreshCommand
        {
            get
            {
                if (_RefreshCommand == null)
                {
                    _RefreshCommand = new DelegateCommand(delegate
                    {
                        Refresh();
                    }, delegate
                    {
                        return !IsNew;
                    });
                }
                return _RefreshCommand;
            }
        }

        private DelegateCommand _ImportRecordsCommand;
        public DelegateCommand ImportRecordsCommand
        {
            get
            {
                if (_ImportRecordsCommand == null)
                {
                    _ImportRecordsCommand = new DelegateCommand(delegate
                    {
                        OpenFileDialog dlg = new OpenFileDialog()
                        {
                            Filter = "Comma Separated Values Files (*.csv;*.txt)|*.csv;*.txt",
                            Multiselect = false
                        };

                        if (dlg.ShowDialog() == true)
                        {

                            using (var txt = System.IO.File.OpenText(dlg.FileName))
                            {
                                string pattern = @"^(?<phone>\d+)[;,\s]*";
                                var matches = Regex.Matches(txt.ReadToEnd(), pattern, RegexOptions.Multiline);
                                CancelImport = false;
                                foreach (Match match in matches)
                                {
                                    if (CancelImport)
                                        return;
                                    Informing.InformingRecords.Add(new InformingRecord()
                                    {
                                        Phone = match.Groups["phone"].Value,
                                        Informing1 = Informing
                                    });
                                    System.Windows.Forms.Application.DoEvents();
                                }
                            }
                            SendPropertyChanged("InformingRecords");
                        }
                    });
                }
                return _ImportRecordsCommand;
            }
        }

        private DelegateCommand _ImportScheduleCommand;
        public DelegateCommand ImportScheduleCommand
        {
            get
            {
                if (_ImportScheduleCommand == null)
                {
                    _ImportScheduleCommand = new DelegateCommand(delegate
                    {

                        OpenFileDialog dlg = new OpenFileDialog()
                        {
                            Filter = "Comma Separated Values Files (*.csv;*.txt)|*.csv;*.txt",
                            Multiselect = false
                        };

                        if (dlg.ShowDialog() == true)
                        {
                            using (var txt = System.IO.File.OpenText(dlg.FileName))
                            {
                                string pattern = @"(?<Date>13\d{2}/[0,1]?\d/[0,1,2,3]?\d)[\s;,](?<StartTime>[0,1,2]?\d:[0,1,2,3,4,5]?\d)[\s;,](?<FinishTime>[0,1,2]?\d:[0,1,2,3,4,5]?\d)";
                                var matches = Regex.Matches(txt.ReadToEnd(), pattern, RegexOptions.Multiline);
                                foreach (Match match in matches)
                                {
                                    Informing.Schedule.Times.Add(new ScheduleTime()
                                    {
                                        Date = PersianDateTime.Parse(match.Groups["Date"].Value).ToGregorian(),
                                        Start = match.Groups["StartTime"].Value,
                                        Finish = match.Groups["FinishTime"].Value,
                                    });
                                }
                            }
                            SendPropertyChanged("InformingRecords");
                        }
                    });
                }
                return _ImportScheduleCommand;
            }
        }

        private DelegateCommand _SortedExportCommand;
        public DelegateCommand SortedExportCommand
        {
            get
            {
                if (_SortedExportCommand == null)
                {
                    _SortedExportCommand = new DelegateCommand(delegate
                    {
                        SaveFileDialog dlg = new SaveFileDialog()
                        {
                            Filter = "Comma Separated Values Files (*.csv)|*.csv",
                            AddExtension = true,
                            DefaultExt = "csv",
                        };
                        if (dlg.ShowDialog() == true)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine("شماره تلفن\tوضعیت\tتاریخ آخرین تماس\tزمان آخرین تماس\tزمان آخرین قطع\tتعداد دفعات تماس");

                            List<InformingRecord> InformingRecordsList = Informing.InformingRecords.OrderBy(t => t.Status).ToList();

                            foreach (var ir in InformingRecordsList)
                            {
                                string date = ir.LastCallTime.HasValue ? new PersianDateTime(ir.LastCallTime.Value).ToString("yyyy/MM/dd") : "-";
                                string callTime = ir.LastCallTime.HasValue ? ir.LastCallTime.Value.ToString("HH:mm:ss") : "-";
                                string disconnectTime = ir.LastDisconnectedTime.HasValue ? ir.LastDisconnectedTime.Value.ToString("HH:mm:ss") : "-";
                                sb.AppendFormat("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}\r\n", "\t", ir.Phone, Folder.Utility.GetEnumDescription((InformingStatus)ir.Status), date, callTime, disconnectTime, ir.CallCount);
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
                    }, () => Informing.InformingRecords.Any());
                }
                return _SortedExportCommand;
            }
        }

        private DelegateCommand _ExportCommand;
        public DelegateCommand ExportCommand
        {
            get
            {
                if (_ExportCommand == null)
                {
                    _ExportCommand = new DelegateCommand(delegate
                    {
                        SaveFileDialog dlg = new SaveFileDialog()
                        {
                            Filter = "Comma Separated Values Files (*.csv)|*.csv",
                            AddExtension = true,
                            DefaultExt = "csv",
                        };
                        if (dlg.ShowDialog() == true)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine("شماره تلفن\tوضعیت\tتاریخ آخرین تماس\tزمان آخرین تماس\tزمان آخرین قطع\tتعداد دفعات تماس");
                            foreach (var ir in Informing.InformingRecords)
                            {
                                string date = ir.LastCallTime.HasValue ? new PersianDateTime(ir.LastCallTime.Value).ToString("yyyy/MM/dd") : "-";
                                string callTime = ir.LastCallTime.HasValue ? ir.LastCallTime.Value.ToString("HH:mm:ss") : "-";
                                string disconnectTime = ir.LastDisconnectedTime.HasValue ? ir.LastDisconnectedTime.Value.ToString("HH:mm:ss") : "-";
                                sb.AppendFormat("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}\r\n", "\t", ir.Phone, Folder.Utility.GetEnumDescription((InformingStatus)ir.Status), date, callTime, disconnectTime, ir.CallCount);
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
                    }, () => Informing.InformingRecords.Any());
                }
                return _ExportCommand;
            }
        }

        private DelegateCommand _AddRecordCommand;
        public DelegateCommand AddRecordCommand
        {
            get
            {
                if (_AddRecordCommand == null)
                {
                    _AddRecordCommand = new DelegateCommand(delegate
                    {
                        Informing.InformingRecords.Add(new InformingRecord()
                        {
                            Informing1 = Informing
                        });
                        SendPropertyChanged("InformingRecords");
                    });
                }
                return _AddRecordCommand;
            }
        }

        private DelegateCommand _AddScheduleCommand;
        public DelegateCommand AddScheduleCommand
        {
            get
            {
                if (_AddScheduleCommand == null)
                {
                    _AddScheduleCommand = new DelegateCommand(delegate
                    {
                        Informing.Schedule.Times.Add(new UMSV.Schema.ScheduleTime()
                        {
                        });
                        SendPropertyChanged("Informing");
                    });
                }
                return _AddScheduleCommand;
            }
        }

        private DelegateCommand _DeleteAllCommand;
        public DelegateCommand DeleteAllCommand
        {
            get
            {
                if (_DeleteAllCommand == null)
                {
                    _DeleteAllCommand = new DelegateCommand(delegate
                    {
                        Informing.InformingRecords.Clear();
                        SendPropertyChanged("InformingRecords");
                    }, () => Informing.InformingRecords.Any());
                }
                return _DeleteAllCommand;
            }
        }

        private DelegateCommand _OKCommand;
        public DelegateCommand OKCommand
        {
            get
            {
                if (_OKCommand == null)
                {
                    _OKCommand = new DelegateCommand(delegate
                    {
                        if (OnSave())
                        {
                            DialogResult = true;
                            SendPropertyChanged("DialogResult");
                        }
                    });
                }
                return _OKCommand;
            }
        }

        #endregion

    }
}
