using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Collections;

namespace InitVent.Metadata.Ado
{
    public class LinqToSqlMetadataLoader : IMetadataLoader
    {
        protected DataContext DataContext { get; private set; }

        public LinqToSqlMetadataLoader(DbConnection dbConnection)
        {
            DataContext = new DataContext(dbConnection);
            //DataContext.Log = new InitVent.Common.IO.DelegatedTextWriter(str => System.Diagnostics.Debug.Write(str));
        }

        public virtual IMetadataLoader<T> GetMetadataLoader<T>()
            where T : class
        {
            return new LinqToSqlMetadataLoader<T>(DataContext);
        }

        public void Dispose()
        {
            DataContext.Dispose();
        }

        public void Refresh(Enum refreshMode, object entity)
        {
            DataContext.Refresh((RefreshMode)refreshMode, entity);
        }

        public void Refresh(Enum refreshMode,IEnumerable collection)
        {
            DataContext.Refresh((RefreshMode)refreshMode, collection);
        }
    }

    public class LinqToSqlMetadataLoader<T> : IMetadataLoader<T>
        where T : class
    {
        protected Table<T> DataTable { get; private set; }

        public LinqToSqlMetadataLoader(DataContext dataContext)
        {
            DataTable = dataContext.GetTable<T>();
        }

        public IQueryable<T> All
        {
            get { return DataTable; }
        }

        public T CreateObject()
        {
            return Activator.CreateInstance<T>();
        }

        public void Save(T obj)
        {
            DataTable.InsertOnSubmit(obj);
            DataTable.Context.SubmitChanges();
        }

        public void SaveAll(IEnumerable<T> objs)
        {
            DataTable.InsertAllOnSubmit(objs);
            DataTable.Context.SubmitChanges();
        }

        public void Delete(T obj)
        {
            DataTable.DeleteOnSubmit(obj);
            DataTable.Context.SubmitChanges();
        }

        public void DeleteAll(IEnumerable<T> objs)
        {
            DataTable.DeleteAllOnSubmit(objs);
            DataTable.Context.SubmitChanges();
        }

        public void Dispose()
        {
            DataTable.Context.Dispose();
        }
    }
}
