using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Folder;
using Enterprise;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace UMSV
{
    public class Global : PluginGlobal
    {
        bool endSessionAtBegining = false;

        public override void SessionStart()
        {
            if (UMSV.Schema.Config.Default.SingleSoftwareLogin)
            {
                Logger.WriteImportant("Plugin.UMSV session start...");
                using (UmsDataContext context = new UmsDataContext())
                {
                    Session session = context.Sessions
                                             .Where(t => t.UserID == Folder.User.Current.ID && t.SipID == Folder.User.Current.Username &&
                                                         t.Type == (int)ClientEventType.SoftwareLogin)
                                             .OrderByDescending(t => t.StartTime)
                                             .FirstOrDefault();

                    if (session != null)
                    {
                        DateTime serverTime = GetServerDate();
                        DateTime startTime = session.StartTime;
                        DateTime endTime = session.EndTime;

                        if (serverTime.Subtract(endTime).TotalMilliseconds > UMSV.Schema.Config.Default.SessionTimeout || session.ExplicitEnd)
                            SessionManager.OnLogin(Folder.User.Current);
                        else
                        {
                            endSessionAtBegining = true;
                            SessionEnd();
                        }
                    }
                    else
                        SessionManager.OnLogin(Folder.User.Current);
                }
            }
        }

        public override void SessionEnd()
        {
            if (UMSV.Schema.Config.Default.SingleSoftwareLogin)
            {
                using (UmsDataContext context = new UmsDataContext())
                {
                    Session session = context.Sessions
                                             .Where(t => t.UserID == Folder.User.Current.ID && t.SipID == Folder.User.Current.Username)
                                             .OrderByDescending(t => t.StartTime)
                                             .FirstOrDefault();

                    if (session != null && !endSessionAtBegining)
                    {
                        SessionManager.OnLogOut(Folder.User.Current);
                    }
                    else if (endSessionAtBegining)
                    {
                        System.Diagnostics.Process[] proc = System.Diagnostics.Process.GetProcessesByName("PresentationHost");
                        foreach (var p in proc)
                        {
                            p.Kill();
                        }
                    }

                    endSessionAtBegining = false;
                }
            }
        }

        private DateTime GetServerDate()
        {
            using (UmsDataContext context = new UmsDataContext())
            {
                return context.ExecuteQuery<DateTime>("SELECT GetDate()").Single();
            }
        }
    }
}

