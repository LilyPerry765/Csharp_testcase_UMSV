using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Folder;
using System.Collections;
using System.Threading;
using Enterprise;
using Folder.EMQ;
using System.Windows.Media;

namespace UMSV
{
    public class DashboardPanelModel : ViewModelBase, IFolderForm
    {
        public DashboardPanelModel()
        {
            this.CurrentActiveLinks = UMSV.Schema.MediaGatewayConfig.Default.Links.Count(l => l.IsEnabled);
            this.ActivePhoneLines = this.CurrentActiveLinks * 30;
            this.LastUpdatingTime = "در حال اتصال به سرور ...";
            this.OptimalRangeStart = ActivePhoneLines / 2;
            this.OptimalRangeEnd = ActivePhoneLines * 3 / 4;
        }

        #region Property ActivePhoneLines
        private int _ActivePhoneLines;
        public int ActivePhoneLines
        {
            get
            {
                return _ActivePhoneLines;
            }
            set
            {
                _ActivePhoneLines = value;
                SendPropertyChanged("ActivePhoneLines");
            }
        }
        #endregion

        #region Property OptimalRangeStart
        private int _OptimalRangeStart;
        public int OptimalRangeStart
        {
            get
            {
                return _OptimalRangeStart;
            }
            set
            {
                _OptimalRangeStart = value;
                SendPropertyChanged("OptimalRangeStart");
            }
        }
        #endregion

        #region Property OptimalRangeEnd
        private int _OptimalRangeEnd;
        public int OptimalRangeEnd
        {
            get
            {
                return _OptimalRangeEnd;
            }
            set
            {
                _OptimalRangeEnd = value;
                SendPropertyChanged("OptimalRangeEnd");
            }
        }
        #endregion

        #region Property CallsHistory
        private PointCollection _CallsHistory;
        public PointCollection CallsHistory
        {
            get
            {
                return _CallsHistory;
            }
            set
            {
                _CallsHistory = value;
                SendPropertyChanged("CallsHistory");
            }
        }
        #endregion



        #region Property LinksCountVisibility
        public System.Windows.Visibility LinksCountVisibility
        {
            get
            {
                return UMSV.Schema.Config.Default.DashboardMonitorLinksCount ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
        }
        #endregion

        #region Property ServerSystemParamsVisibility
        public System.Windows.Visibility ServerSystemParamsVisibility
        {
            get
            {
                return UMSV.Schema.Config.Default.DashboardMonitorServerSystemParams ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
        }
        #endregion

        #region Property CurrentCalls
        private int _CurrentCalls;
        public int CurrentCalls
        {
            get
            {
                return _CurrentCalls;
            }
            set
            {
                _CurrentCalls = value;
                SendPropertyChanged("CurrentCalls");
            }
        }
        #endregion

        #region Property OnlineOperators
        private int _OnlineOperators;
        public int OnlineOperators
        {
            get
            {
                return _OnlineOperators;
            }
            set
            {
                _OnlineOperators = value;
                SendPropertyChanged("OnlineOperators");
            }
        }
        #endregion

        #region Property DialogsCount
        private int _DialogsCount;
        public int DialogsCount
        {
            get
            {
                return _DialogsCount;
            }
            set
            {
                _DialogsCount = value;
                SendPropertyChanged("DialogsCount");
            }
        }
        #endregion

        #region Property LastUpdatingTime
        private string _LastUpdatingTime;
        public string LastUpdatingTime
        {
            get
            {
                return _LastUpdatingTime;
            }
            set
            {
                _LastUpdatingTime = value;
                SendPropertyChanged("LastUpdatingTime");
            }
        }
        #endregion

        #region Property TodayCallsCount
        private int _TodayCallsCount;
        public int TodayCallsCount
        {
            get
            {
                return _TodayCallsCount;
            }
            set
            {
                _TodayCallsCount = value;
                SendPropertyChanged("TodayCallsCount");
            }
        }
        #endregion

        #region Property CurrentActiveLinks
        private int _CurrentActiveLinks;
        public int CurrentActiveLinks
        {
            get
            {
                return _CurrentActiveLinks;
            }
            set
            {
                _CurrentActiveLinks = value;
                SendPropertyChanged("CurrentActiveLinks");
            }
        }
        #endregion

        #region Property TodayDialogsCount
        private int _TodayDialogsCount;
        public int TodayDialogsCount
        {
            get
            {
                return _TodayDialogsCount;
            }
            set
            {
                _TodayDialogsCount = value;
                SendPropertyChanged("TodayDialogsCount");
            }
        }
        #endregion

        #region Property CpuUsage
        private int _CpuUsage;
        public int CpuUsage
        {
            get
            {
                return _CpuUsage;
            }
            set
            {
                _CpuUsage = value;
                SendPropertyChanged("CpuUsage");
            }
        }
        #endregion

        #region Property TotalMemory
        private int _TotalMemory;
        public int TotalMemory
        {
            get
            {
                return _TotalMemory;
            }
            set
            {
                _TotalMemory = value;
                SendPropertyChanged("TotalMemory");
            }
        }
        #endregion

        #region Property MemoryUsage
        private int _MemoryUsage;
        public int MemoryUsage
        {
            get
            {
                return _MemoryUsage;
            }
            set
            {
                _MemoryUsage = value;
                SendPropertyChanged("MemoryUsage");
            }
        }
        #endregion

        #region Property TotalHdd
        private int _TotalHdd;
        public int TotalHdd
        {
            get
            {
                return _TotalHdd;
            }
            set
            {
                _TotalHdd = value;
                SendPropertyChanged("TotalHdd");
            }
        }
        #endregion

        #region Property UsedHdd
        private int _UsedHdd;
        public int UsedHdd
        {
            get
            {
                return _UsedHdd;
            }
            set
            {
                _UsedHdd = value;
                SendPropertyChanged("UsedHdd");
            }
        }
        #endregion

        #region Property TimeLables
        private List<string> _TimeLables;
        public List<string> TimeLables
        {
            get
            {
                return _TimeLables;
            }
            set
            {
                _TimeLables = value;
                SendPropertyChanged("TimeLables");
            }
        }
        #endregion

        private void RequestStaticInfoFromServer(object state)
        {
            try
            {
                CheckFixedParams();
            }
            catch
            {
            }
        }

        private void RequestInfoFromServer(object state)
        {
            try
            {
                //Logger.WriteView("RequestInfoFromServer ...");
                VoIPServiceClient_Plugin_UMSV.Default.RequestAccountsAndDialogs();
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
            finally
            {
                if (requestInfoFromServerTimer != null)
                    requestInfoFromServerTimer.Change(60 * 1000, -1);
            }
        }

        private Timer requestInfoFromServerTimer;
        private Timer requestStaticInfoFromServerTimer;
        System.Windows.Threading.Dispatcher Dispatcher;

        private void Stop()
        {
            try
            {
                requestInfoFromServerTimer.Dispose();
                requestInfoFromServerTimer = null;

                requestStaticInfoFromServerTimer.Dispose();
                requestStaticInfoFromServerTimer = null;
            }
            catch
            {
            }
        }

        private void Start()
        {
            requestInfoFromServerTimer = new Timer(RequestInfoFromServer, null, 0, -1);
            requestStaticInfoFromServerTimer = new Timer(RequestStaticInfoFromServer, null, 0, UMSV.Schema.Config.Default.RequestStaticInfoFromServerTimerInterval);

            Folder.EMQ.ClientTransport.Default.Start();
            Folder.EMQ.ClientTransport.Default.ConnectToServer();
        }

        #region IFolderForm Members

        public void Initialize(FolderFormHelper helper)
        {
            VoIPServiceClient_Plugin_UMSV.Default.OnResponseAccountsAndDialogs += Default_OnResponseAccountsAndDialogs;

            this.Dispatcher = helper.Dispatcher;

            Start();

            helper.Closing += new EventHandler<FormClosingEventArgs>(helper_Closing);
            helper.UnSelected += delegate
            {
                Stop();
            };

            helper.Selected += delegate
            {
                Start();
            };
            helper.Refresh += new EventHandler<RefreshEventArgs>(helper_Refresh);
        }

        void CheckFixedParams()
        {
            var now = new FolderDataContext().GetDate().Value;
            using (UmsDataContext dc = new UmsDataContext())
            {
                TodayCallsCount = dc.Calls.Where(c => c.CallTime > now.Date && c.Type == (int)DialogType.GatewayIncomming).Count();
                TodayDialogsCount = dc.Calls.Where(c => c.CallTime > now.Date && c.Type == (int)DialogType.ClientIncomming).Count();

                now = now.AddMinutes(-now.Minute % 10);
                var calls = dc.Calls.Where(c => c.CallTime > now.AddHours(-12) && c.Type == (int)DialogType.GatewayIncomming);
                var callsIn10Minute = (from c in calls
                                       group c by c.CallTime.Hour * 6 + c.CallTime.Minute / 10 into g
                                       select new
                                       {
                                           Time = g.Key,
                                           Count = g.Count()
                                       }).ToList();
                var list = callsIn10Minute.Select(c => new
                {
                    Time = new TimeSpan(0, c.Time * 10, 0).ToString(@"hh\:mm"),
                    Count = c.Count,
                }).ToList();

                int maxCount = list.Count > 0 ? list.Max(l => l.Count) : 1;

                var lables = new List<string>();
                this.Dispatcher.Invoke((Action)(() =>
                {
                    var points = new PointCollection();
                    for (int index = 0; index <= 60 * 12; index += 10)
                    {
                        string name = now.AddHours(-12).AddMinutes(index).ToString("HH:mm");
                        var item = list.FirstOrDefault(l => l.Time == name);

                        points.Add(new System.Windows.Point()
                        {
                            X = index,
                            Y = (maxCount - (item != null ? item.Count : 0)) * 300 / maxCount,
                        });

                        if (index % 60 == 0)
                            lables.Add(name);
                    }

                    this.CallsHistory = points;
                }));

                TimeLables = lables;
            }

            PresentationService service = PresentationService.Instantiate();
            string result = service.GetServerSystemInfo();
            SystemInfo systemInfo;
            if (result == null)
            {
                systemInfo = SystemInfo.GetSystemInfo();
            }
            else
                systemInfo = SystemInfoUtility.Deserialize<SystemInfo>(result);

            this.CpuUsage = (int)systemInfo.CpuUsagePercentage;
            this.MemoryUsage = (int)systemInfo.RAMUsage;
            this.TotalMemory = (int)systemInfo.RAM;

            var disk = systemInfo.DiskDriveInfos.First(d => d.Caption.ToUpper().Contains(UMSV.Schema.Config.Default.VoiceDirectory.First().ToString()));
            this.TotalHdd = (int)disk.TotalCapacity;
            this.UsedHdd = (int)disk.UsedSpace;
        }

        void helper_Refresh(object sender, RefreshEventArgs e)
        {
            CheckFixedParams();
            RequestInfoFromServer(null);
        }

        void Default_OnResponseAccountsAndDialogs(DateTime serverTime, List<Schema.SipAccount> accounts, List<SipDialog> dialogs)
        {
            try
            {
                var oprs = accounts.Where(a => a.MaxConcurrentCalls == 1 && a.Status != SipAccountStatus.Offline);

                OnlineOperators = oprs.Count();
                DialogsCount = oprs.Where(a => a.Status == SipAccountStatus.Talking).Count();
                CurrentCalls = dialogs.Where(d => d.DialogType == DialogType.GatewayIncomming || d.DialogType == DialogType.GatewayOutgoing).Count();

                var time = new PersianDateTime(serverTime);
                LastUpdatingTime = time.ToString();
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }

            if (requestInfoFromServerTimer != null)
                requestInfoFromServerTimer.Change(UMSV.Schema.Config.Default.OperatorsMonitoringRefreshInterval, -1);
        }

        void helper_Closing(object sender, FormClosingEventArgs e)
        {
            VoIPServiceClient_Plugin_UMSV.Default.OnResponseAccountsAndDialogs -= Default_OnResponseAccountsAndDialogs;
            Stop();
        }

        #endregion

    }
}
