using AttendanceManagement.Data.ExceptionRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace AttendanceManagement.Data.Notifications
{
    public class Notification : CreationAuditedEntity<Guid>
    {
        public Guid UserId { get; set; }
        public Guid? RelatedExceptionRequestId { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }

        // Navigation properties
        public virtual ExceptionRequest RelatedExceptionRequest { get; set; }

        protected Notification() { }

        public Notification(
            Guid id,
            Guid userId,
            string message,
            Guid? relatedExceptionRequestId = null) : base(id)
        {
            UserId = userId;
            Message = message;
            RelatedExceptionRequestId = relatedExceptionRequestId;
            IsRead = false;
        }

        public void MarkAsRead()
        {
            IsRead = true;
        }
    }
}
