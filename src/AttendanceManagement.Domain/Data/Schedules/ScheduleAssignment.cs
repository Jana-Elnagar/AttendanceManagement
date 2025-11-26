using AttendanceManagement.Data.Employees;
using AttendanceManagement.Data.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace AttendanceManagement.Data.Schedules
{
    public class ScheduleAssignment : Entity<Guid>
    {
        public Guid ScheduleId { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? GroupId { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string? SeatNumber { get; set; }
        public string? FloorNumber { get; set; }

        // Navigation properties
        public virtual Schedule Schedule { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Group Group { get; set; }

        protected ScheduleAssignment() { }

        public ScheduleAssignment(
            Guid id,
            Guid scheduleId,
            DateTime effectiveFrom,
            Guid? employeeId = null,
            Guid? groupId = null) : base(id)
        {
            ScheduleId = scheduleId;
            EmployeeId = employeeId;
            GroupId = groupId;
            EffectiveFrom = effectiveFrom;
        }
    }
}
