using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceManagement.Dtos.Employees
{
    public class AssignManagerDto
    {
        [Required(ErrorMessage = "Employee is required")]
        public Guid EmployeeId { get; set; }

        [Required(ErrorMessage = "Manager is required")]
        public Guid ManagerEmployeeId { get; set; }

        public bool IsPrimaryManager { get; set; }

        [Required(ErrorMessage = "Effective from date is required")]
        public DateTime EffectiveFrom { get; set; }
    }
}
