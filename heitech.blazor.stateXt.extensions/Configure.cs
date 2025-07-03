// ReSharper disable once CheckNamespace

using heitech.blazor.stateXt;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ConfigureBlazorStateXt
    {
        /// <summary>
        /// Configure Consume and Produce for your services
        /// </summary>
        /// <param name="services"></param>
        /// <typeparam name="TStateService"></typeparam>
        /// <typeparam name="TState"></typeparam>
        public static void RegisterState<TStateService, TState>(this IServiceCollection services)
            where TStateService : class, IProduceState<TState?>, IConsumeState<TState>
        {
            services.AddSingleton<TStateService>();
            services.AddTransient<IProduceState<TState?>>(sp => sp.GetRequiredService<TStateService>());
            services.AddTransient<IConsumeState<TState?>>(sp => sp.GetRequiredService<TStateService>());
        }
    }
}