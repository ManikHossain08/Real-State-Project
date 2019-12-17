using System;
using Iesi.Collections.Generic;

namespace InitVent.DataServices.Domain
{
    public class Module : iMFASEntity<Module>
    {
        public virtual Guid Id { get; set; }
        public virtual string ModuleName { get; set; }
        public virtual string SerialNo { get; set; }
        public virtual ISet<Menu> Menus { get; set; }
    }
}
