using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace AttendanceManagement.Dtos.Schedules
{
    public class ScheduleDayDto : EntityDto<Guid>
    {
        public Guid ScheduleId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public bool IsOnSite { get; set; }
    }
}
