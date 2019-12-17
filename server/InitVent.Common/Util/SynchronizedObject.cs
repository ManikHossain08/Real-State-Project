using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace InitVent.Common.Util
{
    /// <summary>
    /// Provides thread-safe access to an object that is synchronized to some external source (e.g. a file).
    /// </summary>
    /// <typeparam name="T">The type of the synchronized object.</typeparam>
    public abstract class SynchronizedObject<T>
        where T : class
    {
        /// <summary>
        /// The object used to synchronize access to the instance.
        /// </summary>
        protected readonly Object SyncRoot = new Object();

        private volatile T theInstance;

        /// <summary>
        /// The in-memory, synchronized instance of the object.
        /// </summary>
        /// <remarks>
        /// Accessing this property will cause the object to be loaded from its source if it has
        /// not been already, or if it is in need of a refresh as determined by the
        /// <seealso cref="NeedsRefresh()"/> method.
        /// </remarks>
        public T Instance
        {
            get
            {
                if (theInstance == null || NeedsRefresh())
                {
                    lock (SyncRoot)
                    {
                        if (theInstance == null || NeedsRefresh())
                        {
                            LastUpdate = DateTime.Now;
                            theInstance = LoadObject();
                        }
                    }
                }

                return theInstance;
            }
        }

        /// <summary>
        /// The last time the object was synchronized to its source (or <code>null</code> if never).
        /// </summary>
        public DateTime? LastUpdate { get; private set; }

        /// <summary>
        /// Forces a refresh of the object.
        /// </summary>
        public void Refresh()
        {
            var x = Instance;
            lock (SyncRoot)
            {
                LastUpdate = DateTime.Now;
                theInstance = LoadObject();
            }
        }

        /// <summary>
        /// Loads the object from its source.
        /// </summary>
        /// <returns>The in-memory representation of the object.</returns>
        protected abstract T LoadObject();

        /// <summary>
        /// Checks whether the object needs to be reloaded from its source before any further
        /// access to it is permitted (e.g. if the source has been updated).
        /// </summary>
        /// <returns>True if the object must be reloaded; false otherwise.</returns>
        /// <remarks>
        /// This method is called everytime the object is accessed through this class.  If,
        /// for some implementation, performing the complete check is too expensive to run
        /// each time, then the appropriate scheduling should be handled within this method.
        /// </remarks>
        protected abstract bool NeedsRefresh();
    }

    /// <summary>
    /// Provides thread-safe access to an object that is synchronized to a file.
    /// </summary>
    /// <typeparam name="T">The type of the synchronized object.</typeparam>
    public abstract class FileSynchronizedObject<T> : SynchronizedObject<T>
        where T : class
    {
        /// <summary>
        /// The file to which the object instance is synchronized.
        /// </summary>
        protected FileInfo ObjectDefinitionFile { get; private set; }

        public FileSynchronizedObject(FileInfo objectDefinitionFile)
        {
            ObjectDefinitionFile = objectDefinitionFile;
        }

        protected override bool NeedsRefresh()
        {
            ObjectDefinitionFile.Refresh();
            return ObjectDefinitionFile.LastWriteTime > LastUpdate;
        }
    }
}
