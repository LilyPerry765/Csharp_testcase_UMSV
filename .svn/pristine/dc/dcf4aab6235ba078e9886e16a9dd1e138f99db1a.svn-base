using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Timers;
using Enterprise;

namespace SynchronizeService.Codes
{
    public class SyncTimer
    {
        Timer timer = new Timer();

        private DateTime? _startDate { get; set; }
        private DateTime? _stopDate { get; set; }

        public SyncTimer()
        {
            timer.Interval = 300000;
            timer.Elapsed += timer_Elapsed;
        }

        public void Start(DateTime? startDate = null, DateTime? stopDate = null)
        {
            try
            {
               
                _startDate = startDate;
               
                _stopDate = stopDate;
               
                if (_startDate == null && _stopDate == null)
                {
                    Logger.WriteDebug("#Start date and stop date in offline");
                    timer.Start();
                }
               
                if (_startDate != null && _stopDate != null)
                {
                   
                    using (Database.myCallCenter2 callCenter = new Database.myCallCenter2())
                    {
                      
                        while (_startDate.Value.Date != _stopDate.Value.Date)
                        {
                           
                          
                            List<Database.ChangeType> lstTypes = callCenter.ChangeTypes.ToList();
                          
                            foreach (var item in lstTypes)
                            {
                                UpdateToBeZero(item.CostTypeID, PersianDate(_startDate.Value.Date), PersianDate(_startDate.Value));
                            }

                            _startDate = _startDate.Value.Date.AddDays(1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteError(ex.ToString());
            }
        }

        public void Stop()
        {
            timer.Stop();
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Logger.WriteDebug("#Timer started");

                timer.Stop();
                using (Database.myCallCenter2 callCenter = new Database.myCallCenter2())
                {
                    DateTime date = System.DateTime.Now.Date.Subtract(new TimeSpan(1, 0, 0, 0));

                    Logger.WriteDebug("#Yesterday is {0}", date.ToString());

                    if (callCenter.ChangeLogs.FirstOrDefault(q => q.LogDate == date) == null)
                    {
                        Logger.WriteDebug("#for yesterday ,we dont have any data");
                        callCenter.ChangeTypes.ToList().ForEach(q =>
                            {
                                UpdateToBeZero(q.CostTypeID, PersianDate(date), PersianDate(date));
                            });
                    }
                    else
                    {
                        Logger.WriteDebug("#Data is exist for yesterday");
                    }
                }

                timer.Start();
            }
            catch (Exception ex)
            {
                Logger.WriteError(ex.ToString());
            }
        }

        private void UpdateToBeZero(int costTypeId, string fromDate, string toDate)
        {
            try
            {
                Logger.WriteEnd("entered UpdateToBeZero");
                using (Database.myCallCenter2 entity = new Database.myCallCenter2())
                {
                    Logger.WriteEnd("myCallCenter2" + entity.Database.Connection.ConnectionString);
                    CRM_WBS.CRM_WBS crm = new CRM_WBS.CRM_WBS();
                    Logger.WriteEnd("crm " + crm.Url);
                    Logger.WriteEnd("befor web service");
                    var fetchData = crm.Get_Changes("Admin", "alibaba123", fromDate, toDate, 0, costTypeId);
                    Logger.WriteEnd("after web service");

                    for (int i = 0; i < fetchData.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            Logger.WriteEnd(fetchData.Tables[0].Rows.Count.ToString());
                            long longWebServiceId = long.Parse(fetchData.Tables[0].Rows[i]["WebServiceId"].ToString());

                            if (entity.ChangeLogs.FirstOrDefault(current => current.WebServiceID == longWebServiceId && current.ChangeTypeID == costTypeId) == null)
                            {
                                Database.ChangeLog changeLog = new Database.ChangeLog();

                                changeLog.Address = fetchData.Tables[0].Rows[i]["Address"].ToString();
                                changeLog.CenterID = int.Parse(fetchData.Tables[0].Rows[i]["CenterID"].ToString());
                                changeLog.ChangeTypeID = costTypeId;

                                changeLog.Elka_FI_CODE = fetchData.Tables[0].Rows[i]["Elka_FI_CODE"].ToString();
                                changeLog.FirstName = fetchData.Tables[0].Rows[i]["FIRSTNAME"].ToString();

                                changeLog.IsAppied118 = false;

                                if (fetchData.Tables[0].Rows[i]["IsApplied"] != DBNull.Value && fetchData.Tables[0].Rows[i]["IsApplied"].ToString().Trim() != string.Empty)
                                {
                                    changeLog.IsApplied = Convert.ToBoolean(fetchData.Tables[0].Rows[i]["IsApplied"]);
                                }

                                if (fetchData.Tables[0].Rows[i]["IsAutomaticLog"] != DBNull.Value && fetchData.Tables[0].Rows[i]["IsAutomaticLog"].ToString().Trim() != string.Empty)
                                    changeLog.IsAutomaticLog = Convert.ToBoolean(fetchData.Tables[0].Rows[i]["IsAutomaticLog"]);

                                if (fetchData.Tables[0].Rows[i]["IsConfirmed"] != DBNull.Value && fetchData.Tables[0].Rows[i]["IsConfirmed"].ToString().Trim() != string.Empty)
                                    changeLog.IsConfirmed = Convert.ToBoolean(fetchData.Tables[0].Rows[i]["IsConfirmed"].ToString());

                                if (fetchData.Tables[0].Rows[i]["IsFoxProExport"] != DBNull.Value && fetchData.Tables[0].Rows[i]["IsFoxProExport"].ToString().Trim() != string.Empty)
                                    changeLog.IsFoxProExport = Convert.ToBoolean(fetchData.Tables[0].Rows[i]["IsFoxProExport"].ToString());

                                changeLog.LastName = fetchData.Tables[0].Rows[i]["LASTNAME"].ToString();

                                if (fetchData.Tables[0].Rows[i]["LocalCounter"] != null && fetchData.Tables[0].Rows[i]["LocalCounter"].ToString().Trim() != string.Empty)
                                    changeLog.LocalCounter = float.Parse(fetchData.Tables[0].Rows[i]["LocalCounter"].ToString());

                                if (fetchData.Tables[0].Rows[i]["LogDate"] != null && fetchData.Tables[0].Rows[i]["LogDate"].ToString().Trim() != string.Empty)
                                {
                                    string strDateTime = fetchData.Tables[0].Rows[i]["LogDate"].ToString();
                                    changeLog.LogDate = UsualDate(strDateTime);
                                }

                                if (fetchData.Tables[0].Rows[i]["ModifyDate"] != DBNull.Value && fetchData.Tables[0].Rows[i]["ModifyDate"].ToString().Trim() != string.Empty)
                                    changeLog.ModifyDate = UsualDate(fetchData.Tables[0].Rows[i]["ModifyDate"].ToString());

                                if (fetchData.Tables[0].Rows[i]["OldDomecticCounter"] != DBNull.Value && fetchData.Tables[0].Rows[i]["OldDomecticCounter"].ToString().Trim() != string.Empty)
                                    changeLog.OldDomecticCounter = int.Parse(fetchData.Tables[0].Rows[i]["OldDomecticCounter"].ToString());

                                if (fetchData.Tables[0].Rows[i]["OldInternationalCounter"] != DBNull.Value && fetchData.Tables[0].Rows[i]["OldInternationalCounter"].ToString().Trim() != string.Empty)
                                    changeLog.OldDomecticCounter = int.Parse(fetchData.Tables[0].Rows[i]["OldInternationalCounter"].ToString());

                                if (fetchData.Tables[0].Rows[i]["OldLocalCounter"] != DBNull.Value && fetchData.Tables[0].Rows[i]["OldLocalCounter"].ToString().Trim() != string.Empty)
                                    changeLog.OldDomecticCounter = int.Parse(fetchData.Tables[0].Rows[i]["OldLocalCounter"].ToString());

                                changeLog.OldPhoneNo = fetchData.Tables[0].Rows[i]["OldPhoneNo"].ToString();
                                changeLog.PhoneNo = fetchData.Tables[0].Rows[i]["PhoneNo"].ToString();
                                changeLog.PostCode = fetchData.Tables[0].Rows[i]["PostCode"].ToString();

                                if (fetchData.Tables[0].Rows[i]["SetupDate"] != DBNull.Value && fetchData.Tables[0].Rows[i]["SetupDate"].ToString().Trim() != string.Empty)
                                    changeLog.SetupDate = UsualDate(fetchData.Tables[0].Rows[i]["SetupDate"].ToString());

                                if (fetchData.Tables[0].Rows[i]["SubscriberTypeID"] != DBNull.Value && fetchData.Tables[0].Rows[i]["SubscriberTypeID"].ToString().Trim() != string.Empty)
                                    changeLog.SubscriberTypeID = int.Parse(fetchData.Tables[0].Rows[i]["SubscriberTypeID"].ToString());

                                string a;
                                if (fetchData.Tables[0].Rows[i]["VIEWIN118"] != DBNull.Value && fetchData.Tables[0].Rows[i]["VIEWIN118"].ToString().Trim() != string.Empty)
                                    changeLog.ViewIn118 = int.Parse(fetchData.Tables[0].Rows[i]["VIEWIN118"].ToString()) == 1 ? true : false;
                                else
                                    changeLog.ViewIn118 = false;

                                entity.ChangeLogs.Add(changeLog);

                                entity.SaveChanges();
                                Logger.WriteEnd("one item inserted");
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteError(ex.ToString());
            }
        }

        private string PersianDate(DateTime date)
        {
            try
            {
                PersianCalendar persian = new PersianCalendar();
                int year = persian.GetYear(date);
                int month = persian.GetMonth(date);
                int day = persian.GetDayOfMonth(date);
                return year + "/" + month.ToString().PadLeft(2, '0') + "/" + day.ToString().PadLeft(2, '0');
            }
            catch (Exception ex)
            {
                Logger.WriteError(ex.ToString());

                return string.Empty;
            }
        }

        private DateTime UsualDate(DateTime miladyDateTime)
        {
            try
            {
                System.Globalization.PersianCalendar pCalendar = new System.Globalization.PersianCalendar();
                DateTime datetime = pCalendar.AddYears(new DateTime(622, 3, 21), miladyDateTime.Year - 1);
                datetime = pCalendar.AddMonths(datetime, miladyDateTime.Month - 1);
                datetime = pCalendar.AddDays(datetime, miladyDateTime.Day - 1);

                return datetime;
            }
            catch (Exception ex)
            {
                Logger.WriteError(ex.ToString());

                return (new DateTime());
            }
        }

        private DateTime UsualDate(string miladyDateTime)
        {
            try
            {
                var varArr = miladyDateTime.Split('/');

                System.Globalization.PersianCalendar pCalendar = new System.Globalization.PersianCalendar();
                DateTime datetime = pCalendar.AddYears(new DateTime(622, 3, 21), int.Parse(varArr[0]) - 1);
                datetime = pCalendar.AddMonths(datetime, int.Parse(varArr[1]) - 1);
                datetime = pCalendar.AddDays(datetime, int.Parse(varArr[2]) - 1);

                return datetime;
            }
            catch (Exception ex)
            {
                Logger.WriteError(ex.ToString());

                return (new DateTime());
            }
        }
    }
}
