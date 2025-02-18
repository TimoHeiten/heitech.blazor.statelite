using heitech.blazor.statelite;
using Microsoft.Extensions.DependencyInjection;

namespace ClassLibrary1;

public static class Configuration
{
    /// <summary>
    /// Add the state lite InMemory db as a Singleton to the service collection
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <returns></returns>
    public static IServiceCollection AddStateLite<TKey>(this IServiceCollection serviceCollection)
        where TKey : IEquatable<TKey>
        => serviceCollection.AddSingleton<IStateLite<TKey>, StateLiteCore<TKey>>();
}