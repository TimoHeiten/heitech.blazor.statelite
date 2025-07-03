using System;
using System.Threading.Tasks;

namespace heitech.blazor.stateXt
{
    /// <summary>
    /// Eventcallback semamtics w/o blazor lib dependencies
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public readonly struct Callback<T>
    {
        public object Receiver { get; }
        public static Callback<T> Subscription(Action<T> func, object origin) => new Callback<T>(func, origin);
        public static Callback<T> AsyncSubscription(Func<T, Task> func, object origin) => new Callback<T>(func, origin);
        
        private readonly Action<T>? _sync;
        private readonly Func<T, Task>? _async;

        public Callback(Action<T> sync, object receiver)
        {
            Receiver = receiver;
            _sync = sync;
            _async = null;
        }

        public Callback(Func<T, Task> async, object receiver)
        {
            _async = async;
            Receiver = receiver;
            _sync = null;
        }

        public Task InvokeAsync(T arg)
        {
            if (_async != null)
                return _async(arg);

            _sync?.Invoke(arg);
            return Task.CompletedTask;
        }

        public bool HasDelegate => _async != null || _sync != null;
    }
}