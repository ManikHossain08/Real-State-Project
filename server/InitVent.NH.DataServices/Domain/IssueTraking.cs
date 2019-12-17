
﻿using System;
﻿using Iesi.Collections.Generic;
﻿using InitVent.DataServices.Domain;
using System.Collections.Generic;

namespace InitVent.DataServices.Domain
{
    public class IssueTracking : iMFASEntity<IssueTracking>
    {
        public virtual Guid Id { get; set; }
        public virtual string Priority { get; set; }
        public virtual string IsssueTitle { get; set; }
        public virtual string IssueDescription { get; set; }
        public virtual string IssueType { get; set; }
        public virtual Guid? Assignee { get; set; }       
        public virtual string IssueNo { get; set; }
        public virtual string IssueStatus { get; set; }
        public virtual string ResolutionArea { get; set; }
        public virtual string Severity { get; set; }
        public virtual Guid? Project { get; set; }
        public virtual Guid? Creator { get; set; }
        public virtual DateTime? CreationDate { get; set; }
        public virtual Guid? Modifier { get; set; }
        public virtual DateTime? ModificationDate { get; set; }
        public virtual byte[] IssuePhoto { get; set; }
        public virtual List<IssueAttachment> AttachmentFiles { get; set; }        
    }
}
