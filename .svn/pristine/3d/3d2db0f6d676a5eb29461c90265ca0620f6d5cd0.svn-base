using UMSV.Schema;
namespace Plugin.Mailbox.Model
{
    partial class Mailbox
    {
        private Schedule schedule;
        public Schedule Schedule
        {
            get
            {
                if (schedule == null)
                {
                    if (PagingSchedule == null)
                    {
                        schedule = new Schedule();
                        var st = new ScheduleTime();
                        st.Start = "08:00";
                        st.Finish = "22:00";
                        schedule.Times.Add(st);
                    }
                    else
                        schedule = ScheduleUtility.Deserialize<Schedule>(PagingSchedule.ToString());
                }
                return schedule;
            }
        }

    }

}
