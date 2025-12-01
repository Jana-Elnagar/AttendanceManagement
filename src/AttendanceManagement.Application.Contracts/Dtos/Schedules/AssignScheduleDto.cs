using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceManagement.Dtos.Schedules
{
    public class AssignScheduleDto
    {
        [Required(ErrorMessage = "Schedule is required")]
        public Guid ScheduleId { get; set; }

        public Guid? EmployeeId { get; set; }

        public Guid? GroupId { get; set; }

        [Required(ErrorMessage = "Effective from date is required")]
        public DateTime EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }

        [StringLength(50, ErrorMessage = "Seat number cannot exceed 50 characters")]
        public string SeatNumber { get; set; }

        [StringLength(50, ErrorMessage = "Floor number cannot exceed 50 characters")]
        public string FloorNumber { get; set; }
    }
}
