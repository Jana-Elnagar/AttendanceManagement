using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceManagement.Dtos.Schedules
{
    public class CreateScheduleDayDto
    {
        public DayOfWeek DayOfWeek { get; set; }
        public bool IsOnSite { get; set; }
    }
}
