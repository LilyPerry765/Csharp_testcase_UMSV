using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using Folder;
using System;
using Enterprise;

namespace UMSV.Schema
{
    public partial class Schedule
    {
        public bool CheckNow()
        {
            if (DateTime.Now.TimeOfDay > TimeSpan.Parse("22:00") || DateTime.Now.TimeOfDay < TimeSpan.Parse("7:00"))
                return false;

            foreach (ScheduleTime time in Times)
            {
                try
                {
                    if (time.Date.Value.Year == DateTime.Now.Date.Year && time.Date.Value.Month == DateTime.Now.Date.Month && time.Date.Value.Day == DateTime.Now.Date.Day)
                    {
                        if ((!String.IsNullOrEmpty(time.Start) && TimeSpan.Parse(time.Start) <= DateTime.Now.TimeOfDay) &&
                            (!String.IsNullOrEmpty(time.Finish) && TimeSpan.Parse(time.Finish) > DateTime.Now.TimeOfDay))
                            return true;
                    }

                    if (time.Date.Value == DateTime.Now.Date || time.Date.Value.ToString() == "1/1/0001 12:00:00 AM")
                    {
                        if ((!String.IsNullOrEmpty(time.Start) && TimeSpan.Parse(time.Start) <= DateTime.Now.TimeOfDay) &&
                            (!String.IsNullOrEmpty(time.Finish) && TimeSpan.Parse(time.Finish) > DateTime.Now.TimeOfDay))
                            return true;
                    }
                }
                catch
                {
                    if ((!String.IsNullOrEmpty(time.Start) && TimeSpan.Parse(time.Start) <= DateTime.Now.TimeOfDay) &&
                        (!String.IsNullOrEmpty(time.Finish) && TimeSpan.Parse(time.Finish) > DateTime.Now.TimeOfDay))
                        return true;
                }

            }

            return false;
        }

        public XElement ToXElement()
        {
            return System.Xml.Linq.XElement.Load(new StringReader(ScheduleUtility.SerializeInforming<Schedule>(this)));
        }

        public static Schedule FromXElement(XElement scheduleElement)
        {
            return ScheduleUtility.DeserializeInforming<Schedule>(scheduleElement.ToString());
        }

        [XmlIgnore()]
        public ListWrapper<ScheduleTime> ObservableTimes
        {
            get
            {
                return new ListWrapper<ScheduleTime>(Times);
            }
        }
    }
}
