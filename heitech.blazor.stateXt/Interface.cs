using System;
using System.Threading.Tasks;

namespace heitech.blazor.stateXt
{
    /// <summary>
    /// Consume state of type T - register via subscribe to listen to changes. Unsub on dispose
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IConsumeState<out T>
    {
        /// <summary>
        /// Subscribe synrhonous changes
        /// </summary>
        /// <param name="action"></param>
        /// <param name="receiver"></param>
        /// <returns></returns>
        Task SubscribeAsync(Action<T> action, object receiver);
        
        /// <summary>
        /// Subscribe to async changes
        /// </summary>
        /// <param name="asyncAction"></param>
        /// <param name="receiver"></param>
        /// <returns></returns>
        Task SubscribeAsync(Func<T, Task> asyncAction, object receiver);
        
        /// <summary>
        /// Remove this receiver from receiving updates 
        /// </summary>
        /// <param name="receiver"></param>
        void Unsubscribe(object receiver);
    }

    /// <summary>
    /// Produce aka set new state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IProduceState<in T>
    {
        /// <summary>
        /// Set a new value for all Consumers to consume
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns></returns>
        Task SetValue(T newValue);
    }
}