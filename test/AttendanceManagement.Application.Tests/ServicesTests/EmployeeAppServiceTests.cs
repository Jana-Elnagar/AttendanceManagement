using AttendanceManagement.Dtos.Employees;
using AttendanceManagement.Interfaces;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Modularity;
using Xunit;

namespace AttendanceManagement.ServicesTests
{
    public class EmployeeAppServiceTests : AttendanceManagementApplicationTestBase<AttendanceManagementApplicationTestModule>


    {
        private readonly IEmployeeAppService _employeeAppService;

        public EmployeeAppServiceTests()
        {
            _employeeAppService = GetRequiredService<IEmployeeAppService>();
        }

        [Fact]
        public async Task Should_Get_All_Employees()
        {
            // Act
            var result = await _employeeAppService.GetListAsync(new PagedAndSortedResultRequestDto());

            // Assert
            result.TotalCount.ShouldBeGreaterThan(0);
            result.Items.ShouldNotBeEmpty();
        }

        [Fact]
        public async Task Should_Get_Active_Employees_Only()
        {
            // Act
            var result = await _employeeAppService.GetActiveEmployeesAsync();

            // Assert
            result.ShouldNotBeEmpty();
            result.ShouldAllBe(e => e.IsActive);
        }

        [Fact]
        public async Task Should_Get_Employee_By_Id()
        {
            // Arrange
            var employees = await _employeeAppService.GetActiveEmployeesAsync();
            var firstEmployee = employees.First();

            // Act
            var result = await _employeeAppService.GetAsync(firstEmployee.Id);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(firstEmployee.Id);
            result.Name.ShouldBe(firstEmployee.Name);
        }

        [Fact]
        public async Task Should_Get_Employee_With_Details()
        {
            // Arrange
            var employees = await _employeeAppService.GetActiveEmployeesAsync();
            var employeeWithSchedule = employees.First(e => e.Name == "Alice Employee");

            // Act
            var result = await _employeeAppService.GetWithDetailsAsync(employeeWithSchedule.Id);

            // Assert
            result.ShouldNotBeNull();
            result.CurrentSchedule.ShouldNotBeNull();
            result.ScheduleDays.ShouldNotBeEmpty();
            result.SeatNumber.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public async Task Should_Create_Employee()
        {
            // Arrange
            var newUserId = Guid.NewGuid();
            var groups = await GetRequiredService<IGroupAppService>().GetActiveGroupsAsync();
            var workflows = await GetRequiredService<IWorkflowAppService>().GetActiveWorkflowsAsync();

            var input = new CreateUpdateEmployeeDto
            {
                UserId = newUserId,
                Name = "Test Employee",
                Department = "IT",
                Sector = "Development",
                GroupId = groups.First().Id,
                WorkflowId = workflows.First().Id
            };

            // Act
            var result = await _employeeAppService.CreateAsync(input);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(Guid.Empty);
            result.Name.ShouldBe(input.Name);
            result.Department.ShouldBe(input.Department);
            result.IsActive.ShouldBeTrue();
        }

        [Fact]
        public async Task Should_Update_Employee()
        {
            // Arrange
            var employees = await _employeeAppService.GetActiveEmployeesAsync();
            var employee = employees.First();

            var input = new CreateUpdateEmployeeDto
            {
                UserId = employee.UserId,
                Name = "Updated Name",
                Department = "Updated Department",
                Sector = employee.Sector,
                GroupId = employee.GroupId,
                WorkflowId = employee.WorkflowId
            };

            // Act
            var result = await _employeeAppService.UpdateAsync(employee.Id, input);

            // Assert
            result.Name.ShouldBe("Updated Name");
            result.Department.ShouldBe("Updated Department");
        }

        [Fact]
        public async Task Should_Deactivate_Employee()
        {
            // Arrange
            var employees = await _employeeAppService.GetActiveEmployeesAsync();
            var employee = employees.First();

            // Act
            await _employeeAppService.DeactivateAsync(employee.Id);

            // Assert
            var deactivated = await _employeeAppService.GetAsync(employee.Id);
            deactivated.IsActive.ShouldBeFalse();

            // Cleanup - Reactivate
            await _employeeAppService.ActivateAsync(employee.Id);
        }

        [Fact]
        public async Task Should_Assign_Manager_To_Employee()
        {
            // Arrange
            var employees = await _employeeAppService.GetActiveEmployeesAsync();
            var employee = employees.First(e => e.Name == "Alice Employee");
            var manager = employees.First(e => e.Name == "John Manager");

            var input = new AssignManagerDto
            {
                EmployeeId = employee.Id,
                ManagerEmployeeId = manager.Id,
                IsPrimaryManager = true,
                EffectiveFrom = DateTime.Now
            };

            // Act
            await _employeeAppService.AssignManagerAsync(input);

            // Assert
            var managers = await _employeeAppService.GetManagersAsync(employee.Id);
            managers.ShouldContain(m => m.ManagerEmployeeId == manager.Id);
        }

        [Fact]
        public async Task Should_Get_Managers_For_Employee()
        {
            // Arrange
            var employees = await _employeeAppService.GetActiveEmployeesAsync();
            var employee = employees.First(e => e.Name == "Alice Employee");

            // Act
            var result = await _employeeAppService.GetManagersAsync(employee.Id);

            // Assert
            result.ShouldNotBeEmpty();
            result.First().ManagerName.ShouldNotBeNullOrEmpty();
        }
    }
}
