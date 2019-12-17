
﻿using System;
using InitVent.DataServices.Domain;

namespace InitVent.DataServices.Domain
{
    public class Role : iMFASEntity<Role>
    {
        public virtual Guid Id { get; set; }
        public virtual string RoleName { get; set; }
        public virtual string RoleDescription { get; set; }
        public virtual bool Status { get; set; }          
    }
}

