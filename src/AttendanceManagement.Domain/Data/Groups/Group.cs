using AttendanceManagement.Data.Employees;
using AttendanceManagement.Data.Schedules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace AttendanceManagement.Data.Groups
{
    public class Group : FullAuditedAggregateRoot<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
        public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<ScheduleAssignment> ScheduleAssignments { get; set; }

        protected Group()
        {
            Employees = new List<Employee>();
            ScheduleAssignments = new List<ScheduleAssignment>();
        }

        public Group(Guid id, string name, string description) : base(id)
        {
            Name = name;
            Description = description;
            IsActive = true;
            Employees = new List<Employee>();
            ScheduleAssignments = new List<ScheduleAssignment>();
        }
    }
}
