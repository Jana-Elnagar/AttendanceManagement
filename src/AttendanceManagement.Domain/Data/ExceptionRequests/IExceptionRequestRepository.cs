using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace AttendanceManagement.Data.ExceptionRequests
{
    public interface IExceptionRequestRepository : IRepository<ExceptionRequest, Guid>
    {
        public Task<List<ExceptionRequest>> GetExceptionRequestsByEmployeeId(Guid guid);
    }
}
