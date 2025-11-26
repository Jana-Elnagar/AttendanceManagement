using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace AttendanceManagement.Data.Employees
{
    public class ManagerAssignment : CreationAuditedEntity<Guid>
    {
        public Guid EmployeeId { get; set; }
        public Guid ManagerEmployeeId { get; set; }
        public bool IsPrimaryManager { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }

        // Navigation properties
        public virtual Employee Employee { get; set; }
        public virtual Employee Manager { get; set; }

        protected ManagerAssignment() { }

        public ManagerAssignment(
            Guid id,
            Guid employeeId,
            Guid managerEmployeeId,
            bool isPrimaryManager,
            DateTime effectiveFrom) : base(id)
        {
            EmployeeId = employeeId;
            ManagerEmployeeId = managerEmployeeId;
            IsPrimaryManager = isPrimaryManager;
            EffectiveFrom = effectiveFrom;
        }
    }
}
