
﻿using System;
﻿using Iesi.Collections.Generic;
﻿using InitVent.DataServices.Domain;

namespace InitVent.DataServices.Domain
{
    public class UserProject : iMFASEntity<UserProject>
    {
        public virtual Guid Id { get; set; }
        public virtual Guid UserId { get; set; }        
        public virtual Guid ProjectId { get; set; }
    }
}

