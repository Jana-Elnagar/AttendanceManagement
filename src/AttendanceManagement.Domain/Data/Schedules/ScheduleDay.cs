using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace AttendanceManagement.Data.Schedules
{
    public class ScheduleDay : Entity<Guid>
    {
        public Guid ScheduleId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public bool IsOnSite { get; set; }

        // Navigation properties
        public virtual Schedule Schedule { get; set; }

        protected ScheduleDay() { }

        public ScheduleDay(Guid id, Guid scheduleId, DayOfWeek dayOfWeek, bool isOnSite)
            : base(id)
        {
            ScheduleId = scheduleId;
            DayOfWeek = dayOfWeek;
            IsOnSite = isOnSite;
        }
    }
}
