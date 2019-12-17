using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections;

namespace InitVent.Metadata
{
    public interface IMetadataCollection<T> : IQueryable<T>
    //where T : IMetadataObject
    {
        //T GetMetadataObject(Guid id);

        T CreateObject();

        void Save(T obj);
        void SaveAll(IEnumerable<T> objs);
        void Delete(T obj);
        void DeleteAll(IEnumerable<T> objs);
    }

    public class MetadataCollection<T> : IMetadataCollection<T>
    {
        protected IMetadataLoader<T> Loader { get; private set; }

        public MetadataCollection(IMetadataLoader<T> metadataLoader)
        {
            Loader = metadataLoader;
        }

        public T CreateObject()
        {
            return Loader.CreateObject();
        }

        public void Save(T obj)
        {
            Loader.Save(obj);
        }

        public void SaveAll(IEnumerable<T> objs)
        {
            Loader.SaveAll(objs);
        }

        //public void Update(T obj)
        //{
        //    Loader.Update(obj);
        //}

        //public void SaveOrUpdate(T obj)
        //{
        //    Loader.SaveOrUpdate(obj);
        //}

        public void Delete(T obj)
        {
            Loader.Delete(obj);
        }

        public void DeleteAll(IEnumerable<T> objs)
        {
            Loader.DeleteAll(objs);
        }

        #region IQueryable Members

        Type IQueryable.ElementType
        {
            get { return Loader.All.ElementType; }
        }

        Expression IQueryable.Expression
        {
            get { return Loader.All.Expression; }
        }

        IQueryProvider IQueryable.Provider
        {
            get { return Loader.All.Provider; }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return Loader.All.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Loader.All.GetEnumerator();
        }

        #endregion
    }
}
