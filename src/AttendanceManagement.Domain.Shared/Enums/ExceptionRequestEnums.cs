using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceManagement.Enums
{
    public enum ExceptionRequestStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2,
        Cancelled = 3
    }

    public enum ExceptionRequestType
    {
        Sick = 0,
        SpecialCondition = 1
    }

    public enum AttachmentType
    {
        MedicalDocument = 0,
        SupportingDocument = 1,
        Other = 2
    }

    public enum ApprovalAction
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }

    public enum ApproverType
    {
        Manager = 0,
        HRManager = 1,
        Doctor = 2
    }
}
