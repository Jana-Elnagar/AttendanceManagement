using AttendanceManagement.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace AttendanceManagement.Dtos.Workflows
{
    public class WorkflowStepDto : EntityDto<Guid>
    {
        public Guid WorkflowId { get; set; }
        public int StepOrder { get; set; }
        public ApproverType ApproverType { get; set; }
        public Guid? ApproverEmployeeId { get; set; }
        public string ApproverEmployeeName { get; set; }
        public bool IsActive { get; set; }
    }
}
