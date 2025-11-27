using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceManagement.Dtos.Employees
{
    public class AssignManagerDto
    {
        public Guid EmployeeId { get; set; }
        public Guid ManagerEmployeeId { get; set; }
        public bool IsPrimaryManager { get; set; }
        public DateTime EffectiveFrom { get; set; }
    }
}
