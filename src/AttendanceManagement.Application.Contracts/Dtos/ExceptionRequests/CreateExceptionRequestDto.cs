using AttendanceManagement.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceManagement.Dtos.ExceptionRequests
{
    public class CreateExceptionRequestDto
    {
        public DateTime ExceptionDate { get; set; }
        public string Reason { get; set; }
        public ExceptionRequestType Type { get; set; }
    }
}
