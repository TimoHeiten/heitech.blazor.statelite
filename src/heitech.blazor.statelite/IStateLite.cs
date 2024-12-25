using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace heitech.blazor.statelite
{
    /// <summary>
    /// Creates a store by name - allows for multiple simultaneously
    /// </summary>
    public static class StateLiteFactory
    {
        private static readonly Dictionary<string, IStateLite> Stores = new Dictionary<string, IStateLite>();
        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);
        /// <summary>
        /// Creates a singleton instance of the IStateLite store
        /// </summary>
        /// <returns></returns>
        public static IStateLite Get(string name)
        {
            Semaphore.Wait();
            try
            {
                if (Stores.TryGetValue(name, out var store1))
                {
                    return store1;
                }

                var store = new StateLiteCore();
                Stores.Add(name, store);
                return store;
            }
            finally
            {
                Semaphore.Release();
            }
        }
    }

    /// <summary>
    /// LiteDb based in memory store
    /// </summary>
    public interface IStateLite
    {
        /// <summary>
        /// the underlying memory stream for the Database 
        /// </summary>
        MemoryStream DatabaseStream { get; }
        /// <summary>
        /// Get all records of a type - no query applied
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> GetAll<T>()
            where T : IHasId;

        /// <summary>
        /// Find by id. Can be null.
        /// </summary>
        /// <param name="id"></param>
        /// <typeparam name="T"></typeparam>
        T GetById<T>(Guid id)
            where T : IHasId;
        
        /// <summary>
        /// Find by filter expression. Result can be empty 
        /// </summary>
        /// <param name="filter"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> Query<T>(Func<T, bool> filter)
            where T : IHasId;

        /// <summary>
        /// Replaces the record
        /// </summary>
        /// <param name="record"></param>
        /// <typeparam name="T"></typeparam>
        void Update<T>(T record)
            where T : IHasId;

        /// <summary>
        /// Inserts a record
        /// </summary>
        /// <param name="record"></param>
        /// <typeparam name="T"></typeparam>
        void Insert<T>(T record)
            where T : IHasId;
        
        /// <summary>
        /// Remove a record (if exists)
        /// </summary>
        /// <param name="record"></param>
        /// <typeparam name="T"></typeparam>
        void Delete<T>(T record)
            where T : IHasId;

        /// <summary>
        /// Debugger help for dumping the database to an output
        /// </summary>
        /// <param name="writerCallback"></param>
        void Dump(Action<string> writerCallback);
    }
}