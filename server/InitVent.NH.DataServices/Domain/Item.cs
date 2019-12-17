using System;
using Iesi.Collections.Generic;
namespace InitVent.DataServices.Domain
{
    public class Item : iMFASEntity<Item>
    {
        public virtual Guid Id { get; set; }
        public virtual string ItemName { get; set; }
        public virtual string SerialNo { get; set; }
        public virtual Guid SubMenuId { get; set; }
        public virtual ISet<Permission> Permissions { get; set; }
    }
}
