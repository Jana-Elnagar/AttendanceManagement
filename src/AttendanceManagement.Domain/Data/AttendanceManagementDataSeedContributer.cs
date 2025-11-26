using AttendanceManagement.Data.Employees;
using AttendanceManagement.Data.ExceptionRequests;
using AttendanceManagement.Data.Groups;
using AttendanceManagement.Data.Schedules;
using AttendanceManagement.Data.Workflows;
using AttendanceManagement.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Identity;

namespace AttendanceManagement.Data
{
    public class AttendanceManagementDataSeedContributer : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<Employee, Guid> _employeeRepository;
        private readonly IRepository<Group, Guid> _groupRepository;
        private readonly IRepository<Schedule, Guid> _scheduleRepository;
        private readonly IRepository<Workflow, Guid> _workflowRepository;
        private readonly IIdentityUserRepository _identityUserRepository;
        private readonly IGuidGenerator _guidGenerator;

        // Predefined GUIDs for consistent seeding
        private Guid _adminUserId;
        private Guid _hrManagerUserId;
        private Guid _manager1UserId;
        private Guid _manager2UserId;
        private Guid _doctorUserId;
        private Guid _employee1UserId;
        private Guid _employee2UserId;
        private Guid _employee3UserId;
        private Guid _employee4UserId;
        private Guid _employee5UserId;

        public AttendanceManagementDataSeedContributer(
            IRepository<Employee, Guid> employeeRepository,
            IRepository<Group, Guid> groupRepository,
            IRepository<Schedule, Guid> scheduleRepository,
            IRepository<Workflow, Guid> workflowRepository,
            IIdentityUserRepository identityUserRepository,
            IGuidGenerator guidGenerator)
        {
            _employeeRepository = employeeRepository;
            _groupRepository = groupRepository;
            _scheduleRepository = scheduleRepository;
            _workflowRepository = workflowRepository;
            _identityUserRepository = identityUserRepository;
            _guidGenerator = guidGenerator;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            // Check if already seeded
            if (await _employeeRepository.GetCountAsync() > 0)
            {
                return;
            }

            // Get existing Identity users (created by ABP)
            var adminUser = await _identityUserRepository.FindByNormalizedUserNameAsync("ADMIN");
            _adminUserId = adminUser.Id;

            // Seed Identity Users first
            await SeedIdentityUsersAsync();

            // Seed Groups first (before employees)
            var groups = await SeedGroupsAsync();

            // Seed Employees with group assignments
            var employees = await SeedEmployeesAsync(groups);

            // Seed Manager Assignments (attach to employee aggregates and update)
            await SeedManagerAssignmentsAsync(employees);

            // Seed Schedules (including days) - schedules are aggregates
            var schedules = await SeedSchedulesAsync();

            // Seed Schedule Assignments (attach to schedule aggregates and update)
            await SeedScheduleAssignmentsAsync(schedules, employees, groups);

            // Seed Workflows per Employee (workflow is an aggregate; add steps to Workflow.WorkflowSteps before inserting)
            await SeedWorkflowsAsync(employees);
        }

        private async Task SeedIdentityUsersAsync()
        {
            // Get or create users
            var hrManager = await GetOrCreateUserAsync("hrmanager", "hr@attendance.com", "HR Manager", "HRManager123!");
            _hrManagerUserId = hrManager.Id;

            var manager1 = await GetOrCreateUserAsync("manager1", "manager1@attendance.com", "John Manager", "Manager123!");
            _manager1UserId = manager1.Id;

            var manager2 = await GetOrCreateUserAsync("manager2", "manager2@attendance.com", "Sarah Manager", "Manager123!");
            _manager2UserId = manager2.Id;

            var doctor = await GetOrCreateUserAsync("doctor", "doctor@attendance.com", "Dr. Smith", "Doctor123!");
            _doctorUserId = doctor.Id;

            var emp1 = await GetOrCreateUserAsync("emp1", "emp1@attendance.com", "Alice Employee", "Employee123!");
            _employee1UserId = emp1.Id;

            var emp2 = await GetOrCreateUserAsync("emp2", "emp2@attendance.com", "Bob Employee", "Employee123!");
            _employee2UserId = emp2.Id;

            var emp3 = await GetOrCreateUserAsync("emp3", "emp3@attendance.com", "Charlie Employee", "Employee123!");
            _employee3UserId = emp3.Id;

            var emp4 = await GetOrCreateUserAsync("emp4", "emp4@attendance.com", "Diana Employee", "Employee123!");
            _employee4UserId = emp4.Id;

            var emp5 = await GetOrCreateUserAsync("emp5", "emp5@attendance.com", "Eve Employee", "Employee123!");
            _employee5UserId = emp5.Id;
        }

        private async Task<IdentityUser> GetOrCreateUserAsync(string userName, string email, string name, string password)
        {
            var user = await _identityUserRepository.FindByNormalizedUserNameAsync(userName.ToUpperInvariant());
            if (user == null)
            {
                user = new IdentityUser(_guidGenerator.Create(), userName, email)
                {
                    Name = name
                };
                await _identityUserRepository.InsertAsync(user);
            }
            return user;
        }

        private async Task<Dictionary<string, Group>> SeedGroupsAsync()
        {
            var groups = new Dictionary<string, Group>
            {
                ["engineering"] = new Group(
                    _guidGenerator.Create(),
                    "Engineering Team",
                    "Software development and engineering team"
                ),
                ["marketing"] = new Group(
                    _guidGenerator.Create(),
                    "Marketing Team",
                    "Digital marketing and content team"
                ),
                ["management"] = new Group(
                    _guidGenerator.Create(),
                    "Management Team",
                    "Managers and team leads"
                )
            };

            await _groupRepository.InsertManyAsync(groups.Values);

            return groups;
        }

        private async Task<Dictionary<string, Employee>> SeedEmployeesAsync(Dictionary<string, Group> groups)
        {
            var employees = new Dictionary<string, Employee>();

            // HR Manager
            var hrManager = new Employee(
                _guidGenerator.Create(),
                _hrManagerUserId,
                "HR Manager",
                "Human Resources",
                "HR Sector",
                groups["management"].Id
            )
            {
                ManagerAssignments = new List<ManagerAssignment>(),
                ManagedEmployees = new List<ManagerAssignment>(),
                ScheduleAssignments = new List<ScheduleAssignment>(),
                ExceptionRequests = new List<ExceptionRequest>()
            };
            employees["hrmanager"] = hrManager;

            // Managers
            var manager1 = new Employee(
                _guidGenerator.Create(),
                _manager1UserId,
                "John Manager",
                "Engineering",
                "Software Development",
                groups["management"].Id
            )
            {
                ManagerAssignments = new List<ManagerAssignment>(),
                ManagedEmployees = new List<ManagerAssignment>(),
                ScheduleAssignments = new List<ScheduleAssignment>(),
                ExceptionRequests = new List<ExceptionRequest>()
            };
            employees["manager1"] = manager1;

            var manager2 = new Employee(
                _guidGenerator.Create(),
                _manager2UserId,
                "Sarah Manager",
                "Marketing",
                "Digital Marketing",
                groups["management"].Id
            )
            {
                ManagerAssignments = new List<ManagerAssignment>(),
                ManagedEmployees = new List<ManagerAssignment>(),
                ScheduleAssignments = new List<ScheduleAssignment>(),
                ExceptionRequests = new List<ExceptionRequest>()
            };
            employees["manager2"] = manager2;

            //// Doctor (no group)
            //var doctor = new Employee(
            //    _guidGenerator.Create(),
            //    _doctorUserId,
            //    "Dr. Smith",
            //    "Medical",
            //    "Occupational Health",
            //    null
            //)
            //{
            //    //ManagerAssignments = new List<ManagerAssignment>(),
            //    //ManagedEmployees = new List<ManagerAssignment>(),
            //    //ScheduleAssignments = new List<ScheduleAssignment>(),
            //    //ExceptionRequests = new List<ExceptionRequest>()
            //};
            //employees["doctor"] = doctor;

            // Regular Employees
            var emp1 = new Employee(
                _guidGenerator.Create(),
                _employee1UserId,
                "Alice Employee",
                "Engineering",
                "Software Development",
                groups["engineering"].Id
            )
            {
                ManagerAssignments = new List<ManagerAssignment>(),
                ManagedEmployees = new List<ManagerAssignment>(),
                ScheduleAssignments = new List<ScheduleAssignment>(),
                ExceptionRequests = new List<ExceptionRequest>()
            };
            employees["emp1"] = emp1;

            var emp2 = new Employee(
                _guidGenerator.Create(),
                _employee2UserId,
                "Bob Employee",
                "Engineering",
                "Software Development",
                groups["engineering"].Id
            )
            {
                ManagerAssignments = new List<ManagerAssignment>(),
                ManagedEmployees = new List<ManagerAssignment>(),
                ScheduleAssignments = new List<ScheduleAssignment>(),
                ExceptionRequests = new List<ExceptionRequest>()
            };
            employees["emp2"] = emp2;

            var emp3 = new Employee(
                _guidGenerator.Create(),
                _employee3UserId,
                "Charlie Employee",
                "Marketing",
                "Digital Marketing",
                groups["marketing"].Id
            )
            {
                ManagerAssignments = new List<ManagerAssignment>(),
                ManagedEmployees = new List<ManagerAssignment>(),
                ScheduleAssignments = new List<ScheduleAssignment>(),
                ExceptionRequests = new List<ExceptionRequest>()
            };
            employees["emp3"] = emp3;

            var emp4 = new Employee(
                _guidGenerator.Create(),
                _employee4UserId,
                "Diana Employee",
                "Marketing",
                "Digital Marketing",
                groups["marketing"].Id
            )
            {
                ManagerAssignments = new List<ManagerAssignment>(),
                ManagedEmployees = new List<ManagerAssignment>(),
                ScheduleAssignments = new List<ScheduleAssignment>(),
                ExceptionRequests = new List<ExceptionRequest>()
            };
            employees["emp4"] = emp4;

            var emp5 = new Employee(
                _guidGenerator.Create(),
                _employee5UserId,
                "Eve Employee",
                "Engineering",
                "Software Development",
                groups["engineering"].Id
            )
            {
                ManagerAssignments = new List<ManagerAssignment>(),
                ManagedEmployees = new List<ManagerAssignment>(),
                ScheduleAssignments = new List<ScheduleAssignment>(),
                ExceptionRequests = new List<ExceptionRequest>()
            };
            employees["emp5"] = emp5;

            await _employeeRepository.InsertManyAsync(employees.Values);

            return employees;
        }

        private async Task SeedManagerAssignmentsAsync(Dictionary<string, Employee> employees)
        {
            var assignments = new List<ManagerAssignment>
            {
                // Manager 1 manages emp1, emp2, emp5
                new ManagerAssignment(
                    _guidGenerator.Create(),
                    employees["emp1"].Id,
                    employees["manager1"].Id,
                    true,
                    DateTime.UtcNow.AddMonths(-6)
                ),
                new ManagerAssignment(
                    _guidGenerator.Create(),
                    employees["emp2"].Id,
                    employees["manager1"].Id,
                    true,
                    DateTime.UtcNow.AddMonths(-6)
                ),
                new ManagerAssignment(
                    _guidGenerator.Create(),
                    employees["emp5"].Id,
                    employees["manager1"].Id,
                    true,
                    DateTime.UtcNow.AddMonths(-6)
                ),

                // Manager 2 manages emp3, emp4
                new ManagerAssignment(
                    _guidGenerator.Create(),
                    employees["emp3"].Id,
                    employees["manager2"].Id,
                    true,
                    DateTime.UtcNow.AddMonths(-6)
                ),
                new ManagerAssignment(
                    _guidGenerator.Create(),
                    employees["emp4"].Id,
                    employees["manager2"].Id,
                    true,
                    DateTime.UtcNow.AddMonths(-6)
                )
            };

            // Attach assignments to aggregates and update affected employee aggregates
            var employeesToUpdate = new HashSet<Employee>();

            foreach (var assignment in assignments)
            {
                var employee = employees.Values.First(e => e.Id == assignment.EmployeeId);
                var manager = employees.Values.First(e => e.Id == assignment.ManagerEmployeeId);

                employee.ManagerAssignments ??= new List<ManagerAssignment>();
                manager.ManagedEmployees ??= new List<ManagerAssignment>();

                employee.ManagerAssignments.Add(assignment);
                manager.ManagedEmployees.Add(assignment);

                employeesToUpdate.Add(employee);
                employeesToUpdate.Add(manager);
            }

            // Persist changes via aggregate repository updates
            foreach (var emp in employeesToUpdate)
            {
                await _employeeRepository.UpdateAsync(emp);
            }
        }

        private async Task<Dictionary<string, Schedule>> SeedSchedulesAsync()
        {
            var schedules = new Dictionary<string, Schedule>
            {
                ["hybrid_3days"] = new Schedule(
                    _guidGenerator.Create(),
                    "Hybrid 3 Days On-Site",
                    "3 days on-site (Sun, Tue, Thu), 2 days remote"
                )
                {
                    ScheduleDays = new List<ScheduleDay>(),
                    ScheduleAssignments = new List<ScheduleAssignment>()
                },
                ["hybrid_2days"] = new Schedule(
                    _guidGenerator.Create(),
                    "Hybrid 2 Days On-Site",
                    "2 days on-site (Mon, Wed), 3 days remote"
                )
                {
                    ScheduleDays = new List<ScheduleDay>(),
                    ScheduleAssignments = new List<ScheduleAssignment>()
                },
                ["full_onsite"] = new Schedule(
                    _guidGenerator.Create(),
                    "Full On-Site",
                    "5 days on-site per week"
                )
                {
                    ScheduleDays = new List<ScheduleDay>(),
                    ScheduleAssignments = new List<ScheduleAssignment>()
                },
                ["full_remote"] = new Schedule(
                    _guidGenerator.Create(),
                    "Full Remote",
                    "Remote work arrangement"
                )
                {
                    ScheduleDays = new List<ScheduleDay>(),
                    ScheduleAssignments = new List<ScheduleAssignment>()
                }
            };

            // Add schedule days directly into schedule aggregates
            // Hybrid 3 Days: Sun, Tue, Thu on-site
            var hybrid3Days = new[]
            {
                new ScheduleDay(_guidGenerator.Create(), schedules["hybrid_3days"].Id, DayOfWeek.Sunday, true),
                new ScheduleDay(_guidGenerator.Create(), schedules["hybrid_3days"].Id, DayOfWeek.Monday, false),
                new ScheduleDay(_guidGenerator.Create(), schedules["hybrid_3days"].Id, DayOfWeek.Tuesday, true),
                new ScheduleDay(_guidGenerator.Create(), schedules["hybrid_3days"].Id, DayOfWeek.Wednesday, false),
                new ScheduleDay(_guidGenerator.Create(), schedules["hybrid_3days"].Id, DayOfWeek.Thursday, true),
                new ScheduleDay(_guidGenerator.Create(), schedules["hybrid_3days"].Id, DayOfWeek.Friday, false),
                new ScheduleDay(_guidGenerator.Create(), schedules["hybrid_3days"].Id, DayOfWeek.Saturday, false)
            };
            foreach (var d in hybrid3Days) schedules["hybrid_3days"].ScheduleDays.Add(d);

            // Hybrid 2 Days: Mon, Wed on-site
            var hybrid2Days = new[]
            {
                new ScheduleDay(_guidGenerator.Create(), schedules["hybrid_2days"].Id, DayOfWeek.Sunday, false),
                new ScheduleDay(_guidGenerator.Create(), schedules["hybrid_2days"].Id, DayOfWeek.Monday, true),
                new ScheduleDay(_guidGenerator.Create(), schedules["hybrid_2days"].Id, DayOfWeek.Tuesday, false),
                new ScheduleDay(_guidGenerator.Create(), schedules["hybrid_2days"].Id, DayOfWeek.Wednesday, true),
                new ScheduleDay(_guidGenerator.Create(), schedules["hybrid_2days"].Id, DayOfWeek.Thursday, false),
                new ScheduleDay(_guidGenerator.Create(), schedules["hybrid_2days"].Id, DayOfWeek.Friday, false),
                new ScheduleDay(_guidGenerator.Create(), schedules["hybrid_2days"].Id, DayOfWeek.Saturday, false)
            };
            foreach (var d in hybrid2Days) schedules["hybrid_2days"].ScheduleDays.Add(d);

            // Full On-Site: Sun-Thu
            var fullOnsiteDays = new[]
            {
                new ScheduleDay(_guidGenerator.Create(), schedules["full_onsite"].Id, DayOfWeek.Sunday, true),
                new ScheduleDay(_guidGenerator.Create(), schedules["full_onsite"].Id, DayOfWeek.Monday, true),
                new ScheduleDay(_guidGenerator.Create(), schedules["full_onsite"].Id, DayOfWeek.Tuesday, true),
                new ScheduleDay(_guidGenerator.Create(), schedules["full_onsite"].Id, DayOfWeek.Wednesday, true),
                new ScheduleDay(_guidGenerator.Create(), schedules["full_onsite"].Id, DayOfWeek.Thursday, true),
                new ScheduleDay(_guidGenerator.Create(), schedules["full_onsite"].Id, DayOfWeek.Friday, false),
                new ScheduleDay(_guidGenerator.Create(), schedules["full_onsite"].Id, DayOfWeek.Saturday, false)
            };
            foreach (var d in fullOnsiteDays) schedules["full_onsite"].ScheduleDays.Add(d);

            // Full Remote: all days remote
            var fullRemoteDays = new[]
            {
                new ScheduleDay(_guidGenerator.Create(), schedules["full_remote"].Id, DayOfWeek.Sunday, false),
                new ScheduleDay(_guidGenerator.Create(), schedules["full_remote"].Id, DayOfWeek.Monday, false),
                new ScheduleDay(_guidGenerator.Create(), schedules["full_remote"].Id, DayOfWeek.Tuesday, false),
                new ScheduleDay(_guidGenerator.Create(), schedules["full_remote"].Id, DayOfWeek.Wednesday, false),
                new ScheduleDay(_guidGenerator.Create(), schedules["full_remote"].Id, DayOfWeek.Thursday, false),
                new ScheduleDay(_guidGenerator.Create(), schedules["full_remote"].Id, DayOfWeek.Friday, false),
                new ScheduleDay(_guidGenerator.Create(), schedules["full_remote"].Id, DayOfWeek.Saturday, false)
            };
            foreach (var d in fullRemoteDays) schedules["full_remote"].ScheduleDays.Add(d);

            // Persist schedules (EF will persist schedule days as children of Schedule aggregate)
            await _scheduleRepository.InsertManyAsync(schedules.Values);

            return schedules;
        }

        private async Task SeedScheduleAssignmentsAsync(
            Dictionary<string, Schedule> schedules,
            Dictionary<string, Employee> employees,
            Dictionary<string, Group> groups)
        {
            var assignments = new List<ScheduleAssignment>
            {
                // Individual assignments with seat numbers
                new ScheduleAssignment(
                    _guidGenerator.Create(),
                    schedules["hybrid_3days"].Id,
                    DateTime.UtcNow.AddMonths(-3),
                    employeeId: employees["emp1"].Id
                ) { SeatNumber = "A-101", FloorNumber = "Building A, Floor 1" },

                new ScheduleAssignment(
                    _guidGenerator.Create(),
                    schedules["hybrid_3days"].Id,
                    DateTime.UtcNow.AddMonths(-3),
                    employeeId: employees["emp2"].Id
                ) { SeatNumber = "A-102", FloorNumber = "Building A, Floor 1" },

                new ScheduleAssignment(
                    _guidGenerator.Create(),
                    schedules["hybrid_2days"].Id,
                    DateTime.UtcNow.AddMonths(-3),
                    employeeId: employees["emp3"].Id
                ) { SeatNumber = "B-201", FloorNumber = "Building B, Floor 2" },

                new ScheduleAssignment(
                    _guidGenerator.Create(),
                    schedules["hybrid_2days"].Id,
                    DateTime.UtcNow.AddMonths(-3),
                    employeeId: employees["emp4"].Id
                ) { SeatNumber = "B-202", FloorNumber = "Building B, Floor 2" },

                new ScheduleAssignment(
                    _guidGenerator.Create(),
                    schedules["full_onsite"].Id,
                    DateTime.UtcNow.AddMonths(-3),
                    employeeId: employees["emp5"].Id
                ) { SeatNumber = "A-103", FloorNumber = "Building A, Floor 1" },

                // Manager and HR full remote
                new ScheduleAssignment(
                    _guidGenerator.Create(),
                    schedules["full_remote"].Id,
                    DateTime.UtcNow.AddMonths(-3),
                    employeeId: employees["manager1"].Id
                ) { SeatNumber = "Remote", FloorNumber = "Remote" },

                new ScheduleAssignment(
                    _guidGenerator.Create(),
                    schedules["full_remote"].Id,
                    DateTime.UtcNow.AddMonths(-3),
                    employeeId: employees["manager2"].Id
                ) { SeatNumber = "Remote", FloorNumber = "Remote" },

                new ScheduleAssignment(
                    _guidGenerator.Create(),
                    schedules["full_remote"].Id,
                    DateTime.UtcNow.AddMonths(-3),
                    employeeId: employees["hrmanager"].Id
                ) { SeatNumber = "Remote", FloorNumber = "Remote" }
            };

            // Attach assignments to schedule aggregates and corresponding employee aggregates, then persist schedules
            var schedulesToUpdate = new HashSet<Schedule>();
            var employeesToUpdate = new HashSet<Employee>();

            foreach (var assignment in assignments)
            {
                var schedule = schedules.Values.First(s => s.Id == assignment.ScheduleId);
                schedule.ScheduleAssignments ??= new List<ScheduleAssignment>();
                schedule.ScheduleAssignments.Add(assignment);
                schedulesToUpdate.Add(schedule);

                if (assignment.EmployeeId.HasValue)
                {
                    var emp = employees.Values.First(e => e.Id == assignment.EmployeeId.Value);
                    emp.ScheduleAssignments ??= new List<ScheduleAssignment>();
                    emp.ScheduleAssignments.Add(assignment);
                    employeesToUpdate.Add(emp);
                }

                if (assignment.GroupId.HasValue)
                {
                    var grp = groups.Values.First(g => g.Id == assignment.GroupId.Value);
                    grp.ScheduleAssignments ??= new List<ScheduleAssignment>();
                    grp.ScheduleAssignments.Add(assignment);
                    // groups aren't updated here because AddDefaultRepositories for Group exists; optionally update group repository if needed
                }
            }

            // Persist schedules (cascade will insert ScheduleAssignment children)
            foreach (var sch in schedulesToUpdate)
            {
                await _scheduleRepository.UpdateAsync(sch);
            }

            // Persist employee aggregate updates where we added schedule assignments or manager assignments earlier
            foreach (var emp in employeesToUpdate)
            {
                await _employeeRepository.UpdateAsync(emp);
            }
        }

        private async Task SeedWorkflowsAsync(Dictionary<string, Employee> employees)
        {
            // Create workflows per employee
            var workflows = new List<Workflow>();

            // Standard workflow for employees with managers
            var standardEmployees = new[] { "emp1", "emp2", "emp3", "emp4", "emp5" };

            foreach (var empKey in standardEmployees)
            {
                var employee = employees[empKey];
                var workflow = new Workflow(
                    _guidGenerator.Create(),
                    $"Workflow for {employee.Name}",
                    $"Standard approval workflow for {employee.Name}",
                    employee.Id
                )
                {
                    WorkflowSteps = new List<WorkflowStep>()
                };

                // Determine manager
                var managerId = empKey == "emp1" || empKey == "emp2" || empKey == "emp5"
                    ? employees["manager1"].Id
                    : employees["manager2"].Id;

                // Step 1: Manager Approval
                workflow.WorkflowSteps.Add(new WorkflowStep(
                    _guidGenerator.Create(),
                    workflow.Id,
                    1,
                    ApproverType.Manager,
                    managerId
                ));

                // Step 2: HR Manager Approval
                workflow.WorkflowSteps.Add(new WorkflowStep(
                    _guidGenerator.Create(),
                    workflow.Id,
                    2,
                    ApproverType.HRManager,
                    employees["hrmanager"].Id
                ));

                workflows.Add(workflow);
            }

            // Workflow for managers (only HR approval needed)
            foreach (var managerKey in new[] { "manager1", "manager2" })
            {
                var manager = employees[managerKey];
                var workflow = new Workflow(
                    _guidGenerator.Create(),
                    $"Workflow for {manager.Name}",
                    $"Manager approval workflow for {manager.Name}",
                    manager.Id
                )
                {
                    WorkflowSteps = new List<WorkflowStep>()
                };

                workflow.WorkflowSteps.Add(new WorkflowStep(
                    _guidGenerator.Create(),
                    workflow.Id,
                    1,
                    ApproverType.HRManager,
                    employees["hrmanager"].Id
                ));

                workflows.Add(workflow);
            }

            // Insert workflows with steps as children of the Workflow aggregate
            await _workflowRepository.InsertManyAsync(workflows);
        }
    }
}
