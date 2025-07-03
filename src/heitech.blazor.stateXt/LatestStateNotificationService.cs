using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace heitech.blazor.stateXt
{
    /// <summary>
    /// StateService that allows to Produce (set) and Consume (listen to changes) 
    /// </summary>
    public abstract class LatestStateNotificationService<T> : IConsumeState<T>, IProduceState<T>
    {
        private T _value;
        private readonly List<Subscriber> _subscribers = new List<Subscriber>();

        public T Value => _value;

        protected LatestStateNotificationService(T initialValue = default!)
            => _value = initialValue;

        public async Task SetValue(T newValue)
        {
            var isSameValue = EqualityComparer<T>.Default.Equals(_value, newValue);
            if (isSameValue)
                return;
             
            _value = newValue;
            await NotifySubscribersAsync();
        }

        public async Task SubscribeAsync(Callback<T> callback, object receiver)
        {
            var subscriber = new Subscriber(callback, receiver);
            _subscribers.Add(subscriber);

            if (callback.HasDelegate)
                await callback.InvokeAsync(_value);
        }

        public void Unsubscribe(object receiver)
            => _subscribers.RemoveAll(s => s.Receiver == receiver);

        private async Task NotifySubscribersAsync()
        {
            var tasks = _subscribers.Where(x => x.Callback.HasDelegate).Select(x => x.Callback.InvokeAsync(_value));
            await Task.WhenAll(tasks);
        }

        public Task SubscribeAsync(Action<T> action, object receiver)
            => SubscribeAsync(Callback<T>.Subscription(action, receiver), receiver);

        public Task SubscribeAsync(Func<T, Task> asyncAction, object receiver)
            => SubscribeAsync(Callback<T>.AsyncSubscription(asyncAction, receiver), receiver);

        private sealed class Subscriber
        {
            public Subscriber(Callback<T> callback, object receiver)
            {
                Callback = callback;
                Receiver = receiver;
            }
            
            public object Receiver { get; }
            public Callback<T> Callback { get; }
        }
    }
}