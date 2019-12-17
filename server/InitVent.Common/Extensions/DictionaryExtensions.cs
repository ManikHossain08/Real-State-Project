using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InitVent.Common.Collections;

namespace InitVent.Common.Extensions
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Adds all the elements of the given dictionary into the current one, overwriting
        /// values for keys that already exist.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
        /// <param name="dictionary">The base dictionary.</param>
        /// <param name="additional">The dictionary of additional key-value pairs.</param>
        public static void AddAll<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IDictionary<TKey, TValue> additional)
        {
            foreach (var kvp in additional)
            {
                dictionary[kvp.Key] = kvp.Value;
            }
        }

        /// <summary>
        /// Adds all the elements of the given dictionary into the current one, ignoring
        /// values for keys that already exist.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
        /// <param name="dictionary">The base dictionary.</param>
        /// <param name="additional">The dictionary of additional key-value pairs.</param>
        public static void AddMissing<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IDictionary<TKey, TValue> additional)
        {
            foreach (var kvp in additional)
            {
                if (!dictionary.ContainsKey(kvp.Key))
                {
                    dictionary[kvp.Key] = kvp.Value;
                }
            }
        }

        /// <summary>
        /// Combines the elements of the given dictionary with the current one, overwriting
        /// values for keys that already exist.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
        /// <param name="dictionary">The base dictionary.</param>
        /// <param name="additional">The dictionary of additional key-value pairs.</param>
        /// <returns>A new dictionary containing the elements of both dictionaries.</returns>
        public static IDictionary<TKey, TValue> JoinAll<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IDictionary<TKey, TValue> additional)
        {
            var compositeDictionary = new Dictionary<TKey, TValue>(dictionary);
            AddAll(compositeDictionary, additional);
            return compositeDictionary;
        }

        /// <summary>
        /// Combines the elements of the given dictionary with the current one, ignoring
        /// values for keys that already exist.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
        /// <param name="dictionary">The base dictionary.</param>
        /// <param name="additional">The dictionary of additional key-value pairs.</param>
        /// <returns>A new dictionary containing the elements of both dictionaries.</returns>
        public static IDictionary<TKey, TValue> JoinMissing<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IDictionary<TKey, TValue> additional)
        {
            return JoinAll(additional, dictionary);
        }

        /// <summary>
        /// Creates a read-only wrapper around the given dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
        /// <param name="dictionary">The base dictionary.</param>
        /// <returns>A read-only wrapper around the given dictionary.</returns>
        public static IDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            return new ReadOnlyDictionary<TKey, TValue>(dictionary);
        }

        /// <summary>
        /// Creates a new dictionary that uses the given default value when
        /// a key is not found (per the TryGetValue() method).
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
        /// <param name="dictionary">The base dictionary.</param>
        /// <param name="defaultValue">The value to use as default.</param>
        /// <returns>A new dictionary with default.</returns>
        [Obsolete("Use DictionaryExtensions.WithDefaultValue() instead.")]
        public static IDictionary<TKey, TValue> SetDefaultValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TValue defaultValue)
        {
            return WithDefaultValue(dictionary, defaultValue);
        }

        /// <summary>
        /// Creates a special dictionary that always yields a value for all
        /// keys, using the given default value when a key does not have an
        /// explict value.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
        /// <param name="dictionary">The base dictionary.</param>
        /// <param name="defaultValue">The value to use as default.</param>
        /// <returns>A new dictionary with default.</returns>
        [Obsolete("Use DictionaryExtensions.WithImplicitDefaultValue() instead.")]
        public static IDictionary<TKey, TValue> SetImplicitDefaultValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TValue defaultValue)
        {
            return WithImplicitDefaultValue(dictionary, defaultValue);
        }

        /// <summary>
        /// Creates a new dictionary that uses the given default value when
        /// a key is not found (per the TryGetValue() method).
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
        /// <param name="dictionary">The base dictionary.</param>
        /// <param name="defaultValue">The value to use as default.</param>
        /// <returns>A new dictionary with default.</returns>
        public static IDictionary<TKey, TValue> WithDefaultValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TValue defaultValue)
        {
            return new DictionaryWithDefault<TKey, TValue>(dictionary, defaultValue, false);
        }

        /// <summary>
        /// Creates a special dictionary that always yields a value for all
        /// keys, using the given default value when a key does not have an
        /// explict value.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
        /// <param name="dictionary">The base dictionary.</param>
        /// <param name="defaultValue">The value to use as default.</param>
        /// <returns>A new dictionary with default.</returns>
        public static IDictionary<TKey, TValue> WithImplicitDefaultValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TValue defaultValue)
        {
            return new DictionaryWithDefault<TKey, TValue>(dictionary, defaultValue, true);
        }

        /// <summary>
        /// Gets the value associated with the specified key, or the dictionary's default value if it is not found.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
        /// <param name="dictionary">The dictionary to access.</param>
        /// <param name="key">The key whose value to get.</param>
        /// <returns>The element with the specified key, or the default value generated by the dictionary.</returns>
        /// <remarks>
        /// For backwards-compatibility, this method allows a null dictionary, in which case it returns default(TValue).
        /// However, this use is discouraged.
        /// </remarks>
        public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary == null)
                return default(TValue);

            TValue result;
            dictionary.TryGetValue(key, out result);
            return result;
        }

        /// <summary>
        /// Gets the value associated with the specified key, or the specified default value if it is not found.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
        /// <param name="dictionary">The dictionary to access.</param>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="defaultValue">The default element value to use.</param>
        /// <returns>The element with the specified key, or the specified default value.</returns>
        public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            TValue result;
            if (dictionary.TryGetValue(key, out result))
                return result;

            return defaultValue;
        }

        /// <summary>
        /// Inserts the given value into the collection associated with the given key in the given
        /// dictionary, using the default constructor to create the collection if it does not already
        /// exist.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TCollection">The type of value collections in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary's collections.</typeparam>
        /// <param name="dictionary">The dictionary to add to.</param>
        /// <param name="key">The key identifying the target collection.</param>
        /// <param name="value">The value to insert into the dictionary's matching collection.</param>
        public static void AddValueElement<TKey, TCollection, TValue>(this IDictionary<TKey, TCollection> dictionary, TKey key, TValue value)
            where TCollection : ICollection<TValue>, new()
        {
            AddValueElement(dictionary, key, value, () => new TCollection());
        }

        /// <summary>
        /// Inserts the given value into the collection associated with the given key in the given
        /// dictionary, using the given constructor to create the collection if it does not already
        /// exist.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TCollection">The type of value collections in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary's collections.</typeparam>
        /// <param name="dictionary">The dictionary to add to.</param>
        /// <param name="key">The key identifying the target collection.</param>
        /// <param name="value">The value to insert into the dictionary's matching collection.</param>
        /// <param name="constructor">The constructor to use when the key is not already present in the given dictionary.</param>
        public static void AddValueElement<TKey, TCollection, TValue>(this IDictionary<TKey, TCollection> dictionary, TKey key, TValue value, Func<TCollection> constructor)
            where TCollection : ICollection<TValue>
        {
            TCollection storedValues;

            if (!dictionary.TryGetValue(key, out storedValues))
            {
                storedValues = constructor();
                dictionary.Add(key, storedValues);
            }

            storedValues.Add(value);
        }
    }

    internal class DictionaryWithDefault<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private IDictionary<TKey, TValue> BaseDictionary { get; set; }

        public bool IsDefaultImplicit { get; private set; }
        public TValue DefaultValue { get; private set; }

        public DictionaryWithDefault(IDictionary<TKey, TValue> baseDictionary, TValue defaultValue, bool implicitDefault)
        {
            if (baseDictionary == null)
            {
                throw new ArgumentNullException("Base dictionary cannot be null.", "baseDictionary");
            }

            BaseDictionary = new Dictionary<TKey, TValue>(baseDictionary);
            DefaultValue = defaultValue;
            IsDefaultImplicit = implicitDefault;
        }

        #region IDictionary methods

        public ICollection<TKey> Keys
        {
            get { return BaseDictionary.Keys; }
        }
        public ICollection<TValue> Values
        {
            get { return BaseDictionary.Values; }
        }
        public int Count
        {
            get { return BaseDictionary.Count; }
        }
        public bool IsReadOnly
        {
            get { return BaseDictionary.IsReadOnly; }
        }

        public TValue this[TKey key]
        {
            get
            {
                if (IsDefaultImplicit)
                {
                    TValue value;
                    bool found = BaseDictionary.TryGetValue(key, out value);
                    return found ? value : DefaultValue;
                }

                return BaseDictionary[key];
            }
            set
            {
                BaseDictionary[key] = value;
            }
        }

        public void Add(TKey key, TValue value)
        {
            BaseDictionary.Add(key, value);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            BaseDictionary.Add(item);
        }

        public bool ContainsKey(TKey key)
        {
            return BaseDictionary.ContainsKey(key);
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return BaseDictionary.Contains(item);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            bool found = BaseDictionary.TryGetValue(key, out value);
            if (!found)
            {
                value = DefaultValue;
            }

            return found;
        }

        public bool Remove(TKey key)
        {
            return BaseDictionary.Remove(key);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return BaseDictionary.Remove(item);
        }

        public void Clear()
        {
            BaseDictionary.Clear();
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            BaseDictionary.CopyTo(array, arrayIndex);
        }

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
