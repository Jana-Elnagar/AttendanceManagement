using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace AttendanceManagement.Data.Schedules
{
    public class Schedule : FullAuditedAggregateRoot<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
        public virtual ICollection<ScheduleDay> ScheduleDays { get; set; }
        public virtual ICollection<ScheduleAssignment> ScheduleAssignments { get; set; }

        protected Schedule()
        {
            ScheduleDays = new List<ScheduleDay>();
            ScheduleAssignments = new List<ScheduleAssignment>();
        }

        public Schedule(Guid id, string name, string description)
            : base(id)
        {
            Name = name;
            Description = description;
            IsActive = true;
            ScheduleDays = new List<ScheduleDay>();
            ScheduleAssignments = new List<ScheduleAssignment>();
        }
    }
}
