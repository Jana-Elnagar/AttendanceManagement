using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceManagement.Dtos.Employees
{
    public class CreateUpdateEmployeeDto
    {
        [Required(ErrorMessage = "User is required")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        public string Name { get; set; }

        [StringLength(100, ErrorMessage = "Department cannot exceed 100 characters")]
        public string Department { get; set; }

        [StringLength(100, ErrorMessage = "Sector cannot exceed 100 characters")]
        public string Sector { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(256, ErrorMessage = "Email cannot exceed 256 characters")]
        public string Email { get; set; }

        [StringLength(100, ErrorMessage = "Company ID cannot exceed 100 characters")]
        public string CompanyId { get; set; }

        public Guid? GroupId { get; set; }

        public Guid? WorkflowId { get; set; }
    }

}
