using AttendanceManagement.Data.Employees;
using AttendanceManagement.Data.Groups;
using AttendanceManagement.Dtos.Employees;
using AttendanceManagement.Dtos.Group;
using AttendanceManagement.Interfaces;
using AttendanceManagement.Permissions;
using Microsoft.EntityFrameworkCore;
using AutoMapper.Internal.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp;

namespace AttendanceManagement.Services
{
    public class GroupAppService :
        CrudAppService<Group, GroupDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateGroupDto>,
        IGroupAppService
    {
        public GroupAppService(IRepository<Group, Guid> repository) : base(repository)
        {
            GetPolicyName = AttendanceManagementPermissions.Groups.Default;
            GetListPolicyName = AttendanceManagementPermissions.Groups.Default;
            CreatePolicyName = AttendanceManagementPermissions.Groups.Create;
            UpdatePolicyName = AttendanceManagementPermissions.Groups.Edit;
            DeletePolicyName = AttendanceManagementPermissions.Groups.Delete;
        }

        protected override async Task<IQueryable<Group>> CreateFilteredQueryAsync(PagedAndSortedResultRequestDto input)
        {
            return await Repository.WithDetailsAsync(g => g.Employees);
        }

        public async Task<GroupWithEmployeesDto> GetWithEmployeesAsync(Guid id)
        {
            var queryable = await Repository.WithDetailsAsync(g => g.Employees);
            var group = await queryable.FirstOrDefaultAsync(g => g.Id == id);

            var dto = ObjectMapper.Map<Group, GroupWithEmployeesDto>(group);
            dto.Employees = ObjectMapper.Map<List<Employee>, List<EmployeeDto>>(
                group.Employees.Where(e => e.IsActive).ToList());

            return dto;
        }

        public async Task<List<GroupDto>> GetActiveGroupsAsync()
        {
            var queryable = await Repository.WithDetailsAsync(g => g.Employees);
            var groups = await queryable
                .Where(g => g.IsActive)
                .OrderBy(g => g.Name)
                .ToListAsync();

            return ObjectMapper.Map<List<Group>, List<GroupDto>>(groups);
        }

        public async Task ActivateAsync(Guid id)
        {
            await CheckUpdatePolicyAsync();

            var group = await Repository.GetAsync(id);
            group.IsActive = true;
            await Repository.UpdateAsync(group);
        }

        public async Task DeactivateAsync(Guid id)
        {
            await CheckUpdatePolicyAsync();

            var group = await Repository.GetAsync(id);
            group.IsActive = false;
            await Repository.UpdateAsync(group);
        }

        public override async Task<GroupDto> CreateAsync(CreateUpdateGroupDto input)
        {
            // Validate name uniqueness
            var existingGroup = await Repository.FirstOrDefaultAsync(g => g.Name == input.Name);
            if (existingGroup != null)
            {
                throw new UserFriendlyException("A group with this name already exists.");
            }

            var group = new Group(
                GuidGenerator.Create(),
                input.Name,
                input.Description
            );

            await Repository.InsertAsync(group);
            return ObjectMapper.Map<Group, GroupDto>(group);
        }

        public override async Task<GroupDto> UpdateAsync(Guid id, CreateUpdateGroupDto input)
        {
            var group = await Repository.GetAsync(id);

            // Validate name uniqueness (excluding current group)
            var existingGroup = await Repository.FirstOrDefaultAsync(g => g.Name == input.Name && g.Id != id);
            if (existingGroup != null)
            {
                throw new UserFriendlyException("A group with this name already exists.");
            }

            group.Name = input.Name;
            group.Description = input.Description;

            await Repository.UpdateAsync(group);
            return ObjectMapper.Map<Group, GroupDto>(group);
        }
    }
}
