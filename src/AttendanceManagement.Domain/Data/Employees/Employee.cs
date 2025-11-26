using AttendanceManagement.Data.Groups;
using AttendanceManagement.Data.Schedules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using AttendanceManagement.Data.ExceptionRequests;

namespace AttendanceManagement.Data.Employees
{
    public class Employee : FullAuditedAggregateRoot<Guid>
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string Sector { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
        public virtual ICollection<GroupMembership> GroupMemberships { get; set; }
        public virtual ICollection<ManagerAssignment> ManagerAssignments { get; set; }
        public virtual ICollection<ManagerAssignment> ManagedEmployees { get; set; }
        public virtual ICollection<ScheduleAssignment> ScheduleAssignments { get; set; }
        public virtual ICollection<ExceptionRequest> ExceptionRequests { get; set; }

        protected Employee()
        {
            GroupMemberships = new List<GroupMembership>();
            ManagerAssignments = new List<ManagerAssignment>();
            ManagedEmployees = new List<ManagerAssignment>();
            ScheduleAssignments = new List<ScheduleAssignment>();
            ExceptionRequests = new List<ExceptionRequest>();
        }

        public Employee(Guid id, Guid userId, string name, string department, string sector)
            : base(id)
        {
            UserId = userId;
            Name = name;
            Department = department;
            Sector = sector;
            IsActive = true;

            GroupMemberships = new List<GroupMembership>();
            ManagerAssignments = new List<ManagerAssignment>();
            ManagedEmployees = new List<ManagerAssignment>();
            ScheduleAssignments = new List<ScheduleAssignment>();
            ExceptionRequests = new List<ExceptionRequest>();
        }
    }
}
