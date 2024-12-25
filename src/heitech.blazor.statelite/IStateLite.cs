using System;
using System.Collections.Generic;
using System.IO;

namespace heitech.blazor.statelite
{
    /// <summary>
    /// LiteDb based in memory store
    /// </summary>
    public interface IStateLite : IDisposable
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

        /// <summary>
        /// Clears all data from this store - effectively a reset - not a dispose
        /// </summary>
        void Purge();
    }
}