using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UMSV.Schema;
using Enterprise;


namespace UMSV.Engine
{
    internal static class NodeTimeoutManager
    {
        private static Timer timer;
        internal static Folder.SafeDictionary<SipDialog, DateTime> NodeTimeouts = new Folder.SafeDictionary<SipDialog, DateTime>();
        internal static event EventHandler<TimeoutEventArgs> NodeTimeout;

        public static void Start()
        {
            Logger.WriteInfo("Starting NodeTimeoutManager ...");
            timer = new Timer(timer_Elapsed, null, Config.Default.NodeTimeoutCheckInterval, -1);
        }

        static void timer_Elapsed(object sender)
        {
            try
            {
                var dialogs = NodeTimeouts.Keys.ToList();
                foreach (var dialog in dialogs)
                {
                    if (NodeTimeouts.ContainsKey(dialog) && NodeTimeouts[dialog] < DateTime.Now)
                    {
                        NodeTimeouts.Remove(dialog);
                        NodeTimeout(null, new TimeoutEventArgs(dialog));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex, "TimeoutManager timer_Elapsed");
            }
            finally
            {
                if (timer != null)
                    timer.Change(Config.Default.NodeTimeoutCheckInterval, -1);
            }
        }

        public static object CheckNodeForTimeoutSyncObject = new object();

        public static void CheckNodeForTimeout(SipDialog dialog)
        {
            try
            {
                lock (CheckNodeForTimeoutSyncObject)
                {
                    if (NodeTimeouts.ContainsKey(dialog))
                        NodeTimeouts.Remove(dialog);

                    if (dialog.CurrentNode is WithTimerNode && dialog.CurrentNode.AsWithTimerNode.Timeout.HasValue)
                        // if (dialog.CurrentNode is WithTimerNode && dialog.CurrentNode.Type != NodeType.Divert && dialog.CurrentNode.AsWithTimerNode.Timeout.HasValue) 
                        NodeTimeouts.Add(dialog, DateTime.Now.AddSeconds(dialog.CurrentNode.AsWithTimerNode.Timeout.Value));
                }
            }
            catch (Exception ex)
            {
                Logger.WriteWarning("CheckNodeForTimeout dialog:{0}, message:{1}", dialog.DialogID, ex.Message);
            }
        }

        public static void Stop()
        {
            Logger.WriteInfo("Stopping NodeTimeoutManager ...");
            timer.Dispose();
            timer = null;
            Logger.WriteInfo("NodeTimeoutManager Stopped.");
        }
    }
}
