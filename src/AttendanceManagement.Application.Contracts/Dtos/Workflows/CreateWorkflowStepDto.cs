using AttendanceManagement.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceManagement.Dtos.Workflows
{
    public class CreateWorkflowStepDto
    {
        [Required(ErrorMessage = "Step order is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Step order must be greater than 0")]
        public int StepOrder { get; set; }

        [Required(ErrorMessage = "Approver type is required")]
        public ApproverType ApproverType { get; set; }

        public Guid? ApproverEmployeeId { get; set; }
    }
}
