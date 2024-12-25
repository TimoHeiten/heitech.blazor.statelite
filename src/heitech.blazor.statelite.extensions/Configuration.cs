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
    public static IServiceCollection AddStateLite(this IServiceCollection serviceCollection)
        => serviceCollection.AddSingleton<IStateLite, StateLiteCore>();
}