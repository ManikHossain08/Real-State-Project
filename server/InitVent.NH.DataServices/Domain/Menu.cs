using System;
using Iesi.Collections.Generic;

namespace InitVent.DataServices.Domain
{
    public class Menu : iMFASEntity<Menu>
    {
        public virtual Guid Id { get; set; }
        public virtual string MenuName { get; set; }
        public virtual string SerialNo { get; set; }
        public virtual Int32 Sequence { get; set; }
        public virtual Guid ModuleId { get; set; }
        public virtual ISet<SubMenu> SubMenus { get; set; }
    }
}
