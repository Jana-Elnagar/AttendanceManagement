using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceManagement.Dtos.Schedules
{
    public class AssignScheduleDto
    {
        public Guid ScheduleId { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? GroupId { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string SeatNumber { get; set; }
        public string FloorNumber { get; set; }
    }
}
