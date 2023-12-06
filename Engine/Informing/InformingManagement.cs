using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Enterprise;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UMSV.Schema;
using System.Timers;
using System.Net;

namespace UMSV.Engine
{
    public class InformingManagement
    {
        Timer timer;

        public InformingManagement()
        {
            timer = new Timer(1000);
            timer.Enabled = true;
            timer.Elapsed += timer_Elapsed;
        }

        private  void SetDefault()
        {
            // باز سازی وضعیت ها از حالت های (درحال تماس و برقرار و آماده سازی) به حالت منتظر
            //  بعد از اینکه فولدر سرویس ریست می شود وضعیت های 
            // (درحال تماس و برقرار و آماده سازی)
            // برای پخش وارد صف نمیشوند چون هر بار حالت های منتظر وارد صف می شوند

            using (UmsDataContext db = new UmsDataContext())
            {
                var informings = db.Informings.Where(i => i.Enabled == true).ToList();

                foreach (Informing inform in informings)
                {
                    if (inform.Schedule.CheckNow())
                    {
                        var records = db.InformingRecords.Where(i => (i.Informing == inform.ID) && (i.Status == (byte)InformingStatus.SetupProgress || i.Status == (byte)InformingStatus.InProgress || i.Status == (byte)InformingStatus.Connected)).ToList();
                        foreach (var record in records)
                        {
                            record.Status = (byte)InformingStatus.Queued;
                            record.CallCount = record.CallCount;
                            db.SubmitChanges();
                        }
                    }
                }
            }
        }

        public void Start()
        {
            SetDefault();
            timer.Start();

            Logger.WriteStart("InformingManagment started");
        }

        public void Stop()
        {
            timer.Stop();

            Logger.WriteStart("InformingManagment stoped");
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();

            List<Informing> informings = null;
            using (UmsDataContext dc = new UmsDataContext())
            {
                informings = dc.Informings.Where(i => i.Enabled).ToList();
            }

            if (informings.Count > 0)
            {
                foreach (Informing informing in informings)
                {
                    if (informing.Schedule.CheckNow())
                    {
                        SendCall(informing);
                    }
                }
            }

            timer.Start();
        }

        public void SendCall(Informing informing)
        {
            if (Config.Default.InformingConcurrentCalls <= 0)
            {
                Logger.WriteWarning("InformingManagment: InformingConcurrentCalls is 0");
                return;
            }
            else
            {
                Graph graph = null;
                List<InformingRecord> currentRecords = null;

                using (UmsDataContext dc = new UmsDataContext())
                {
                    //if (informing.Graph != null)
                    //    graph = dc.Graphs.Where(t => t.ID == informing.Graph.Value).SingleOrDefault();

                    if (informing.Graph != null)
                        graph = dc.Graphs.SingleOrDefault(t => t.ID == informing.Graph.Value);

                    int currentCallsCount = dc.InformingRecords.Count(i => i.Informing == informing.ID && (i.Status == (byte)InformingRecordStatus.SetupProgress || i.Status == (byte)InformingRecordStatus.InProgress || i.Status == (byte)InformingRecordStatus.Connected));

                    if (currentCallsCount >= Config.Default.InformingConcurrentCalls)
                        return;

                    currentRecords = dc.InformingRecords.Where(i => i.Informing == informing.ID &&
                        (
                            (i.Status == (byte)InformingRecordStatus.UnsuccessfulCall && i.CallCount < informing.RetryCount)
                                ||
                            (i.Status == (byte)InformingRecordStatus.Queued && i.CallCount < informing.RetryCount)
                            )
                            &&
                            (
                                !i.LastCallTime.HasValue || i.LastCallTime.Value < DateTime.Now.AddSeconds(-Config.Default.InformingRetryInterval)
                            )
                        )
                        .OrderBy(t => t.LastCallTime)
                        .Take(Config.Default.InformingConcurrentCalls - currentCallsCount)
                        .ToList();
                }

                foreach (InformingRecord informingRecord in currentRecords)
                {
                    DivertTarget target = null;
                    if (Config.Default.CityCode == "kermanshah")
                        target = FindProperGateway(informingRecord.Phone);
                    else
                        target = FindProperGateway(Config.Default.OutcallPrefix + informingRecord.Phone);

                    using (UmsDataContext context = new UmsDataContext())
                    {
                        InformingRecord record = context.InformingRecords.Where(i => i.Informing == informing.ID && i.ID == informingRecord.ID).SingleOrDefault();
                        record.Status = (byte)InformingRecordStatus.SetupProgress;
                        context.SubmitChanges();
                    }

                    InformingDial(informing, SipService.Default, informingRecord, graph, target);
                }
            }
        }

        private void InformingDial(Informing informing, SipService sipServer, InformingRecord informingRecord, Graph graph, DivertTarget properGateway)
        {
            if (informing.Graph == null)
            {
                Logger.WriteWarning("InformingManagment: Graph for UMSV-Informing is null .");
                return;
            }

            if (properGateway == null)
            {
                Logger.WriteWarning("InformingManagment: DivertTarget for UMSV-Informing is null .");
                return;
            }

            if (informing.Graph == null || properGateway == null)
            {
                using (UmsDataContext context = new UmsDataContext())
                {
                    InformingRecord record = context.InformingRecords.Where(i => i.Informing == informing.ID && i.ID == informingRecord.ID).FirstOrDefault();
                    if (record.Status == (byte)InformingRecordStatus.SetupProgress)
                    {
                        if (record.CallCount > 0)
                            record.Status = (byte)InformingRecordStatus.UnsuccessfulCall;
                        else
                            record.Status = (byte)InformingRecordStatus.Queued;
                    }
                    context.SubmitChanges();
                }
                return;
            }


            //5350xm supports 20 calls per second
            //we chose approximately 5 cps
            System.Threading.Thread.Sleep(200);

            SipDialog dialog = sipServer.StartIVROutCall(graph.Code, Config.Default.OutcallPrefix + informingRecord.Phone, properGateway);

            if (dialog == null)
            {
                Logger.WriteWarning("InformingManagment: Dialog for UMSV-Informing is null .");

                using (UmsDataContext context = new UmsDataContext())
                {
                    InformingRecord record = context.InformingRecords.Where(i => i.Informing == informing.ID && i.ID == informingRecord.ID).FirstOrDefault();
                    if (record.Status == (byte)InformingRecordStatus.SetupProgress)
                    {
                        if (record.CallCount > 0)
                            record.Status = (byte)InformingRecordStatus.UnsuccessfulCall;
                        else
                            record.Status = (byte)InformingRecordStatus.Queued;
                    }
                    context.SubmitChanges();
                }
                return;
            }

            dialog[Constants.DialogState_InformingRecord] = informingRecord;
            //dialog.Call.GraphID = informing.Graph;
            dialog.Call.GraphID = informing.Graph.Value;
            dialog.StatusChanged += dialog_StatusChanged;
        }

        private void dialog_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            SipDialog dialog = (SipDialog)sender;
            switch (dialog.Status)
            {
                case Schema.DialogStatus.DialingWaitForOk:  // تلفن مقصد در حال زنگ خوردن است
                    if (dialog[Constants.DialogState_InformingRecord] != null)
                    {
                        InformingRecord informingRecord = (InformingRecord)dialog[Constants.DialogState_InformingRecord];
                        using (UmsDataContext dc = new UmsDataContext())
                        {
                            var informingRec = dc.InformingRecords.FirstOrDefault(p => p.ID == informingRecord.ID);
                            informingRec.Status = (byte)InformingRecordStatus.InProgress;
                            dc.SubmitChanges();
                        }
                    }
                    break;
                case DialogStatus.Connect:// تلفن مقصد در حال جواب دادن تماس است
                    if (dialog[Constants.DialogState_InformingRecord] != null)
                    {
                        InformingRecord informingRecord = (InformingRecord)dialog[Constants.DialogState_InformingRecord];
                        using (UmsDataContext dc = new UmsDataContext())
                        {
                            var informingRec = dc.InformingRecords.FirstOrDefault(p => p.ID == informingRecord.ID);
                            informingRec.Status = (byte)InformingRecordStatus.Connected;
                            informingRec.CallCount++;
                            informingRec.LastCallTime = DateTime.Now;
                            dc.SubmitChanges();
                        }
                    }
                    break;
                case DialogStatus.Disconnected: // تلفن مقصد تماس را قطع کرد
                    if (dialog[Constants.DialogState_InformingRecord] != null)
                    {
                        InformingRecord informingRecord = (InformingRecord)dialog[Constants.DialogState_InformingRecord];
                        if (dialog.Graph == null)
                        {
                            using (UmsDataContext dc = new UmsDataContext())
                            {
                                var informingRec = dc.InformingRecords.FirstOrDefault(p => p.ID == informingRecord.ID);

                                informingRec.Status = (byte)InformingRecordStatus.UnsuccessfulCall;
                                informingRec.CallCount++;
                                informingRec.LastCallTime = DateTime.Now;
                                informingRec.LastDisconnectedTime = DateTime.Now;
                                dc.SubmitChanges();
                            }
                        }
                        else
                        {
                            using (UmsDataContext dc = new UmsDataContext())
                            {
                                var informingRec = dc.InformingRecords.FirstOrDefault(p => p.ID == informingRecord.ID);

                                if (informingRec.Status == (byte)InformingRecordStatus.Connected)
                                {
                                    informingRec.Status = (byte)InformingRecordStatus.Done;
                                    informingRec.LastDisconnectedTime = DateTime.Now;
                                    dc.SubmitChanges();
                                }
                            }

                        }
                    }
                    break;
            }
        }

        private DivertTarget FindProperGateway(string calleeId)
        {
            List<DivertTarget> targets = new List<DivertTarget>();

            foreach (var item in MediaGatewayConfig.Default.Ciscos)
            {
                var account = new SipAccount()
                {
                    UserID = item.UserID,
                    SipEndPoint = new IPEndPoint(IPAddress.Parse(item.Address), 5060),
                    Comment = string.Empty,
                    Status = SipAccountStatus.Idle,
                    RegisterTime = DateTime.Now,
                    Password = item.Password,
                    ExpireTime = DateTime.Now.AddDays(10),
                    MaxConcurrentCalls = 120,
                    DisplayName = item.UserID,
                    MatchRule = item.MatchRule,
                    LastCallTime = DateTime.Now
                };

                targets.Add(new DivertTarget(account, calleeId));
            }

            return targets.FirstOrDefault();

            /////////////////////
            //List<DivertTarget> targets = new List<DivertTarget>();




            //    foreach (SipAccount account in InformingDivertTargets)
            //    {
            //        if (Regex.IsMatch(calleeId, account.MatchRule) && (SipService.Default.Dialogs.Count(t => t.CalleeID == account.UserID) < (account.MaxConcurrentCalls - 4)))
            //            targets.Add(new DivertTarget(account, calleeId));
            //    }

            //    targets = targets.OrderBy(o => SipService.Default.Dialogs.Count(t => t.CalleeID == o.Account.UserID)).ToList();
            //    if (targets != null && targets.Count > 0)
            //        return targets.First();

            //    return null;


            //return targets.FirstOrDefault();



            //List<DivertTarget> targets = new List<DivertTarget>();
            //foreach (SipAccount account in InformingDivertTargets)
            //{
            //    if (Regex.IsMatch(calleeId, account.MatchRule) &&
            //        (SipService.Default.Dialogs.Count(t => t.CalleeID == account.UserID/*outgoing*/ || t.DialogID.EndsWith(MediaGatewayConfig.Default.Ciscos.Where(c => c.UserID == account.UserID).Single().Address)/*incoming*/) < (account.MaxConcurrentCalls - 4))
            //        )
            //        targets.Add(new DivertTarget(account, calleeId));

            //}
            //targets = targets.OrderBy
            //    (o =>
            //        SipService.Default.Dialogs.Count(t => t.CalleeID == o.Account.UserID/*outgoing*/ || t.DialogID.EndsWith(MediaGatewayConfig.Default.Ciscos.Where(c => c.UserID == o.Account.UserID).Single().Address)/*incoming*/)
            //    ).ToList();

            //if (targets != null && targets.Count > 0)
            //    return targets.First();

            //return null;
        }

        private List<SipAccount> informingDivertTargets = null;
        public List<SipAccount> InformingDivertTargets
        {
            get
            {
                if (informingDivertTargets == null)
                {
                    var registeredAccounts = SipService.Default.Accounts.Where(a => a.Status != SipAccountStatus.Offline && a.MaxConcurrentCalls > 1 && !string.IsNullOrEmpty(a.MatchRule)).ToList();
                    if (registeredAccounts != null)
                        informingDivertTargets = registeredAccounts;
                }
                return informingDivertTargets;
            }
        }
    }
}
