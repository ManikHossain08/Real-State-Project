using System;
using Iesi.Collections.Generic;
namespace InitVent.DataServices.Domain
{
    public class SubMenu : iMFASEntity<SubMenu>
    {
        public virtual Guid Id { get; set; }
        public virtual string SubMenuName { get; set; }
        public virtual string PluginName { get; set; }
        public virtual string SerialNo { get; set; }
        public virtual Guid MenuId { get; set; }
        public virtual ISet<Item> Items { get; set; }
    }
}
