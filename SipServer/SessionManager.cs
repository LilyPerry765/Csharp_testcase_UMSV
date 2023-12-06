using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Enterprise;
using UMSV.Schema;

namespace UMSV
{
    public static class SessionManager
    {
        static Timer flushTimer;
        static Folder.SafeCollection<Session> Sessions = new Folder.SafeCollection<Session>();

        public static void Start()
        {
            try
            {
                using (UmsDataContext dc = new UmsDataContext())
                {
                    Sessions = new Folder.SafeCollection<Session>(dc.Sessions.Where(s => s.EndTime > DateTime.Now.AddMilliseconds(-Config.Default.SessionStatusFlushInterval).AddMilliseconds(-Config.Default.SessionTimeout) && !s.ExplicitEnd).OrderByDescending(s => s.EndTime).ToList());
                }
                flushTimer = new Timer(Flush, flushTimer, 0, Config.Default.SessionStatusFlushInterval);
                Logger.WriteInfo("SessionManager Started, sessions: {0}", Sessions.Count);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        public static void Stop()
        {
            Flush(null);
            flushTimer.Dispose();
            Logger.WriteInfo("SessionManager safe stopped");
        }

        public static void OnAccountUnRegister(SipAccount account)
        {
            try
            {
                Logger.WriteView("OnAccount UnRegister account:{0}", account.UserID);
                var session = Sessions.FirstOrDefault(s => s.SipID == account.UserID);
                if (session != null)
                {
                    using (UmsDataContext dc = new UmsDataContext())
                    {
                        dc.ExecuteCommand("update [Session] set EndTime={0}, ExplicitEnd={2} where ID={1}", DateTime.Now, session.ID, true);
                    }
                    Sessions.Remove(session);
                }
                else
                    Logger.WriteWarning("No session found for account {0} white unregister", account.UserID);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        public static void OnAccountRegister(SipAccount account)
        {
            try
            {
                //Logger.WriteView("OnAccountRegister account:{0}", account.UserID);
                var session = Sessions.FirstOrDefault(s => s.SipID == account.UserID);
                if (session == null)
                {
                    session = new Session()
                    {
                        SipID = account.UserID,
                        StartTime = DateTime.Now,
                        EndTime = DateTime.Now,
                        MachineAddress = BitConverter.ToInt32(account.SipEndPoint.Address.GetAddressBytes(), 0),
                        Type = (byte)ClientEventType.Login,
                    };
                    if (account.FolderUser != null)
                        session.UserID = account.FolderUser.ID;

                    Sessions.Add(session);
                    using (UmsDataContext dc = new UmsDataContext())
                    {
                        dc.Sessions.InsertOnSubmit(session);
                        dc.SubmitChanges();
                    }
                    Logger.WriteInfo("Session created for account {0} white register", account.UserID);
                }
                else
                    session.EndTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        public static void OnLogin(Folder.User user)
        {
            try
            {
                var session = Sessions.FirstOrDefault(s => s.UserID == user.ID && s.Type == (int)ClientEventType.SoftwareLogin);
                if (session == null)
                {
                    session = new Session()
                    {
                        UserID = user.ID,
                        SipID = user.Username,
                        StartTime = DateTime.Now,
                        EndTime = DateTime.Now,
                        MachineAddress = 0,
                        //MachineAddress = BitConverter.ToInt32(user., 0),
                        Type = (byte)ClientEventType.SoftwareLogin,
                    };

                    Sessions.Add(session);
                    using (UmsDataContext dc = new UmsDataContext())
                    {
                        dc.Sessions.InsertOnSubmit(session);
                        dc.SubmitChanges();
                    }
                    Logger.WriteInfo("Internal Session created for user {0}", user.Username);
                }
                else
                    session.EndTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        public static void OnLogOut(Folder.User user)
        {
            try
            {
                Logger.WriteView("On user logout :{0}", user.Username);
                var session = Sessions.FirstOrDefault(s => s.UserID == user.ID && s.Type == (int)ClientEventType.SoftwareLogin);
                if (session != null)
                {
                    using (UmsDataContext dc = new UmsDataContext())
                    {
                        dc.ExecuteCommand("update [Session] set EndTime={0}, ExplicitEnd={2} where ID={1}", DateTime.Now, session.ID, true);
                    }
                    Sessions.Remove(session);
                }
                else
                    Logger.WriteWarning("No internal session found for user {0} to remove", user.Username);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        private static void Flush(object state)
        {
            try
            {
                Logger.WriteView("SessionManager flush started!");
                Sessions.RemoveAll(s => DateTime.Now.Subtract(s.EndTime).TotalMilliseconds > Config.Default.SessionTimeout);

                using (UmsDataContext dc = new UmsDataContext())
                {
                    foreach (var session in Sessions)
                    {
                        dc.ExecuteCommand("update [Session] set EndTime={0} where ID={1}", session.EndTime, session.ID);
                    }
                }
                Logger.WriteDebug("SessionManager flush done, login sessions: {0}", Sessions.Count);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }
    }
}
