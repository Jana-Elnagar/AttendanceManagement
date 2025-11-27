using AttendanceManagement.Dtos.Employees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceManagement.Dtos.Group
{
    public class GroupWithEmployeesDto : GroupDto
    {
        public List<EmployeeDto> Employees { get; set; }
    }
}
