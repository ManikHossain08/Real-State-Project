using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Collections;

namespace InitVent.Metadata.Impl
{
    public class DynamicMetadataLoader : IMetadataLoader
    {
        private readonly IDictionary<Type, Object> MetadataLoaders = new Dictionary<Type, Object>();

        public virtual IMetadataLoader<T> GetMetadataLoader<T>()
            where T : class
        {
            var type = typeof(T);
            if (!MetadataLoaders.ContainsKey(type))
                throw new InvalidOperationException("No loader registered for type: " + type);

            return (IMetadataLoader<T>)MetadataLoaders[type];
        }

        public void SetMetadataLoader<T>(IMetadataLoader<T> loader)
        {
            MetadataLoaders[typeof(T)] = loader;
        }

        public void Dispose()
        {
            foreach (var loader in MetadataLoaders.Values.OfType<IDisposable>())
            {
                loader.Dispose();
            }
        }

        public virtual void Refresh(Enum refreshMode, object entity)
        {
            //There are no context here so nothing to refresh
        }


        public virtual void Refresh(Enum refreshMode,IEnumerable collection)
        {
            //There are no context here so nothing to refresh
        }
    }

    //[Obsolete]
    //public class SingleInstanceDynamicMetadataLoader<TScope> : IMetadataLoader
    //{
    //    private static readonly DynamicMetadataLoader DynamicLoader = new DynamicMetadataLoader();

    //    public IMetadataLoader<T> GetMetadataLoader<T>()
    //        where T : class
    //    {
    //        return DynamicLoader.GetMetadataLoader<T>();
    //    }

    //    public static void SetMetadataLoader<T>(IMetadataLoader<T> loader)
    //    {
    //        DynamicLoader.SetMetadataLoader<T>(loader);
    //    }
    //}

    //[Obsolete]
    //public class DelayedDynamicMetadataLoader : IMetadataLoader
    //{
    //    private readonly IDictionary<Type, Object> MetadataLoaderGenerators = new Dictionary<Type, Object>();

    //    public virtual IMetadataLoader<T> GetMetadataLoader<T>()
    //        where T : class
    //    {
    //        var generator = (Func<IMetadataLoader<T>>)MetadataLoaderGenerators[typeof(T)];
    //        return generator();
    //    }

    //    public void SetMetadataLoaderGenerator<T>(Func<IMetadataLoader<T>> loaderGenerator)
    //    {
    //        MetadataLoaderGenerators[typeof(T)] = loaderGenerator;
    //    }
    //}
}
