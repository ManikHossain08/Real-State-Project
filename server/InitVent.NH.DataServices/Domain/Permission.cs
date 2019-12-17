using System;
namespace InitVent.DataServices.Domain
{
    public class Permission : iMFASEntity<Permission>
    {
        public virtual Guid Id { get; set; }
        public virtual string PermissionName { get; set; }
        public virtual string SerialNo { get; set; }
        public virtual Guid ItemId { get; set; }
        public virtual Int32 Code { get; set; }
        public virtual string ActionName { get; set; }
    }
}
