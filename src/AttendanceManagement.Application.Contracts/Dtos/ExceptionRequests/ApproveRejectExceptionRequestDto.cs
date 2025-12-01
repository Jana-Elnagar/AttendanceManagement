using AttendanceManagement.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceManagement.Dtos.ExceptionRequests
{
    public class ApproveRejectExceptionRequestDto
    {
        [Required(ErrorMessage = "Exception request is required")]
        public Guid ExceptionRequestId { get; set; }

        [Required(ErrorMessage = "Action is required")]
        public ApprovalAction Action { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string Notes { get; set; }
    }
}
