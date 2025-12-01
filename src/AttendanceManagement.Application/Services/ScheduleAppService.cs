using AttendanceManagement.Data.Employees;
using AttendanceManagement.Data.Groups;
using AttendanceManagement.Data.Schedules;
using AttendanceManagement.Dtos.Schedules;
using AttendanceManagement.Interfaces;
using AttendanceManagement.Permissions;
using AutoMapper.Internal.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace AttendanceManagement.Services
{
    public class ScheduleAppService :
         CrudAppService<Schedule, ScheduleDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateScheduleDto>,
         IScheduleAppService
    {
        private readonly IRepository<Employee, Guid> _employeeRepository;
        private readonly IRepository<Group, Guid> _groupRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly ILogger<EmployeeAppService> _logger;

        public ScheduleAppService(
            ILogger<EmployeeAppService> logger,
            IRepository<Schedule, Guid> repository,
            IRepository<Employee, Guid> employeeRepository,
            IRepository<Group, Guid> groupRepository,
            IScheduleRepository scheduleRepository)
            : base(repository)
        {
            _logger = logger;
            _employeeRepository = employeeRepository;
            _groupRepository = groupRepository;
            _scheduleRepository = scheduleRepository;

            GetPolicyName = AttendanceManagementPermissions.Schedules.Default;
            GetListPolicyName = AttendanceManagementPermissions.Schedules.Default;
            CreatePolicyName = AttendanceManagementPermissions.Schedules.Create;
            UpdatePolicyName = AttendanceManagementPermissions.Schedules.Edit;
            DeletePolicyName = AttendanceManagementPermissions.Schedules.Delete;
        }

        protected override async Task<IQueryable<Schedule>> CreateFilteredQueryAsync(PagedAndSortedResultRequestDto input)
        {
            return (await Repository.WithDetailsAsync(s => s.ScheduleDays));
        }

        public override async Task<ScheduleDto> GetAsync(Guid id)
        {
            var queryable = await Repository.WithDetailsAsync(s => s.ScheduleDays);
            var schedule = await queryable.FirstOrDefaultAsync(s => s.Id == id);

            return ObjectMapper.Map<Schedule, ScheduleDto>(schedule);
        }

        public async Task<List<ScheduleDto>> GetActiveSchedulesAsync()
        {
            var queryable = await Repository.WithDetailsAsync(s => s.ScheduleDays);
            var schedules = await queryable
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync();

            return ObjectMapper.Map<List<Schedule>, List<ScheduleDto>>(schedules);
        }

        public async Task ActivateAsync(Guid id)
        {
            await CheckUpdatePolicyAsync();

            var schedule = await Repository.GetAsync(id);
            schedule.IsActive = true;
            await Repository.UpdateAsync(schedule);
        }

        public async Task DeactivateAsync(Guid id)
        {
            await CheckUpdatePolicyAsync();

            var schedule = await Repository.GetAsync(id);
            schedule.IsActive = false;
            await Repository.UpdateAsync(schedule);
        }

        public async Task AssignScheduleAsync(AssignScheduleDto input)
        {
            await CheckPolicyAsync(AttendanceManagementPermissions.Schedules.Assign);

            // Validate ScheduleId exists
            var schedule = await Repository.GetAsync(input.ScheduleId);

            // Validate at least EmployeeId or GroupId is provided
            if (!input.EmployeeId.HasValue && !input.GroupId.HasValue)
            {
                throw new UserFriendlyException("Either Employee or Group must be specified.");
            }

            // Validate EmployeeId if provided
            if (input.EmployeeId.HasValue)
            {
                var employeeExists = await _employeeRepository.AnyAsync(e => e.Id == input.EmployeeId.Value);
                if (!employeeExists)
                {
                    throw new UserFriendlyException("The specified employee does not exist.");
                }
            }

            // Validate GroupId if provided
            if (input.GroupId.HasValue)
            {
                var groupExists = await _groupRepository.AnyAsync(g => g.Id == input.GroupId.Value);
                if (!groupExists)
                {
                    throw new UserFriendlyException("The specified group does not exist.");
                }
            }

            // Validate date range
            if (input.EffectiveTo.HasValue && input.EffectiveTo.Value <= input.EffectiveFrom)
            {
                throw new UserFriendlyException("Effective to date must be after effective from date.");
            }

            // If assigning to an employee, ensure only one active assignment per employee
            if (input.EmployeeId.HasValue)
            {
                var existingActiveAssignment = await GetEmployeeCurrentScheduleAssignmentEntityAsync(input.EmployeeId.Value);
                if (existingActiveAssignment != null)
                {
                    existingActiveAssignment.EffectiveTo = input.EffectiveFrom.AddDays(-1);
                    await _scheduleRepository.UpdateAsync(existingActiveAssignment);
                }
            }

            var assignment = new ScheduleAssignment(
                GuidGenerator.Create(),
                input.ScheduleId,
                input.EffectiveFrom,
                input.EmployeeId,
                input.GroupId
            )
            {
                SeatNumber = input.SeatNumber,
                FloorNumber = input.FloorNumber,
                EffectiveTo = input.EffectiveTo
            };

            schedule.ScheduleAssignments.Add(assignment);

            if (input.EmployeeId.HasValue)
            {
                var employee = await _employeeRepository.GetAsync(input.EmployeeId.Value);
                employee.ScheduleAssignments.Add(assignment);
                await _employeeRepository.UpdateAsync(employee);
            }

            if (input.GroupId.HasValue)
            {
                var group = await _groupRepository.GetAsync(input.GroupId.Value);
                group.ScheduleAssignments.Add(assignment);
                await _groupRepository.UpdateAsync(group);
            }

            await Repository.UpdateAsync(schedule);
        }

        private async Task<ScheduleAssignment> GetEmployeeCurrentScheduleAssignmentEntityAsync(Guid employeeId)
        {
            var queryable = await _scheduleRepository.GetQueryableAsync();
            return await queryable
                .Where(sa => sa.EmployeeId == employeeId 
                    && sa.EffectiveFrom <= DateTime.Now 
                    && (sa.EffectiveTo == null || sa.EffectiveTo >= DateTime.Now))
                .OrderByDescending(sa => sa.EffectiveFrom)
                .FirstOrDefaultAsync();
        }

        public async Task<ScheduleAssignmentDto> GetEmployeeCurrentScheduleAsync(Guid employeeId)
        {
            _logger.LogDebug("Request for Employee ID" + employeeId + " Schedule");

            var employee = await _employeeRepository.GetAsync(employeeId);
           

            if (employee != null)
            {
                _logger.LogDebug("Employee " + employee.Name + " Found");
            }
            else
            {
                _logger.LogDebug("Employee " + employeeId+ " NOT Found");
            }

            var currentAssignment = await GetEmployeeCurrentScheduleAssignmentEntityAsync(employeeId);    

            if (currentAssignment == null)
            {
                _logger.LogDebug("NO Schedule Assignment Found.");
                return null;
            }

            var schedule = await Repository.GetAsync(currentAssignment.ScheduleId);

            return new ScheduleAssignmentDto
            {
                Id = currentAssignment.Id,
                ScheduleId = currentAssignment.ScheduleId,
                ScheduleName = schedule.Name,
                EmployeeId = currentAssignment.EmployeeId,
                EmployeeName = employee.Name,
                GroupId = currentAssignment.GroupId,
                EffectiveFrom = currentAssignment.EffectiveFrom,
                EffectiveTo = currentAssignment.EffectiveTo,
                SeatNumber = currentAssignment.SeatNumber,
                FloorNumber = currentAssignment.FloorNumber
            };
        }

        public async Task<List<ScheduleAssignmentDto>> GetEmployeeScheduleAssignmentsAsync(Guid employeeId)
        {
            var queryable = await _scheduleRepository.GetQueryableAsync();
            var assignments = await queryable
                .Where(sa => sa.EmployeeId == employeeId)
                .OrderByDescending(sa => sa.EffectiveFrom)
                .Include(sa => sa.Schedule)
                .Include(sa => sa.Employee)
                .ToListAsync();

            return assignments.Select(a => new ScheduleAssignmentDto
            {
                Id = a.Id,
                ScheduleId = a.ScheduleId,
                ScheduleName = a.Schedule?.Name ?? "-",
                EmployeeId = a.EmployeeId,
                EmployeeName = a.Employee?.Name ?? "-",
                GroupId = a.GroupId,
                EffectiveFrom = a.EffectiveFrom,
                EffectiveTo = a.EffectiveTo,
                SeatNumber = a.SeatNumber ?? "-",
                FloorNumber = a.FloorNumber ?? "-"
            }).ToList();
        }

        public async Task UpdateScheduleAssignmentAsync(Guid assignmentId, AssignScheduleDto input)
        {
            await CheckPolicyAsync(AttendanceManagementPermissions.Schedules.Assign);

            var assignment = await _scheduleRepository.GetAsync(assignmentId);

            // Validate ScheduleId exists
            var scheduleExists = await Repository.AnyAsync(s => s.Id == input.ScheduleId);
            if (!scheduleExists)
            {
                throw new UserFriendlyException("The specified schedule does not exist.");
            }

            // Validate EmployeeId if provided
            if (input.EmployeeId.HasValue)
            {
                var employeeExists = await _employeeRepository.AnyAsync(e => e.Id == input.EmployeeId.Value);
                if (!employeeExists)
                {
                    throw new UserFriendlyException("The specified employee does not exist.");
                }
            }

            // Validate GroupId if provided
            if (input.GroupId.HasValue)
            {
                var groupExists = await _groupRepository.AnyAsync(g => g.Id == input.GroupId.Value);
                if (!groupExists)
                {
                    throw new UserFriendlyException("The specified group does not exist.");
                }
            }

            // Validate date range
            if (input.EffectiveTo.HasValue && input.EffectiveTo.Value <= input.EffectiveFrom)
            {
                throw new UserFriendlyException("Effective to date must be after effective from date.");
            }
            
            // If schedule changed, we need to update the schedule's assignments
            if (assignment.ScheduleId != input.ScheduleId)
            {
                var oldSchedule = await Repository.GetAsync(assignment.ScheduleId);
                var newSchedule = await Repository.GetAsync(input.ScheduleId);
                
                // Remove from old schedule
                oldSchedule.ScheduleAssignments.Remove(assignment);
                await Repository.UpdateAsync(oldSchedule);
                
                // Add to new schedule
                assignment.ScheduleId = input.ScheduleId;
                newSchedule.ScheduleAssignments.Add(assignment);
                await Repository.UpdateAsync(newSchedule);
            }

            // If assigning to an employee and effective from date changed, end any overlapping active assignment
            if (input.EmployeeId.HasValue && assignment.EffectiveFrom != input.EffectiveFrom && assignment.EffectiveTo == null)
            {
                var existingActive = await GetEmployeeCurrentScheduleAssignmentEntityAsync(input.EmployeeId.Value);
                if (existingActive != null && existingActive.Id != assignmentId)
                {
                    existingActive.EffectiveTo = input.EffectiveFrom.AddDays(-1);
                    await _scheduleRepository.UpdateAsync(existingActive);
                }
            }

            // Update assignment properties
            assignment.EffectiveFrom = input.EffectiveFrom;
            assignment.EffectiveTo = input.EffectiveTo;
            assignment.SeatNumber = input.SeatNumber;
            assignment.FloorNumber = input.FloorNumber;

            await _scheduleRepository.UpdateAsync(assignment);
        }

        public async Task EndScheduleAssignmentAsync(Guid assignmentId, DateTime endDate)
        {
            await CheckPolicyAsync(AttendanceManagementPermissions.Schedules.Assign);

            var assignment = await _scheduleRepository.GetAsync(assignmentId);
            assignment.EffectiveTo = endDate;
            await _scheduleRepository.UpdateAsync(assignment);
        }

        // TODO: Implement export functionality
        public async Task<byte[]> ExportEmployeeScheduleAsync(Guid employeeId)
        {
            throw new NotImplementedException();
        }

        public override async Task<ScheduleDto> CreateAsync(CreateUpdateScheduleDto input)
        {
            // Validate schedule days have unique DayOfWeek values
            var duplicateDays = input.ScheduleDays
                .GroupBy(d => d.DayOfWeek)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateDays.Any())
            {
                throw new UserFriendlyException($"Duplicate days of week found: {string.Join(", ", duplicateDays)}");
            }

            var schedule = new Schedule(
                GuidGenerator.Create(),
                input.Name,
                input.Description
            );

            foreach (var dayDto in input.ScheduleDays)
            {
                var scheduleDay = new ScheduleDay(
                    GuidGenerator.Create(),
                    schedule.Id,
                    dayDto.DayOfWeek,
                    dayDto.IsOnSite
                );
                schedule.ScheduleDays.Add(scheduleDay);
            }

            await Repository.InsertAsync(schedule);
            return ObjectMapper.Map<Schedule, ScheduleDto>(schedule);
        }

        public override async Task<ScheduleDto> UpdateAsync(Guid id, CreateUpdateScheduleDto input)
        {
            var queryable = await Repository.WithDetailsAsync(s => s.ScheduleDays);
            var schedule = await queryable.FirstOrDefaultAsync(s => s.Id == id);

            // Validate schedule days have unique DayOfWeek values
            var duplicateDays = input.ScheduleDays
                .GroupBy(d => d.DayOfWeek)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateDays.Any())
            {
                throw new UserFriendlyException($"Duplicate days of week found: {string.Join(", ", duplicateDays)}");
            }

            schedule.Name = input.Name;
            schedule.Description = input.Description;

            // Clear existing days and add new ones
            schedule.ScheduleDays.Clear();
            foreach (var dayDto in input.ScheduleDays)
            {
                var scheduleDay = new ScheduleDay(
                    GuidGenerator.Create(),
                    schedule.Id,
                    dayDto.DayOfWeek,
                    dayDto.IsOnSite
                );
                schedule.ScheduleDays.Add(scheduleDay);
            }

            await Repository.UpdateAsync(schedule);
            return ObjectMapper.Map<Schedule, ScheduleDto>(schedule);
        }
    }
}
