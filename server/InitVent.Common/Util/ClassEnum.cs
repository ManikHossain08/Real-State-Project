using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace InitVent.Common.Util
{
    /// <summary>
    /// Provides a base for creating class enumerations (instead of the built-in struct enumerations).
    /// </summary>
    /// <typeparam name="TClassEnum">The type of the enumeration (i.e. the class that is extending this class).</typeparam>
    public abstract class ClassEnum<TClassEnum>
        where TClassEnum : ClassEnum<TClassEnum>
    {
        private static readonly IDictionary<String, Lazy<TClassEnum>> ValuesByName;
        private static readonly IDictionary<String, Lazy<TClassEnum>> ValuesByNameIgnoringCase;

        public static TClassEnum[] Values
        {
            get
            {
                return ValuesByName.Values.Select(e => e.Value).ToArray();
            }
        }

        static ClassEnum()
        {
            var staticFields = typeof(TClassEnum).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

            ValuesByName = staticFields
                .Where(field => field.FieldType == typeof(TClassEnum))
                .ToDictionary(field => field.Name, field => new Lazy<TClassEnum>(() => (TClassEnum)field.GetValue(null), LazyThreadSafetyMode.ExecutionAndPublication));

            ValuesByNameIgnoringCase = new Dictionary<String, Lazy<TClassEnum>>(ValuesByName, StringComparer.OrdinalIgnoreCase);
        }

        public static TClassEnum Parse(String name, bool ignoreCase = false)
        {
            var source = ignoreCase ? ValuesByNameIgnoringCase : ValuesByName;

            Lazy<TClassEnum> result;
            if (source.TryGetValue(name, out result))
                return result.Value;

            throw new ArgumentException("Provided name did not match an existing value.", name);
        }

        private readonly Lazy<String> LazyName;
        public String Name { get { return LazyName.Value; } }

        public ClassEnum()
        {
            LazyName = new Lazy<String>(() => ValuesByName.Single(kvp => kvp.Value.Value == this).Key, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public override string ToString()
        {
            return Name;
        }
    }

}
