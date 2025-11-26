using AttendanceManagement.Data.Employees;
using AttendanceManagement.Data.ExceptionRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace AttendanceManagement.Data.Workflows
{
    public class Workflow : FullAuditedAggregateRoot<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? EmployeeId { get; set; } // Workflow assigned per employee by admin
        public bool IsActive { get; set; }

        // Navigation properties
        public virtual Employee Employee { get; set; }
        public virtual ICollection<WorkflowStep> WorkflowSteps { get; set; }
        public virtual ICollection<ExceptionRequest> ExceptionRequests { get; set; }

        protected Workflow()
        {
            WorkflowSteps = new List<WorkflowStep>();
            ExceptionRequests = new List<ExceptionRequest>();
        }

        public Workflow(Guid id, string name, string description, Guid? employeeId = null) : base(id)
        {
            Name = name;
            Description = description;
            EmployeeId = employeeId;
            IsActive = true;
            WorkflowSteps = new List<WorkflowStep>();
            ExceptionRequests = new List<ExceptionRequest>();
        }
    }
}
