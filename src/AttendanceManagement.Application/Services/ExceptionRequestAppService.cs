using AttendanceManagement.Data.Employees;
using AttendanceManagement.Data.ExceptionRequests;
using AttendanceManagement.Data.Workflows;
using AttendanceManagement.Dtos.ExceptionRequests;
using AttendanceManagement.Enums;
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
using Volo.Abp.Content;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Users;

namespace AttendanceManagement.Services
{
    public class ExceptionRequestAppService : ApplicationService, IExceptionRequestAppService
    {
        private readonly IRepository<Employee, Guid> _employeeRepository;
        private readonly IRepository<Workflow, Guid> _workflowRepository;
        private readonly IExceptionRequestRepository _exceptionRequestRepository;
        private readonly ICurrentUser _currentUser;
        private readonly ILogger<ExceptionRequestAppService> _logger;

        public ExceptionRequestAppService(
            IExceptionRequestRepository exceptionRequestRepository,
            IRepository<Employee, Guid> employeeRepository,
            IRepository<Workflow, Guid> workflowRepository,
            ICurrentUser currentUser,
            ILogger<ExceptionRequestAppService> logger
            )
        {
            _exceptionRequestRepository = exceptionRequestRepository;
            _employeeRepository = employeeRepository;
            _workflowRepository = workflowRepository;
            _currentUser = currentUser;
            _logger = logger;
        }

        public async Task<ExceptionRequestDto> GetAsync(Guid id)
        {
            var queryable = await _exceptionRequestRepository.WithDetailsAsync(
                er => er.Employee,
                er => er.Workflow,
                er => er.ApprovalHistories,
                er => er.Attachments);

            var request = await queryable.FirstOrDefaultAsync(er => er.Id == id);
            if (request == null) {
                throw new UserFriendlyException("Exception request not found.");
            }

            return ObjectMapper.Map<ExceptionRequest, ExceptionRequestDto>(request);
        }

        public async Task<PagedResultDto<ExceptionRequestDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            await CheckPolicyAsync(AttendanceManagementPermissions.ExceptionRequests.ViewAll);

            var queryable = await _exceptionRequestRepository.WithDetailsAsync(
                er => er.Employee,
                er => er.Workflow);

            var totalCount = await queryable.CountAsync();

            var requests = await queryable
                .OrderByDescending(er => er.CreationTime)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToListAsync();

            return new PagedResultDto<ExceptionRequestDto>(
                totalCount,
                ObjectMapper.Map<List<ExceptionRequest>, List<ExceptionRequestDto>>(requests)
            );
        }

        public async Task<ExceptionRequestDto> CreateAsync(CreateExceptionRequestDto input)
        {
            await CheckPolicyAsync(AttendanceManagementPermissions.ExceptionRequests.Create);

            // Get current employee
            var employee = await _employeeRepository
                .FirstOrDefaultAsync(e => e.UserId == _currentUser.Id.Value);

            if (employee == null)
            {
                throw new UserFriendlyException("Employee record not found for current user.");
            }

            if (!employee.WorkflowId.HasValue)
            {
                throw new UserFriendlyException("No workflow assigned to this employee.");
            }

            // Validate exception date is for a scheduled on-site day
            var currentSchedule = employee.ScheduleAssignments
                .Where(sa => sa.EffectiveFrom <= DateTime.Now
                    && (sa.EffectiveTo == null || sa.EffectiveTo >= DateTime.Now))
                .FirstOrDefault();

            if (currentSchedule == null)
            {
                throw new UserFriendlyException("You don't have an active schedule.");
            }

            // Load schedule with days
            var scheduleQueryable = await _exceptionRequestRepository.GetQueryableAsync();
            var scheduleDay = await scheduleQueryable
                .Where(er => er.EmployeeId == employee.Id)
                .SelectMany(er => er.Employee.ScheduleAssignments)
                .Where(sa => sa.Id == currentSchedule.Id)
                .SelectMany(sa => sa.Schedule.ScheduleDays)
                .FirstOrDefaultAsync(sd => sd.DayOfWeek == input.ExceptionDate.DayOfWeek);

            if (scheduleDay == null || !scheduleDay.IsOnSite)
            {
                throw new UserFriendlyException(
                    "Exception requests can only be submitted for scheduled on-site days.");
            }

            var exceptionRequest = new ExceptionRequest(
                GuidGenerator.Create(),
                employee.Id,
                input.ExceptionDate,
                input.Reason,
                input.Type,
                employee.WorkflowId.Value
            );

            await _exceptionRequestRepository.InsertAsync(exceptionRequest);

            return ObjectMapper.Map<ExceptionRequest, ExceptionRequestDto>(exceptionRequest);
        }

        public async Task<ExceptionRequestDto> ApproveOrRejectAsync(ApproveRejectExceptionRequestDto input)
        {
            await CheckPolicyAsync(AttendanceManagementPermissions.ExceptionRequests.Approve);

            var queryable = await _exceptionRequestRepository.WithDetailsAsync(
                er => er.Workflow.WorkflowSteps,
                er => er.ApprovalHistories);

            var request = await queryable.FirstOrDefaultAsync(er => er.Id == input.ExceptionRequestId);
            if (request == null) {
                throw new UserFriendlyException("Exception request not found.");
            }

            if (request.Status != ExceptionRequestStatus.Pending)
            {
                throw new UserFriendlyException("This request has already been processed.");
            }

            // Get current employee
            var currentEmployee = await _employeeRepository
                .FirstOrDefaultAsync(e => e.UserId == _currentUser.Id.Value);

            // Get current workflow step
            var currentStep = request.Workflow.WorkflowSteps
                .FirstOrDefault(ws => ws.StepOrder == request.CurrentStepOrder);

            if (currentStep == null)
            {
                throw new UserFriendlyException("Invalid workflow step.");
            }

            if (currentEmployee == null)
            {
                throw new UserFriendlyException("Employee record not found for current user.");
            }

            // Verify approver
            if (currentStep.ApproverEmployeeId != currentEmployee.Id)
            {
                throw new UserFriendlyException("You are not authorized to approve this request at this step.");
            }

            // Create approval history
            var history = new ExceptionRequestApprovalHistory(
                GuidGenerator.Create(),
                request.Id,
                currentStep.Id,
                currentEmployee.Id,
                currentStep.StepOrder,
                input.Action,
                input.Notes
            );

            request.ApprovalHistories.Add(history);

            if (input.Action == ApprovalAction.Approved)
            {
                // Check if there are more steps
                var nextStep = request.Workflow.WorkflowSteps
                    .FirstOrDefault(ws => ws.StepOrder == request.CurrentStepOrder + 1);

                if (nextStep != null)
                {
                    request.Approve(currentStep.StepOrder);
                }
                else
                {
                    request.FinalApprove();
                }
            }
            else if (input.Action == ApprovalAction.Rejected)
            {
                request.Reject();
            }

            await _exceptionRequestRepository.UpdateAsync(request);

            return ObjectMapper.Map<ExceptionRequest, ExceptionRequestDto>(request);
        }

        public async Task<List<ExceptionRequestDto>> GetMyRequestsAsync()
        {
            await CheckPolicyAsync(AttendanceManagementPermissions.ExceptionRequests.ViewOwn);

            var employee = await _employeeRepository
                .FirstOrDefaultAsync(e => e.UserId == _currentUser.Id.Value);

            if (employee == null)
            {
                _logger.LogDebug("employee with id: " + _currentUser.Id + "NOT Found");
                throw new UserFriendlyException("Employee record not found for current user.");
            }
            else
            {
                _logger.LogDebug("employee with name: " + employee.Id + "Found");
            }
              

            var requests = await _exceptionRequestRepository.GetExceptionRequestsByEmployeeId(employee.Id);
            _logger.LogDebug("requests: " + requests);
            return ObjectMapper.Map<List<ExceptionRequest>, List<ExceptionRequestDto>>(requests);
        }

        public async Task<List<ExceptionRequestDto>> GetPendingApprovalsAsync()
        {
            await CheckPolicyAsync(AttendanceManagementPermissions.ExceptionRequests.Approve);

            var currentEmployee = await _employeeRepository
                .FirstOrDefaultAsync(e => e.UserId == _currentUser.Id.Value);

            var queryable = await _exceptionRequestRepository.WithDetailsAsync(
                er => er.Employee,
                er => er.Workflow.WorkflowSteps);

            var allPendingRequests = await queryable
                .Where(er => er.Status == ExceptionRequestStatus.Pending)
                .ToListAsync();

            // Filter requests where current employee is the approver at current step
            var myPendingRequests = allPendingRequests
                .Where(er => er.Workflow.WorkflowSteps
                    .Any(ws => ws.StepOrder == er.CurrentStepOrder
                        && ws.ApproverEmployeeId == currentEmployee.Id))
                .ToList();

            return ObjectMapper.Map<List<ExceptionRequest>, List<ExceptionRequestDto>>(myPendingRequests);
        }

        public async Task CancelAsync(Guid id)
        {
            await CheckPolicyAsync(AttendanceManagementPermissions.ExceptionRequests.Delete);

            var request = await _exceptionRequestRepository.GetAsync(id);

            if (request.Status != ExceptionRequestStatus.Pending)
            {
                throw new UserFriendlyException("Only pending requests can be cancelled.");
            }

            request.Status = ExceptionRequestStatus.Cancelled;
            await _exceptionRequestRepository.UpdateAsync(request);
        }

        public async Task<Guid> UploadAttachmentAsync(
            Guid exceptionRequestId,
            IRemoteStreamContent file,
            AttachmentType attachmentType)
        {
            var request = await _exceptionRequestRepository.GetAsync(exceptionRequestId);

            // Ensure fileName is not null
            var fileName = file.FileName ?? throw new UserFriendlyException("File name cannot be null.");
            var filePath = $"attachments/{exceptionRequestId}/{fileName}";

            // TODO: Implement actual file saving logic

            var attachment = new ExceptionRequestAttachment(
                GuidGenerator.Create(),
                exceptionRequestId,
                fileName,
                filePath,
                file.ContentType,
                attachmentType
            );

            request.Attachments.Add(attachment);
            await _exceptionRequestRepository.UpdateAsync(request);

            return attachment.Id;
        }
    }
}
