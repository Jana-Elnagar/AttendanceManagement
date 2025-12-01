using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceManagement.Dtos.Schedules
{
    public class CreateScheduleDayDto
    {
        [Required(ErrorMessage = "Day of week is required")]
        public DayOfWeek DayOfWeek { get; set; }

        public bool IsOnSite { get; set; }
    }
}
