using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceManagement.Dtos.Employees
{
    public class CreateUpdateEmployeeDto
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string Sector { get; set; }
        public Guid? GroupId { get; set; }
        public Guid? WorkflowId { get; set; }
    }

}
