using AttendanceManagement.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceManagement.Dtos.ExceptionRequests
{
    public class ApproveRejectExceptionRequestDto
    {
        public Guid ExceptionRequestId { get; set; }
        public ApprovalAction Action { get; set; }
        public string Notes { get; set; }
    }
}
