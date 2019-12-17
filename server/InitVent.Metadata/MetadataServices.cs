using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Collections;

namespace InitVent.Metadata
{
    public class MetadataServices : IDisposable
    {
        protected IMetadataLoader MetadataLoader { get; private set; }

        public MetadataServices(IMetadataLoader metadataLoader)
        {
            MetadataLoader = metadataLoader;
        }

        //public IMetadataObject GetMetadataObject(Guid id, Type type)
        //{
        //    return MetadataLoader.GetMetadataObject(id, type);
        //}

        //public T GetMetadataObject<T>(Guid id)
        //    where T : IMetadataObject
        //{
        //    return (T)GetMetadataObject(id, typeof(T));
        //}

        //public IMetadataObject GetMetadataObject(Guid id, MetadataObjectType metadataObjectType)
        //{
        //    var type = MetadataObjectTypeHandler.GetType(metadataObjectType);

        //    return GetMetadataObject(id, type);
        //}

        //public void SaveMetadataObject(IMetadataObject obj)
        //{
        //    MetadataLoader.SaveMetadataObject(obj);
        //}

        public virtual IMetadataCollection<T> GetMetadataCollection<T>()
            where T : class
        {
            return CreateMetadataCollection(MetadataLoader.GetMetadataLoader<T>());
        }

        protected virtual IMetadataCollection<T> CreateMetadataCollection<T>(IMetadataLoader<T> loader)
        {
            return new MetadataCollection<T>(loader);
        }

        /// <summary>
        /// This method refreshes the current data context
        /// </summary>
        /// <param name="refreshMode">the refresh mode</param>
        /// <param name="entity">the entity for which to refresh the data context</param>
        public void Refresh(Enum refreshMode, object entity)
        {
            MetadataLoader.Refresh(refreshMode, entity);
        }

        /// <summary>
        /// This method refreshes the current data context
        /// </summary>
        /// <param name="refreshMode">the refresh mode</param>
        /// <param name="collection">the entity collection for which to refresh the data context</param>
        public void Refresh(Enum refreshMode, IEnumerable collection)
        {
            MetadataLoader.Refresh(refreshMode, collection);
        }

        public void Dispose()
        {
            MetadataLoader.Dispose();
        }
    }
}
