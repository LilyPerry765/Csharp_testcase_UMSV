using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enterprise;
using UMSV.Schema;
using System.Text.RegularExpressions;
using Folder;
using System.Threading;

namespace UMSV.Engine
{
    public partial class VoipService
    {
        System.Threading.Timer CheckDivertQueueTimer;
        const int CheckDivertQueueInterval = 500;

        void CheckDisconnectedDialogToAccountForDivertQueue(SipDialog dialog)
        {
            //if (DivertQueue.Contains(dialog))
            //    DivertQueue.Remove(dialog);

            //if (dialog.DivertPartner != null) // RTP Proxy
            //{
            //    if (dialog.ToAccount == null) // Source
            //    {
            //        Logger.WriteInfo("Disconnecting Target Call on transfer mode, ConnectTime:{0}", dialog.DivertPartner.Call.CallTime);
            //        if (dialog.DivertPartner.Call.AnswerTime != DateTime.MinValue)
            //            SipServer.DisconnectCallOnTransferMode(dialog.DivertPartner);
            //        else
            //            SipServer.CancelDialog(dialog.DivertPartner);

            //    }
            //    else // Target
            //    {

            //        Logger.WriteInfo("Continue Source Call after end of transfering.");
            //        var sourceDialog = dialog.DivertPartner;

            //        if (sourceDialog.BeforeDivertGraph != null)
            //        {
            //            sourceDialog.Graph = sourceDialog.BeforeDivertGraph;
            //            sourceDialog.CurrentNode = sourceDialog.BeforeDivertNode;
            //        }

            //        SipServer.StopPlayVoice(sourceDialog);

            //        if (sourceDialog.CurrentNode != null && sourceDialog.CurrentNode.AsDivertNode != null)
            //        {

            //            switch ((DisconnectCause)dialog.Call.DisconnectCause)
            //            {
            //                case DisconnectCause.UserBusy:
            //                    sourceDialog.CurrentNodeID = sourceDialog.CurrentNode.AsDivertNode.BusyNode ?? sourceDialog.CurrentNode.AsDivertNode.FailureNode;
            //                    break;

            //                case DisconnectCause.UnallocatedNumber:
            //                    sourceDialog.CurrentNodeID = sourceDialog.CurrentNode.AsDivertNode.UnallocatedNumberNode ?? sourceDialog.CurrentNode.AsDivertNode.FailureNode;
            //                    break;

            //                case DisconnectCause.NoUserResponding:
            //                    sourceDialog.CurrentNodeID = sourceDialog.CurrentNode.AsDivertNode.FailureNode;
            //                    break;

            //                case DisconnectCause.SubscriberAbsent:
            //                    sourceDialog.CurrentNodeID = sourceDialog.CurrentNode.AsDivertNode.SubscriberAbsentNode ?? sourceDialog.CurrentNode.AsDivertNode.FailureNode;
            //                    break;

            //                default:
            //                    sourceDialog.CurrentNodeID = sourceDialog.CurrentNode.AsDivertNode.TargetNode;
            //                    break;
            //            }

            //            TrackCurrentNode(sourceDialog);

            //        }
            //    }
            //}
            try
            {
                if (DivertQueue.Contains(dialog))
                    DivertQueue.Remove(dialog);

                if (dialog.DivertPartner != null) // RTP Proxy
                {

                    if (dialog.ToAccount == null) // Source
                    {
                        Logger.WriteInfo("Disconnecting Target Call on transfer mode, ConnectTime:{0}", dialog.DivertPartner.Call.CallTime);
                        if (dialog.DivertPartner.Call != null && dialog.DivertPartner.Call.AnswerTime != DateTime.MinValue)
                        {
                            //طرف مقابل این دیالوگ که اپراتور است قطع میگردد
                            SipServer.DisconnectCallOnTransferMode(dialog.DivertPartner);
                            try
                            {
                                SipServer.DisconnectCall(dialog.DivertPartner);
                            }
                            catch (Exception ex3)
                            {
                                Logger.WriteCritical("CheckDisconnectedDialogToAccountForDivertQueue After disconnecting divertPartner {0}", ex3.ToString());
                            }
                            //اگر فقط یک اپراتور باقیمانده بود مشترک را دوباره به همان اپراتوری که ریجکت نموده متصل ننماید
                            if (SipServer.Accounts.Count(p => String.IsNullOrEmpty(p.MatchRule) && p.Status == SipAccountStatus.Idle && p.MaxConcurrentCalls == 1) == 1)
                            {
                                Logger.WriteImportant("CheckDisconnectedDialogToAccountForDivertQueue in Step 2");
                                try
                                {
                                    SipServer.DisconnectCall(dialog);
                                }
                                catch (Exception ex2)
                                {
                                    Logger.WriteCritical("CheckDisconnectedDialogToAccountForDivertQueue after disconnectin in Step 2 {0}", ex2.ToString());
                                }
                                return;
                            }

                            if (!string.IsNullOrEmpty(dialog.ToAccount.MatchRule) && dialog.ToAccount.MaxConcurrentCalls > 1)
                            {

                                Logger.WriteImportant("CheckDisconnectedDialogToAccountForDivertQueue in Step 3");
                                SipServer.DisconnectCall(dialog);

                                #region Partner Management
                                /*
                                Logger.WriteInfo("Continue Source Call after end of transfering.");

                                var sourceDialog = dialog.DivertPartner;

                                if (sourceDialog.BeforeDivertGraph != null)
                                {
                                    sourceDialog.Graph = sourceDialog.BeforeDivertGraph;
                                    sourceDialog.CurrentNode = sourceDialog.BeforeDivertNode;
                                }

                                SipServer.StopPlayVoice(sourceDialog);

                                if (sourceDialog.CurrentNode != null && sourceDialog.CurrentNode.AsDivertNode != null)
                                {

                                    switch ((DisconnectCause)dialog.Call.DisconnectCause)
                                    {
                                        case DisconnectCause.UserBusy:
                                            sourceDialog.CurrentNodeID = sourceDialog.CurrentNode.AsDivertNode.BusyNode ?? sourceDialog.CurrentNode.AsDivertNode.FailureNode;
                                            break;

                                        case DisconnectCause.UnallocatedNumber:
                                            sourceDialog.CurrentNodeID = sourceDialog.CurrentNode.AsDivertNode.UnallocatedNumberNode ?? sourceDialog.CurrentNode.AsDivertNode.FailureNode;
                                            break;

                                        case DisconnectCause.NoUserResponding:
                                            sourceDialog.CurrentNodeID = sourceDialog.CurrentNode.AsDivertNode.FailureNode;
                                            break;

                                        case DisconnectCause.SubscriberAbsent:
                                            sourceDialog.CurrentNodeID = sourceDialog.CurrentNode.AsDivertNode.SubscriberAbsentNode ?? sourceDialog.CurrentNode.AsDivertNode.FailureNode;
                                            break;

                                        default:
                                            sourceDialog.CurrentNodeID = sourceDialog.CurrentNode.AsDivertNode.TargetNode;
                                            break;
                                    }

                                    TrackCurrentNode(sourceDialog);

                                }
                                 */
                                #endregion

                                return;
                            }
                            else
                            {
                                Logger.WriteImportant("CheckDisconnectedDialogToAccountForDivertQueue in Step 4");
                                SipServer.DisconnectCall(dialog);
                                return;
                            }


                            //
                            DivertQueue.Insert(0, dialog);
                            dialog.Status = DialogStatus.WaitForDiverting;
                            dialog.RtpNet.Stop();
                        }
                        else
                            SipServer.CancelDialog(dialog.DivertPartner);
                    }
                    else // Target
                    {
                        Logger.WriteInfo("Continue Source Call after end of transfering.");
                        var sourceDialog = dialog.DivertPartner;

                        if (sourceDialog.BeforeDivertGraph != null)
                        {
                            sourceDialog.Graph = sourceDialog.BeforeDivertGraph;
                            sourceDialog.CurrentNode = sourceDialog.BeforeDivertNode;
                        }

                        SipServer.StopPlayVoice(sourceDialog);

                        if (sourceDialog.CurrentNode != null && sourceDialog.CurrentNode.AsDivertNode != null)
                        {
                            switch ((DisconnectCause)dialog.Call.DisconnectCause)
                            {
                                case DisconnectCause.UserBusy:
                                    sourceDialog.CurrentNodeID = sourceDialog.CurrentNode.AsDivertNode.BusyNode ?? sourceDialog.CurrentNode.AsDivertNode.FailureNode;
                                    break;

                                case DisconnectCause.UnallocatedNumber:
                                    sourceDialog.CurrentNodeID = sourceDialog.CurrentNode.AsDivertNode.UnallocatedNumberNode ?? sourceDialog.CurrentNode.AsDivertNode.FailureNode;
                                    break;

                                case DisconnectCause.NoUserResponding:
                                    sourceDialog.CurrentNodeID = sourceDialog.CurrentNode.AsDivertNode.FailureNode;
                                    break;

                                case DisconnectCause.SubscriberAbsent:
                                    sourceDialog.CurrentNodeID = sourceDialog.CurrentNode.AsDivertNode.SubscriberAbsentNode ?? sourceDialog.CurrentNode.AsDivertNode.FailureNode;
                                    break;

                                default:
                                    sourceDialog.CurrentNodeID = sourceDialog.CurrentNode.AsDivertNode.TargetNode;
                                    break;
                            }
                            TrackCurrentNode(sourceDialog);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(LogType.Critical, ex.ToString());
            }
        }

        void SipServer_TransferFailed(object sender, TransferFailedEventArgs e)
        {
            Logger.WriteInfo("SipServer_TransferFailed.");
            //CheckDisconnectedDialogToAccountForDivertQueue(e.Dialog);

            if (e.Dialog.DivertPartner == null)
            {
                Logger.WriteWarning("SipServer_TransferFailed when e.Dialog.DivertPartner is null, DialogID: {0}", e.Dialog.DialogID);
                return;
            }

            switch (e.Dialog.DivertPartner.DisconnectCause)
            {
                case DisconnectCause.NoUserResponding:
                case DisconnectCause.SubscriberAbsent:
                case DisconnectCause.UnallocatedNumber:
                case DisconnectCause.UnknownProblem:
                    CheckDisconnectedDialogToAccountForDivertQueue(e.Dialog);
                    return;

                case DisconnectCause.UserBusy:
                    CheckDisconnectedDialogToAccountForDivertQueue(e.Dialog);
                    //TrackDivertNode(e.Dialog, true);
                    break;

                default:
                    TrackDivertNode(e.Dialog, true);
                    break;
            }
        }

        IEnumerable<DivertTarget> ReadyToDivertTargets(IEnumerable<DivertTarget> divertTargets)
        {
            return divertTargets.Where(t =>
                        t.Account.MaxConcurrentCalls > 1 ||
                        (
                            t.Account.Status == SipAccountStatus.Idle &&
                            t.Account.RegisterTime.HasValue &&
                            DateTime.Now.Subtract(t.Account.RegisterTime.Value).TotalMilliseconds > Config.Default.OperatorDivertInterval) &&
                            DateTime.Now.Subtract(t.Account.LastCallTime).TotalMilliseconds > Config.Default.OperatorDivertInterval &&
                            (!t.Account.LastCallEndedTime.HasValue ||
                                DateTime.Now.Subtract(t.Account.LastCallEndedTime.Value).TotalMilliseconds > Config.Default.OperatorDivertInterval))
                        .OrderBy(t => t.Account.LastCallEndedTime);
        }

        void CheckDivertQueueForNewUser(SipAccount account)
        {
            using (FolderDataContext dc = new FolderDataContext())
            {
                var teams = (from ur in dc.UserRoles
                             join u in dc.Users on ur.UserID equals u.ID
                             where u.Username == account.UserID && ur.Role.ParentID == Constants.TeamsRole
                             select ur.RoleID.ToString().ToUpper()).ToList();

                var queues = DivertQueue.Where(q => !string.IsNullOrEmpty(q.DivertTargetTeam) &&
                    teams.Contains(q.DivertTargetTeam.ToUpper()));
                foreach (var dialog in queues)
                {
                    dialog.DivertTargets = new SafeCollection<DivertTarget>(
                        ExtractDivertTargetAccounts(dialog, dialog.BeforeDivertNode.AsDivertNode));
                }
            }

            CheckDivertQueue(null);
        }

        void CheckDivertQueue(object state)
        {
            try
            {
                foreach (var dialog in DivertQueue)
                {
                    if (dialog.Status == DialogStatus.DivertingWaitForTargetResponse)
                        continue;

                    var divertTarget = ReadyToDivertTargets(dialog.DivertTargets).FirstOrDefault();
                    if (divertTarget != null)
                    {
                        Logger.Write("DivertQueue", "Account '{0}' selected. LastCallTime: '{1}'. LastCallEndedTime: '{2}'",
                            divertTarget.Account.UserID, divertTarget.Account.LastCallTime, divertTarget.Account.LastCallEndedTime);

                        divertTarget.Account.LastCallTime = DateTime.Now;
                        DivertQueue.Remove(dialog);
                        dialog.Status = DialogStatus.DivertingWaitForTargetResponse;

                        if (dialog.BeforeDivertGraph != null)
                            dialog.Graph = dialog.BeforeDivertGraph;

                        if (dialog.BeforeDivertNode != null)
                            dialog.CurrentNode = dialog.BeforeDivertNode;
                        SipServer.PlayVoice(dialog, RingingVoice);
                        SipServer.TransferCall(dialog, divertTarget, dialog.CurrentNode.AsDivertNode.ProxyMode,
                            dialog.CurrentNode.AsDivertNode.RecordVoice);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
            finally
            {
                if (CheckDivertQueueTimer != null)
                    CheckDivertQueueTimer.Change(CheckDivertQueueInterval, -1);
            }
        }

        void TrackDivertNode(SipDialog dialog, bool insertInQueueHead = false)
        {
            var divertNode = dialog.CurrentNode.AsDivertNode;

            if (!string.IsNullOrWhiteSpace(divertNode.TargetTeam))
            {
                using (FolderDataContext dc = new FolderDataContext())
                {
                    Guid targetTeam = Guid.Parse(divertNode.TargetTeam);
                    divertNode.DynamicTargetPhone = string.Join(",", (from ur in dc.UserRoles
                                                                      where ur.RoleID == targetTeam
                                                                      join u in dc.Users on ur.UserID equals u.ID
                                                                      select u.Username).ToArray());
                    dialog.DivertTargetTeam = divertNode.TargetTeam;
                }
            }
            else
                divertNode.DynamicTargetPhone = divertNode.TargetPhone;

            if (string.IsNullOrWhiteSpace(divertNode.DynamicTargetPhone))
            {
                Logger.WriteWarning("No Target phone for Divert Node is Specified.");
                dialog.CurrentNode = dialog.Graph.FindNodeById(divertNode.FailureNode);
                TrackCurrentNode(dialog);
                return;
            }

            if (Regex.IsMatch(divertNode.TargetPhone, @"\[Extension\]"))
                divertNode.TargetPhone = Regex.Replace(divertNode.TargetPhone, @"\[Extension\]", dialog.Extension.Replace("#", "").Replace("*", ""));

            if (Regex.IsMatch(divertNode.TargetPhone, @"\[CalleeID\]"))
                divertNode.DynamicTargetPhone = Regex.Replace(divertNode.TargetPhone, @"\[CalleeID\]", dialog.CalleeID);

            #region Process CallerID
            if (divertNode.ClearAllSource)
                dialog.CallerID = string.Empty;

            if (divertNode.CallerDeleteFromStart > 0 && dialog.CallerID.Length > divertNode.CallerDeleteFromStart)
                dialog.CallerID = dialog.CallerID.Remove(0, divertNode.CallerDeleteFromStart);

            if (divertNode.CallerDeleteFromEnd > 0 && dialog.CallerID.Length > divertNode.CallerDeleteFromStart)
                dialog.CallerID = dialog.CallerID.Remove(divertNode.CallerDeleteFromEnd, dialog.CallerID.Length - divertNode.CallerDeleteFromEnd);

            if (!String.IsNullOrEmpty(divertNode.CallerPrefix))
                dialog.CallerID = dialog.CallerID.Insert(0, divertNode.CallerPrefix);

            if (!String.IsNullOrEmpty(divertNode.CallerPostfix))
                dialog.CallerID = dialog.CallerID.Insert(dialog.CallerID.Length, divertNode.CallerPostfix);
            #endregion

            #region Process CalleeID
            divertNode.DynamicTargetPhone =
                divertNode.DynamicTargetPhone.Remove(0, Math.Min(divertNode.CalleeDeleteFromStart, divertNode.DynamicTargetPhone.Length));

            if (divertNode.DynamicTargetPhone.Length > divertNode.CalleeDeleteFromEnd)
                divertNode.DynamicTargetPhone = divertNode.DynamicTargetPhone.Substring(0, divertNode.DynamicTargetPhone.Length - divertNode.CalleeDeleteFromEnd);

            divertNode.DynamicTargetPhone = divertNode.DynamicTargetPhone.Insert(0, divertNode.CalleePrefix ?? string.Empty);
            divertNode.DynamicTargetPhone += divertNode.CalleePostfix ?? string.Empty;
            #endregion

            var targets = ExtractDivertTargetAccounts(dialog, divertNode);
            if (targets == null)
                return;

            dialog.Status = DialogStatus.WaitForDiverting;
            dialog.WaitForDivertingTimeout = divertNode.Timeout;

            if (DivertQueue.Count == 0)
            {
                var divertTarget = ReadyToDivertTargets(targets).FirstOrDefault();
                if (divertTarget != null)
                {
                    Logger.Write("DivertQueue", "Account '{0}' selected immediately. LastCallTime: '{1}'. LastCallEndedTime: '{2}'",
                        divertTarget.Account.UserID, divertTarget.Account.LastCallTime, divertTarget.Account.LastCallEndedTime);

                    divertTarget.Account.LastCallTime = DateTime.Now;
                    SipServer.PlayVoice(dialog, RingingVoice);
                    SipServer.TransferCall(dialog, divertTarget, divertNode.ProxyMode, divertNode.RecordVoice);
                    dialog.Status = DialogStatus.DivertingWaitForTargetResponse;

                    //if (divertNode.MaxTalkTime > 0)
                    //    NodeTimeoutManager.NodeTimeouts.Add(dialog, DateTime.Now.AddSeconds(divertNode.MaxTalkTime));
                    return;
                }
            }

            #region Check Queue Is Busy
            if (!string.IsNullOrEmpty(divertNode.BusyNode))
            {
                dialog.Extension = "BusyQueue";
                dialog.CurrentNode = dialog.Graph.FindNodeById(divertNode.BusyNode);
                if (dialog.CurrentNode != null)
                {
                    Logger.WriteInfo("Target is busy, and busy target node is available: {0}", dialog.CurrentNode.ID);
                    TrackCurrentNode(dialog);
                    return;
                }
            }
            #endregion

            #region Check Queue Is Full
            if (!string.IsNullOrEmpty(divertNode.FullQueueNode))
            {
                if ((divertNode.QueueSize != 0 && DivertQueue.Count >= divertNode.QueueSize) ||
                    (divertNode.QueueSizePerOnlineUsers != 0 && DivertQueue.Count >= targets.Count(t => t.Account.Status != SipAccountStatus.Offline) * divertNode.QueueSizePerOnlineUsers))
                {
                    dialog.CurrentNode = dialog.Graph.FindNodeById(divertNode.FullQueueNode);
                    dialog.Extension = "FullQueue";
                    if (dialog.CurrentNode != null)
                    {
                        Logger.WriteInfo("Queue is full, current size: {0}, target node: {1}", DivertQueue.Count, dialog.CurrentNode.ID);
                        TrackCurrentNode(dialog);
                        return;
                    }
                }
            }
            #endregion

            if (string.IsNullOrWhiteSpace(Config.Default.DivertWaitGraph))
            {
                VoiceStream voiceStram = new VoiceStream();
                if (!string.IsNullOrEmpty(divertNode.AsDivertNode.WaitMessage))
                    voiceStram.AddVoice(new Guid(divertNode.AsDivertNode.WaitMessage));

                if (!string.IsNullOrEmpty(divertNode.AsDivertNode.WaitSound))
                    voiceStram.AddVoice(new Guid(divertNode.AsDivertNode.WaitSound));
                else
                    voiceStram.AddVoice(Constants.VoiceName_WaitMusic);

                SipServer.PlayVoice(dialog, voiceStram.stream.ToArray());
            }
            else
            {
                using (UmsDataContext dc = new UmsDataContext())
                {
                    var graphRecord = dc.Graphs.FirstOrDefault(g => g.ID == new Guid(Config.Default.DivertWaitGraph));
                    if (graphRecord == null)
                    {
                        Logger.WriteCritical("DivertWaitGraph ID:'{0}' not found", Config.Default.DivertWaitGraph);
                        return;
                    }

                    var graphXml = graphRecord.Data.ToString();
                    var graph = Schema.Graph.Deserialize(graphXml);

                    dialog.BeforeDivertGraph = dialog.Graph;
                    dialog.BeforeDivertNode = dialog.CurrentNode;

                    dialog.Graph = graph;
                    dialog.CurrentNodeID = graph.StartNode;
                    TrackCurrentNode(dialog);
                }
            }

            if (insertInQueueHead)
                DivertQueue.Insert(0, dialog);
            else
                DivertQueue.Add(dialog);
            CheckDivertQueue(null);
        }

        List<DivertTarget> ExtractDivertTargetAccounts(SipDialog dialog, DivertNode divertNode)
        {
            var splittedPhones = divertNode.DynamicTargetPhone.Split(',');
            List<string> phones = new List<string>();
            foreach (var phone in splittedPhones)
            {
                if (Regex.IsMatch(phone, @"\d+-\d+"))
                {
                    int start = int.Parse(phone.Split('-').First());
                    int end = int.Parse(phone.Split('-').Last());

                    if (end >= start)
                        for (int index = start; index <= end; index++)
                            phones.Add(index.ToString());
                    else
                        for (int index = start; index >= end; index--)
                            phones.Add(index.ToString());
                }
                else
                {
                    phones.Add(phone);
                }
            }

            var registeredAccounts = SipServer.Accounts.Where(a => a.Status != SipAccountStatus.Offline);
            List<DivertTarget> divertTargets = new List<DivertTarget>();

            foreach (var phone in phones.Distinct())
            {
                foreach (var account in registeredAccounts)
                {
                    if (phone == account.UserID || (!String.IsNullOrEmpty(account.MatchRule) && Regex.IsMatch(phone, account.MatchRule)))
                        divertTargets.Add(new DivertTarget(account, phone));
                }
            }

            if (divertTargets.Count() == 0)
            {
                Logger.WriteInfo("({0}:{1})->No online account found for diverting, Target phone:{2}", dialog.CalleeID, dialog.CallerID, divertNode.DynamicTargetPhone);
                dialog.CurrentNode = dialog.Graph.FindNodeById(divertNode.FailureNode);
                TrackCurrentNode(dialog);
                return null;
            }

            divertTargets = divertTargets.OrderBy(a => a.Account.LastCallTime).ToList();
            dialog.DivertTargets = new SafeCollection<DivertTarget>(divertTargets);

            return divertTargets.ToList();
        }

        SafeCollection<SipDialog> DivertQueue = new SafeCollection<SipDialog>();
    }
}
