using nRoute.Services;
using System;

namespace nRoute.Components.Messaging
{
    public interface IChannel<T>
        : ISubject<T>
    {
        void OnNext(T value, bool asynchronously);

        void OnNext(T value, IDispatcher dispatcher);

        void OnError(Exception exception, bool asynchronously);

        IDisposable Subscribe(IObserver<T> subscriber, ThreadOption threadOption);

        IDisposable Subscribe(IObserver<T> subscriber, bool useWeakReference);

        IDisposable Subscribe(IObserver<T> subscriber, ThreadOption threadOption, bool useWeakReference);
    }
}
