using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceManagement.Dtos.Schedules
{
    public class CreateUpdateScheduleDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<CreateScheduleDayDto> ScheduleDays { get; set; }
    }

}
