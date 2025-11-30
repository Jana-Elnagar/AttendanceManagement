using AttendanceManagement.Data.Employees;
using AttendanceManagement.Data.Workflows;
using AttendanceManagement.Data.Notifications;
using AttendanceManagement.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace AttendanceManagement.Data.ExceptionRequests
{
    public class ExceptionRequest : FullAuditedAggregateRoot<Guid>
    {
        public Guid EmployeeId { get; set; }
        public DateTime ExceptionDate { get; set; }
        public string Reason { get; set; }
        public ExceptionRequestType Type { get; set; }
        public ExceptionRequestStatus Status { get; set; }
        public Guid WorkflowId { get; set; }
        public int CurrentStepOrder { get; set; }

        // Navigation properties
        public virtual Employee Employee { get; set; }
        public virtual Workflow Workflow { get; set; }
        public virtual ICollection<ExceptionRequestApprovalHistory> ApprovalHistories { get; set; }
        public virtual ICollection<ExceptionRequestAttachment> Attachments { get; set; }
        //public virtual ICollection<Notification> Notifications { get; set; }

        protected ExceptionRequest()
        {
            ApprovalHistories = new List<ExceptionRequestApprovalHistory>();
            Attachments = new List<ExceptionRequestAttachment>();
            //Notifications = new List<Notification>();
        }

        public ExceptionRequest(
            Guid id,
            Guid employeeId,
            DateTime exceptionDate,
            string reason,
            ExceptionRequestType type,
            Guid workflowId) : base(id)
        {
            EmployeeId = employeeId;
            ExceptionDate = exceptionDate;
            Reason = reason;
            Type = type;
            WorkflowId = workflowId;
            Status = ExceptionRequestStatus.Pending;
            CurrentStepOrder = 1;

            ApprovalHistories = new List<ExceptionRequestApprovalHistory>();
            Attachments = new List<ExceptionRequestAttachment>();
            //Notifications = new List<Notification>();
        }

        public void Approve(int stepOrder)
        {
            CurrentStepOrder = stepOrder + 1;
        }

        public void FinalApprove()
        {
            Status = ExceptionRequestStatus.Approved;
        }

        public void Reject()
        {
            Status = ExceptionRequestStatus.Rejected;
        }
    }
}
