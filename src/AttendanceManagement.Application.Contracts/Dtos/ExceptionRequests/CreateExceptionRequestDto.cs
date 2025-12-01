using AttendanceManagement.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceManagement.Dtos.ExceptionRequests
{
    public class CreateExceptionRequestDto
    {
        [Required(ErrorMessage = "Exception date is required")]
        public DateTime ExceptionDate { get; set; }

        [Required(ErrorMessage = "Reason is required")]
        [StringLength(1000, ErrorMessage = "Reason cannot exceed 1000 characters")]
        public string Reason { get; set; }

        [Required(ErrorMessage = "Exception request type is required")]
        public ExceptionRequestType Type { get; set; }
    }
}
