using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace heitech.blazor.stateXt
{
    /// <summary>
    /// StateService that allows to Produce (set) and Consume (listen to changes) 
    /// </summary>
    public abstract class LatestStateNotificationService<T> : IConsumeState<T>, IProduceState<T>
    {
        private T _value;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private List<Subscriber> _subscribers = new List<Subscriber>();

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
            await _semaphore.WaitAsync();
            try
            {
                var subscriber = new Subscriber(callback, receiver);
                _subscribers.Add(subscriber);
            }
            finally 
            {
                _semaphore.Release();
            }

            if (callback.HasDelegate)
                await callback.InvokeAsync(_value);
        }

        public void Unsubscribe(object receiver)
        {
            _semaphore.Wait();
            try
            {
                _subscribers.RemoveAll(x => x.Receiver == receiver); 
            }
            finally
            {
                _semaphore.Release();
            } 
        }

        private async Task NotifySubscribersAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                var tasks = _subscribers.ToList().Where(x => x.Callback.HasDelegate).Select(x => x.Callback.InvokeAsync(_value));
                await Task.WhenAll(tasks);
            }
            finally 
            {
                _semaphore.Release();
            }
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