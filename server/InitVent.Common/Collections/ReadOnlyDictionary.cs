using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitVent.Common.Collections
{
    public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> BaseDictionary;

        public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
        {
            BaseDictionary = dictionary;
        }

        #region IDictionary Members

        public void Add(TKey key, TValue value)
        {
            throw new NotSupportedException("This dictionary is read-only");
        }

        public bool ContainsKey(TKey key)
        {
            return BaseDictionary.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get { return BaseDictionary.Keys; }
        }

        public bool Remove(TKey key)
        {
            throw new NotSupportedException("This dictionary is read-only");
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return BaseDictionary.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get { return BaseDictionary.Values; }
        }

        public TValue this[TKey key]
        {
            get
            {
                return BaseDictionary[key];
            }
            set
            {
                throw new NotSupportedException("This dictionary is read-only");
            }
        }

        #endregion

        #region ICollection Members

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException("This dictionary is read-only");
        }

        public void Clear()
        {
            throw new NotSupportedException("This dictionary is read-only");
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return BaseDictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            BaseDictionary.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return BaseDictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException("This dictionary is read-only");
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return BaseDictionary.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }

}
