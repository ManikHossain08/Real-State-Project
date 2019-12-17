using System;
using System.Collections.Generic;
namespace InitVent.DataServices.Domain
{
    public class IssueAttachment : iMFASEntity<IssueAttachment>
    {

        public virtual Guid Id { get; set; }
        public virtual Guid IssueId { get; set; }
        public virtual DateTime? AttachmentDate { get; set; }
        public virtual string FileLocation { get; set; }

    }
}
