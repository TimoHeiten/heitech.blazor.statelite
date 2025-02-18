namespace heitech.blazor.statelite.repositories;

/// <summary>
/// Manage the state with an underlying litedb store
/// </summary>
public interface IManagedState<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Get Access to the underlying store directly for queries and such
    /// </summary>
    IStateLite<TKey> Store { get; }
    /// <summary>
    /// Subscribe to the state changes
    /// </summary>
    /// <param name="subscriber"></param>
    void Subscribe(ISubscriber<TKey> subscriber);
    /// <summary>
    /// Unsubscribe - uses reference equality
    /// </summary>
    /// <param name="subscriber"></param>
    void Unsubscribe(ISubscriber<TKey> subscriber);

    /// <summary>
    /// All current items of the specified type available in the store
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    IEnumerable<T> CurrentItems<T>()
        where T : IHasId<TKey>, new();

    /// <summary>
    /// Upsert the items returned by the getItems fn
    /// </summary>
    /// <param name="getItems"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task UpsertItems<T>(Func<Task<IEnumerable<T>>> getItems)
        where T : IHasId<TKey>, new();

    /// <summary>
    /// Delete the records after the delete fn has been executed
    /// </summary>
    /// <param name="delete"></param>
    /// <param name="records"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task DeleteItems<T>(Func<Task> delete, IEnumerable<T> records)
        where T : IHasId<TKey>, new();

    /// <summary>
    /// Delte the records via its keys
    /// </summary>
    /// <param name="delete"></param>
    /// <param name="keys"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task DeleteItemsByKey<T>(Func<Task> delete, IEnumerable<TKey> keys)
        where T : IHasId<TKey>, new();
}

/// <summary>
/// Entry point to the managed state
/// </summary>
public static class EnterState
{
    /// <summary>
    /// Create Managed State with an underlying store addressable with the _storeName_
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="_storeName_">The identifier for the created store</param>
    /// <returns></returns>
    public static IManagedState<TKey> Go<TKey>(string _storeName_)
        where TKey : IEquatable<TKey>
    {
        return new MangedState<TKey>(_storeName_);
    }
}