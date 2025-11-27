using AttendanceManagement.Dtos.Schedules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceManagement.Dtos.Employees
{
    public class EmployeeWithDetailsDto : EmployeeDto
    {
        public ScheduleDto CurrentSchedule { get; set; }
        public List<ScheduleDayDto> ScheduleDays { get; set; }
        public string PrimaryManagerName { get; set; }
        public string SeatNumber { get; set; }
        public string FloorNumber { get; set; }
    }
}
