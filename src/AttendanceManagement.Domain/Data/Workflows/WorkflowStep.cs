using AttendanceManagement.Data.Employees;
using AttendanceManagement.Data.ExceptionRequests;
using AttendanceManagement.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace AttendanceManagement.Data.Workflows
{
    public class WorkflowStep : Entity<Guid>
    {
        public Guid WorkflowId { get; set; }
        public int StepOrder { get; set; }
        public ApproverType ApproverType { get; set; }
        public Guid? ApproverEmployeeId { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
        public virtual Workflow Workflow { get; set; }
        public virtual Employee ApproverEmployee { get; set; }
        public virtual ICollection<ExceptionRequestApprovalHistory> ApprovalHistories { get; set; }

        protected WorkflowStep()
        {
            ApprovalHistories = new List<ExceptionRequestApprovalHistory>();
        }

        public WorkflowStep(
            Guid id,
            Guid workflowId,
            int stepOrder,
            ApproverType approverType,
            Guid? approverEmployeeId = null) : base(id)
        {
            WorkflowId = workflowId;
            StepOrder = stepOrder;
            ApproverType = approverType;
            ApproverEmployeeId = approverEmployeeId;
            IsActive = true;
            ApprovalHistories = new List<ExceptionRequestApprovalHistory>();
        }
    }
}
