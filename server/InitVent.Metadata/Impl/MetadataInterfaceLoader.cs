using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Collections;

namespace InitVent.Metadata.Impl
{
    public class MetadataInterfaceLoader : DynamicMetadataLoader
    {
        protected IMetadataLoader ConcreteLoader { get; private set; }

        public MetadataInterfaceLoader(IMetadataLoader concreteLoader)
        {
            ConcreteLoader = concreteLoader;
        }

        public void AssignMapping<TInterface, TConcrete>()
            where TConcrete : class, TInterface
        {
            SetMetadataLoader(new MetadataInterfaceLoader<TInterface, TConcrete>(ConcreteLoader.GetMetadataLoader<TConcrete>()));
        }

        public void AssignMapping<TInterface, TConcrete>(Func<TInterface, TConcrete> interfaceToConcreteConverter)
            where TConcrete : class, TInterface
        {
            SetMetadataLoader(new MetadataInterfaceLoader<TInterface, TConcrete>(ConcreteLoader.GetMetadataLoader<TConcrete>(), interfaceToConcreteConverter));
        }

        public override void Refresh(Enum refreshMode, object entity)
        {
            ConcreteLoader.Refresh(refreshMode, entity);
        }

        public override void Refresh(Enum refreshMode, IEnumerable collection)
        {
            ConcreteLoader.Refresh(refreshMode, collection);
        }
    }

    /// <typeparam name="TMappingScope">The scope to which the mappings apply.</typeparam>
    /// <remarks>
    /// Note that use of the MappingScope type parameter allows there to exist
    /// several independent, yet statically-accessible, mappings.
    /// </remarks>
    public class StaticMappingMetadataInterfaceLoader<TMappingScope> : IMetadataLoader
    {
        private static readonly IDictionary<Type, Object> MetadataLoaderGenerators = new Dictionary<Type, Object>();

        public static void AssignMapping<TInterface, TConcrete>()
            where TConcrete : class, TInterface
        {
            SetMetadataLoaderGenerator(loader => new MetadataInterfaceLoader<TInterface, TConcrete>(loader.ConcreteLoader.GetMetadataLoader<TConcrete>()));
        }

        public static void AssignMapping<TInterface, TConcrete>(Func<TInterface, TConcrete> interfaceToConcreteConverter)
            where TConcrete : class, TInterface
        {
            SetMetadataLoaderGenerator(loader => new MetadataInterfaceLoader<TInterface, TConcrete>(loader.ConcreteLoader.GetMetadataLoader<TConcrete>(), interfaceToConcreteConverter));
        }

        private static void SetMetadataLoaderGenerator<T>(Func<StaticMappingMetadataInterfaceLoader<TMappingScope>, IMetadataLoader<T>> loaderGenerator)
        {
            MetadataLoaderGenerators[typeof(T)] = loaderGenerator;
        }

        protected IMetadataLoader ConcreteLoader { get; private set; }

        public StaticMappingMetadataInterfaceLoader(IMetadataLoader concreteLoader)
        {
            ConcreteLoader = concreteLoader;
        }

        public virtual IMetadataLoader<T> GetMetadataLoader<T>()
            where T : class
        {
            var type = typeof(T);
            if (!MetadataLoaderGenerators.ContainsKey(type))
                throw new InvalidOperationException("No loader registered for type: " + type);

            var generator = (Func<StaticMappingMetadataInterfaceLoader<TMappingScope>, IMetadataLoader<T>>)MetadataLoaderGenerators[type];
            return generator(this);
        }

        public void Dispose()
        {
            ConcreteLoader.Dispose();
        }

        public void Refresh(Enum refreshMode, object entity)
        {
            ConcreteLoader.Refresh(refreshMode, entity);
        }

        public void Refresh(Enum refreshMode, IEnumerable collection)
        {
            ConcreteLoader.Refresh(refreshMode, collection);
        }
    }

    public class MetadataInterfaceLoader<TInterface, TConcrete> : IMetadataLoader<TInterface>
        where TConcrete : class, TInterface
    {
        protected IMetadataLoader<TConcrete> ConcreteLoader { get; private set; }
        protected Func<TInterface, TConcrete> InterfaceToConcrete { get; private set; }

        public MetadataInterfaceLoader(IMetadataLoader<TConcrete> concreteLoader)
            : this(concreteLoader, obj => (TConcrete)obj)
        {
            ;
        }

        public MetadataInterfaceLoader(IMetadataLoader<TConcrete> concreteLoader, Func<TInterface, TConcrete> interfaceToConcreteConverter)
        {
            ConcreteLoader = concreteLoader;
            InterfaceToConcrete = interfaceToConcreteConverter;
        }

        public IQueryable<TInterface> All
        {
            get { return ConcreteLoader.All; }
        }

        public TInterface CreateObject()
        {
            return ConcreteLoader.CreateObject();
        }

        public void Save(TInterface obj)
        {
            ConcreteLoader.Save(InterfaceToConcrete(obj));
        }

        public void SaveAll(IEnumerable<TInterface> objs)
        {
            ConcreteLoader.SaveAll(objs.Select(InterfaceToConcrete));
        }

        public void Delete(TInterface obj)
        {
            ConcreteLoader.Delete(InterfaceToConcrete(obj));
        }

        public void DeleteAll(IEnumerable<TInterface> objs)
        {
            ConcreteLoader.DeleteAll(objs.Select(InterfaceToConcrete));
        }

        public void Dispose()
        {
            ConcreteLoader.Dispose();
        }
    }
}
