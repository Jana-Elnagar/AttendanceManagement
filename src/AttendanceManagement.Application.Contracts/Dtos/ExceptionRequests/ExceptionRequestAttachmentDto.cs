using AttendanceManagement.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace AttendanceManagement.Dtos.ExceptionRequests
{
    public class ExceptionRequestAttachmentDto : CreationAuditedEntityDto<Guid>
    {
        public Guid ExceptionRequestId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string ContentType { get; set; }
        public AttachmentType AttachmentType { get; set; }
    }
}
