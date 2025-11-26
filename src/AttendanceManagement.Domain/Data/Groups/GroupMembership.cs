using AttendanceManagement.Data.Employees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace AttendanceManagement.Data.Groups
{
    public class GroupMembership : CreationAuditedEntity<Guid>
    {
        public Guid GroupId { get; set; }
        public Guid EmployeeId { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
        public virtual Group Group { get; set; }
        public virtual Employee Employee { get; set; }

        protected GroupMembership() { }

        public GroupMembership(Guid id, Guid groupId, Guid employeeId) : base(id)
        {
            GroupId = groupId;
            EmployeeId = employeeId;
            IsActive = true;
        }
    }
}
