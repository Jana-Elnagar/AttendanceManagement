using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using AttendanceManagement.Data.Schedules;

namespace AttendanceManagement.Data.Groups
{
    public class Group : FullAuditedAggregateRoot<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
        public virtual ICollection<GroupMembership> GroupMemberships { get; set; }
        public virtual ICollection<ScheduleAssignment> ScheduleAssignments { get; set; }

        protected Group()
        {
            GroupMemberships = new List<GroupMembership>();
            ScheduleAssignments = new List<ScheduleAssignment>();
        }

        public Group(Guid id, string name, string description) : base(id)
        {
            Name = name;
            Description = description;
            IsActive = true;
            GroupMemberships = new List<GroupMembership>();
            ScheduleAssignments = new List<ScheduleAssignment>();
        }
    }
}
