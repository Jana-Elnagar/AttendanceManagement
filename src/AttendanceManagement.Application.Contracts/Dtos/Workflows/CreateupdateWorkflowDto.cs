using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceManagement.Dtos.Workflows
{
    public class CreateUpdateWorkflowDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        public string Name { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Workflow steps are required")]
        [MinLength(1, ErrorMessage = "At least one workflow step is required")]
        public List<CreateWorkflowStepDto> WorkflowSteps { get; set; }
    }
}
