
﻿using System;
﻿using Iesi.Collections.Generic;
﻿using InitVent.DataServices.Domain;

namespace InitVent.DataServices.Domain
{
    public class User : iMFASEntity<User>
    {
        public virtual Guid Id { get; set; }
        public virtual string UserId { get; set; }
        public virtual string UserPassword { get; set; }
        public virtual string UserName { get; set; }
        public virtual bool Status { get; set; }        
        public virtual Role UserRole { get; set; }
        public virtual string Mobile { get; set; }
        public virtual string Email { get; set; }
        public virtual string Address { get; set; }
        public virtual Guid? Project_Id { get; set; }
    }
}

