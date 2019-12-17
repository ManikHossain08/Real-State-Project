namespace InitVent.DataServices.Domain
{
    using System;
    using InitVent.ORM;

    public class iMFASEntity<T> : PersistentObject<T>
    {
        public iMFASEntity() 
        {
            // The first part of string is the full static class name for the data services that will be used to access all the domain objects in imfas application.
            // The second part is the name of the assembly that contains this data services class.
            base.DataSerivcesClassName = "InitVent.DataServices.NHibernate.Repository.IssueTrackerDataServices, InitVent.NH.DataServices";
        }
    }
}
