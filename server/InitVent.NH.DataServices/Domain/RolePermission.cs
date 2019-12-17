using System;
namespace InitVent.DataServices.Domain
{
    public class RolePermission : iMFASEntity<RolePermission>
    {
        public virtual Guid Id { get; set; }
        public virtual Guid RoleId { get; set; }
        public virtual Guid PermissionId { get; set; }
    }
}
