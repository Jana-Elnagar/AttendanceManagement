using AttendanceManagement.Dtos.Employees;
using AttendanceManagement.Dtos.Schedules;
using AutoMapper;

namespace AttendanceManagement.Blazor.Client;

public class AttendanceManagementBlazorAutoMapperProfile : Profile
{
    public AttendanceManagementBlazorAutoMapperProfile()
    {
        //Define your AutoMapper configuration here for the Blazor project.
        CreateMap<EmployeeDto, CreateUpdateEmployeeDto>();
        CreateMap<ScheduleDto, CreateUpdateScheduleDto>();
        CreateMap<ScheduleDayDto, CreateScheduleDayDto>();
    }
}
