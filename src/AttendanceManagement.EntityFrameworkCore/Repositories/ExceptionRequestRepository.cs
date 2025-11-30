using AttendanceManagement.Data.ExceptionRequests;
using AttendanceManagement.Data.Schedules;
using AttendanceManagement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace AttendanceManagement.Repositories
{
    public class ExceptionRequestRepository(IDbContextProvider<AttendanceManagementDbContext> dbContextProvider) : EfCoreRepository<AttendanceManagementDbContext, ExceptionRequest, Guid>(dbContextProvider), IExceptionRequestRepository
    {
        public async Task<List<ExceptionRequest>> GetExceptionRequestsByEmployeeId(Guid guid)
        {
            var query = await GetQueryableAsync();
            return await query
                .Where(er => er.EmployeeId == guid)
                .Include(er => er.Workflow)
                .Include(er => er.ApprovalHistories)
                .Include(er => er.Attachments)
                .OrderByDescending(er => er.CreationTime)
                .ToListAsync();
        }
    }
}
