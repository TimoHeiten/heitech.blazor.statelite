namespace heitech.blazor.statelite.repositories;

/// <summary>
/// Subscribe to changes of managed store
/// </summary>
/// <typeparam name="TKey"></typeparam>
public interface ISubscriber<TKey>
{
    /// <summary>
    /// Will be invoked for the type T when changes occur (delete, upsert)
    /// </summary>
    /// <param name="items"></param>
    /// <typeparam name="T"></typeparam>
    void OnChanges<T>(IEnumerable<T> items)
        where T : IHasId<TKey>, new();
} 