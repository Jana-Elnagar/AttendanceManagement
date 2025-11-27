using AttendanceManagement.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceManagement.Dtos.Workflows
{
    public class CreateWorkflowStepDto
    {
        public int StepOrder { get; set; }
        public ApproverType ApproverType { get; set; }
        public Guid? ApproverEmployeeId { get; set; }
    }
}
