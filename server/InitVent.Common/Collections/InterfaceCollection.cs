using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitVent.Common.Collections
{

    public class InterfaceCollection<TConcrete, TInterface> : ICollection<TInterface>
        where TConcrete : TInterface
    {
        protected ICollection<TConcrete> ConcreteCollection { get; private set; }
        protected Func<TInterface, TConcrete> InterfaceToConcrete { get; private set; }

        public InterfaceCollection()
            : this(obj => (TConcrete)obj)
        {
            ;
        }

        public InterfaceCollection(Func<TInterface, TConcrete> interfaceToConcreteConverter)
            : this(new List<TConcrete>())
        {

        }

        public InterfaceCollection(ICollection<TConcrete> concreteCollection)
            : this(concreteCollection, obj => (TConcrete)obj)
        {
            ;
        }

        public InterfaceCollection(ICollection<TConcrete> concreteCollection, Func<TInterface, TConcrete> interfaceToConcreteConverter)
        {
            ConcreteCollection = concreteCollection;
            InterfaceToConcrete = interfaceToConcreteConverter;
        }

        public void Add(TInterface item)
        {
            ConcreteCollection.Add(InterfaceToConcrete(item));
        }

        public void Clear()
        {
            ConcreteCollection.Clear();
        }

        public bool Contains(TInterface item)
        {
            return ConcreteCollection.Contains(InterfaceToConcrete(item));
        }

        public void CopyTo(TInterface[] array, int arrayIndex)
        {
            ConcreteCollection.Cast<TInterface>().ToList().CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return ConcreteCollection.Count; }
        }

        public bool IsReadOnly
        {
            get { return ConcreteCollection.IsReadOnly; }
        }

        public bool Remove(TInterface item)
        {
            return ConcreteCollection.Remove(InterfaceToConcrete(item));
        }

        public IEnumerator<TInterface> GetEnumerator()
        {
            return ConcreteCollection.Cast<TInterface>().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ConcreteCollection.GetEnumerator();
        }
    }
}
