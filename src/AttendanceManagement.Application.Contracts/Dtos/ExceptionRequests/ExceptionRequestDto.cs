using AttendanceManagement.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace AttendanceManagement.Dtos.ExceptionRequests
{
    public class ExceptionRequestDto : FullAuditedEntityDto<Guid>
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime ExceptionDate { get; set; }
        public string Reason { get; set; }
        public ExceptionRequestType Type { get; set; }
        public ExceptionRequestStatus Status { get; set; }
        public Guid WorkflowId { get; set; }
        public string WorkflowName { get; set; }
        public int CurrentStepOrder { get; set; }
        public List<ExceptionRequestApprovalHistoryDto> ApprovalHistories { get; set; }
        public List<ExceptionRequestAttachmentDto> Attachments { get; set; }
    }
}
