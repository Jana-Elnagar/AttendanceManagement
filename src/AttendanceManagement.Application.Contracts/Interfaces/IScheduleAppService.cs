using AttendanceManagement.Dtos.Schedules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace AttendanceManagement.Interfaces
{
    public interface IScheduleAppService :
        ICrudAppService<ScheduleDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateScheduleDto>
    {
        Task<List<ScheduleDto>> GetActiveSchedulesAsync();
        Task ActivateAsync(Guid id);
        Task DeactivateAsync(Guid id);
        Task AssignScheduleAsync(AssignScheduleDto input);
        Task<ScheduleAssignmentDto> GetEmployeeCurrentScheduleAsync(Guid employeeId);
        Task<byte[]> ExportEmployeeScheduleAsync(Guid employeeId);
    }
}
