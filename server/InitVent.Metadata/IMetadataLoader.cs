using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Collections;

namespace InitVent.Metadata
{
    public interface IMetadataLoader : IDisposable
    {
        //System.IO.TextWriter Log { get; set; }

        IMetadataLoader<T> GetMetadataLoader<T>()
            where T : class;

        void Refresh(Enum refreshMode, object entity);

        void Refresh(Enum refreshMode, IEnumerable collection);
    }

    public interface IMetadataLoader<T> : IDisposable
    {
        IQueryable<T> All { get; }

        T CreateObject();

        void Save(T obj);
        void SaveAll(IEnumerable<T> objs);
        //void Update(T obj);
        //void SaveOrUpdate(T obj);
        void Delete(T obj);
        void DeleteAll(IEnumerable<T> objs);
    }

}
