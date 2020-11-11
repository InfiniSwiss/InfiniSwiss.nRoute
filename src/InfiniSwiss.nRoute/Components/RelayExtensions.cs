using nRoute.Components.Messaging;
using nRoute.Internal;
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace nRoute.Components
{
    public static class RelayExtensions
    {
        // ValueConverters Relay Related

        public static IValueConverter SetRelayConverter(this FrameworkElement target, Object resourceKey,
            IValueConverter converter)
        {
            Guard.ArgumentNotNull(target, "target");
            Guard.ArgumentNotNull(resourceKey, "resourceKey");
            ((ValueConverterRelay)target.Resources[resourceKey]).Converter = converter;
            return converter;
        }

        public static ValueConverter<TIn, TOut> SetRelayConverter<TIn, TOut>(this FrameworkElement target, Object resourceKey,
            Func<TIn, TOut> forwardConverter)
        {
            Guard.ArgumentNotNull(target, "target");
            Guard.ArgumentNotNull(resourceKey, "resourceKey");
            var _converter = new ValueConverter<TIn, TOut>(forwardConverter);
            ((ValueConverterRelay)target.Resources[resourceKey]).Converter = _converter;
            return _converter;
        }

        public static ValueConverter<TIn, TOut> SetRelayConverter<TIn, TOut>(this FrameworkElement target, Object resourceKey,
            Func<TIn, TOut> forwardConverter, Func<TOut, TIn> reverseConverter)
        {
            Guard.ArgumentNotNull(target, "target");
            Guard.ArgumentNotNull(resourceKey, "resourceKey");
            var _converter = new ValueConverter<TIn, TOut>(forwardConverter, reverseConverter);
            ((ValueConverterRelay)target.Resources[resourceKey]).Converter = _converter;
            return _converter;
        }

        // Commands Relays Related

        public static ICommand SetRelayCommand(this FrameworkElement target, Object resourceKey, ICommand command)
        {
            Guard.ArgumentNotNull(target, "target");
            Guard.ArgumentNotNull(resourceKey, "resourceKey");
            ((CommandRelay)target.Resources[resourceKey]).Command = command;
            return command;
        }

        public static ActionCommand<T> SetRelayCommand<T>(this FrameworkElement target, Object resourceKey, Action<T> executeHandler)
        {
            Guard.ArgumentNotNull(target, "target");
            Guard.ArgumentNotNull(resourceKey, "resourceKey");
            var _command = new ActionCommand<T>(executeHandler);
            ((CommandRelay)target.Resources[resourceKey]).Command = _command;
            return _command;
        }

        public static ActionCommand<T> SetRelayCommand<T>(this FrameworkElement target, Object resourceKey,
            Action<T> executeHandler, Func<T, bool> canExecuteHandler)
        {
            Guard.ArgumentNotNull(target, "target");
            Guard.ArgumentNotNull(resourceKey, "resourceKey");
            var _command = new ActionCommand<T>(executeHandler, canExecuteHandler);
            ((CommandRelay)target.Resources[resourceKey]).Command = _command;
            return _command;
        }

        // Value Relays Related

        public static T SetRelayValue<T>(this FrameworkElement target, Object resourceKey, T value)
        {
            Guard.ArgumentNotNull(target, "target");
            Guard.ArgumentNotNull(resourceKey, "resourceKey");
            ((ValueRelay)target.Resources[resourceKey]).Value = value;
            return value;
        }

        public static object SetRelayValue(this FrameworkElement target, Object resourceKey, object value)
        {
            Guard.ArgumentNotNull(target, "target");
            Guard.ArgumentNotNull(resourceKey, "resourceKey");
            ((ValueRelay)target.Resources[resourceKey]).Value = value;
            return value;
        }

        // Observers Related

        public static IDisposable Subscribe<T>(this IObservable<T> source)
        {
            return Subscribe<T>(source, (v) => { }, null, null);
        }

        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext)
        {
            return Subscribe<T>(source, onNext, null, null);
        }

        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action<Exception> onError)
        {
            return Subscribe<T>(source, onNext, onError, null);
        }

        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action onCompleted)
        {
            return Subscribe<T>(source, onNext, null, onCompleted);
        }

        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            Guard.ArgumentNotNull(source, "source");
            return source.Subscribe(new RelayObserver<T>(onNext, onError, onCompleted));
        }

        // IChannel - specially for IChannels

        public static IDisposable Subscribe<T>(this IChannel<T> source, Action<T> onNext, bool useWeakReference)
        {
            return Subscribe(source, onNext, null, ThreadOption.PublisherThread, useWeakReference);
        }

        public static IDisposable Subscribe<T>(this IChannel<T> source, Action<T> onNext, ThreadOption threadOption)
        {
            return Subscribe(source, onNext, null, threadOption, false);
        }

        public static IDisposable Subscribe<T>(this IChannel<T> source, Action<T> onNext, Action<Exception> onError, bool useWeakReference)
        {
            return Subscribe(source, onNext, onError, ThreadOption.PublisherThread, useWeakReference);
        }

        public static IDisposable Subscribe<T>(this IChannel<T> source, Action<T> onNext, Action<Exception> onError, ThreadOption threadOption)
        {
            return Subscribe(source, onNext, onError, threadOption, false);
        }

        public static IDisposable Subscribe<T>(this IChannel<T> source, Action<T> onNext, Action<Exception> onError, ThreadOption threadOption,
            bool useWeakReference)
        {
            Guard.ArgumentNotNull(source, "source");
            return source.Subscribe(new RelayObserver<T>(onNext, onError), threadOption, useWeakReference);
        }
    }
}
