using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Linq;
using System.Text;

namespace InitVent.Metadata.Ado
{
    public class AdoMetadataLoader : IMetadataLoader
    {
        protected ObjectContext DataContext { get; private set; }

        public AdoMetadataLoader(EntityConnection dbConnection)
        {
            DataContext = new ObjectContext(dbConnection);
            DataContext.ContextOptions.LazyLoadingEnabled = true;
        }

        public virtual IMetadataLoader<T> GetMetadataLoader<T>()
            where T : class
        {
            return new AdoMetadataLoader<T>(DataContext);
        }

        public void Dispose()
        {
            DataContext.Dispose();
        }

        public void Refresh(Enum refreshMode, object entity)
        {
            DataContext.Refresh((RefreshMode)refreshMode, entity);
        }


        public void Refresh(Enum refreshMode, System.Collections.IEnumerable collection)
        {
            DataContext.Refresh((RefreshMode)refreshMode, collection);
        }
    }

    public class AdoMetadataLoader<T> : IMetadataLoader<T>
        where T : class
    {
        protected ObjectSet<T> AdoTable { get; private set; }

        public AdoMetadataLoader(ObjectContext dataContext)
        {
            AdoTable = dataContext.CreateObjectSet<T>();
        }

        public IQueryable<T> All
        {
            get { return AdoTable; }
        }

        public T CreateObject()
        {
            return AdoTable.CreateObject();
        }

        protected void EnsureAttached(T obj)
        {
            ObjectStateEntry entry;
            if (AdoTable.Context.ObjectStateManager.TryGetObjectStateEntry(obj, out entry))
            {
                if (entry.State == EntityState.Detached)
                    AdoTable.Attach(obj);
            }
            else
            {
                AdoTable.AddObject(obj);
            }
        }

        public void Save(T obj)
        {
            EnsureAttached(obj);
            AdoTable.Context.SaveChanges();
        }

        public void SaveAll(IEnumerable<T> objs)
        {
            foreach (var obj in objs)
                EnsureAttached(obj);

            AdoTable.Context.SaveChanges();
        }

        public void Delete(T obj)
        {
            AdoTable.DeleteObject(obj);
            AdoTable.Context.SaveChanges();
        }

        public void DeleteAll(IEnumerable<T> objs)
        {
            foreach (var obj in objs)
            {
                AdoTable.DeleteObject(obj);
            }
            AdoTable.Context.SaveChanges();
        }

        public void Dispose()
        {
            AdoTable.Context.Dispose();
        }
    }
}
