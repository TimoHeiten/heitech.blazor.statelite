namespace heitech.blazor.statelite.repositories;

internal sealed class MangedStore<TKey>
    where TKey : IEquatable<TKey>
{
    public IStateLite<TKey> Store { get; }
    private readonly List<ISubscriber<TKey>> _subscribers = new();

    public MangedStore(string storeName)
        => Store = StateLiteFactory.Get<TKey>(storeName);

    public void Subscribe(ISubscriber<TKey> subscriber)
    {
        if (_subscribers.Any(x => ReferenceEquals(x, subscriber)))
            return;

        _subscribers.Add(subscriber);
    }
    
    public void Unsubscribe(ISubscriber<TKey> subscriber)
        => _subscribers.Remove(subscriber);

    public IEnumerable<T> CurrentItems<T>()
        where T : IHasId<TKey>, new()
    => Store.GetAll<T>();

    public async Task UpsertItems<T>(Func<Task<IEnumerable<T>>> getItems)
        where T : IHasId<TKey>, new()
    {
        var items = await getItems();
        Do(items, Store.Replace);
    }

    public async Task DeleteItems<T>(Func<Task> delete, IEnumerable<T> records)
        where T : IHasId<TKey>, new()
    {
        await delete();
        Do(records, Store.Delete);
    }

    public async Task DeleteItemsByKey<T>(Func<Task> delete, IEnumerable<TKey> keys)
        where T : IHasId<TKey>, new()
    {
        await delete();
        keys.ToList().ForEach(Store.Delete);
        NotifyChanges<T>();
    }

    private void Do<T>(IEnumerable<T> records, Action<T> action)
        where T: IHasId<TKey>, new()
    {
        records.ToList().ForEach(action);
        NotifyChanges<T>();
    }

    private void NotifyChanges<T>()
        where T : IHasId<TKey>, new()
    {
        var refreshed = CurrentItems<T>();
        _subscribers.ForEach(s => s.OnChanges(refreshed));
    }
}