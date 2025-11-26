using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using AttendanceManagement.Enums;

namespace AttendanceManagement.Data.ExceptionRequests
{
    public class ExceptionRequestAttachment : CreationAuditedEntity<Guid>
    {
        public Guid ExceptionRequestId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string ContentType { get; set; }
        public AttachmentType AttachmentType { get; set; }

        // Navigation properties
        public virtual ExceptionRequest ExceptionRequest { get; set; }

        protected ExceptionRequestAttachment() { }

        public ExceptionRequestAttachment(
            Guid id,
            Guid exceptionRequestId,
            string fileName,
            string filePath,
            string contentType,
            AttachmentType attachmentType) : base(id)
        {
            ExceptionRequestId = exceptionRequestId;
            FileName = fileName;
            FilePath = filePath;
            ContentType = contentType;
            AttachmentType = attachmentType;
        }
    }
}
