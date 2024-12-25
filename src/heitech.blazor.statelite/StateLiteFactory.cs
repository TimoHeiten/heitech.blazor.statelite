using System.Collections.Generic;
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

}