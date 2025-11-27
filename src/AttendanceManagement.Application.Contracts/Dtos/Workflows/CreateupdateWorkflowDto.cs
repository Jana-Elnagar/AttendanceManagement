using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceManagement.Dtos.Workflows
{
    public class CreateUpdateWorkflowDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<CreateWorkflowStepDto> WorkflowSteps { get; set; }
    }
}
