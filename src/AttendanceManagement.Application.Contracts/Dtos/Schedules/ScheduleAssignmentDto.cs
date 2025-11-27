using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace AttendanceManagement.Dtos.Schedules
{
    public class ScheduleAssignmentDto : EntityDto<Guid>
    {
        public Guid ScheduleId { get; set; }
        public string ScheduleName { get; set; }
        public Guid? EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public Guid? GroupId { get; set; }
        public string GroupName { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string SeatNumber { get; set; }
        public string FloorNumber { get; set; }
    }
}
