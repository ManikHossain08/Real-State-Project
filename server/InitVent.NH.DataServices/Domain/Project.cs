using System;
namespace InitVent.DataServices.Domain
{
    public class Project : iMFASEntity<Project>
    {
        public Project() { }
        public virtual Guid Id { get; set; }
        public virtual string ProjectCode { get; set; }
        public virtual string ProjectName { get; set; }
        public virtual DateTime? StartDate { get; set; }
    }
}
