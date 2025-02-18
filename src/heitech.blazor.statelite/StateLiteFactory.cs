using System;
using System.Collections.Generic;
using System.Threading;

namespace heitech.blazor.statelite
{
    /// <summary>
    /// Creates a store by name - allows for multiple simultaneously
    /// </summary>
    public static class StateLiteFactory
    {
        private static readonly Dictionary<string, object> Stores = new Dictionary<string, object>();
        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);
        /// <summary>
        /// Creates a singleton instance of the IStateLite store
        /// </summary>
        /// <returns></returns>
        public static IStateLite<TKey> Get<TKey>(string name)
            where TKey : IEquatable<TKey>
        {
            Semaphore.Wait();
            try
            {
                if (Stores.TryGetValue(name, out var store1))
                {
                    if (store1 is IStateLite<TKey> stateLite)
                        return stateLite;
                    throw new KeyNotFoundException($"for identifier {name} is no matching store with key {typeof(TKey)} available - found {store1.GetType().GetGenericArguments()[0].Name} instead");
                }

                var store = new StateLiteCore<TKey>();
                Stores.Add(name, store);
                return store;
            }
            finally
            {
                Semaphore.Release();
            }
        }
    }

}