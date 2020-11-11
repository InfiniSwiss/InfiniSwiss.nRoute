using nRoute.Internal;
using nRoute.Services;
using System;
using System.Collections.Generic;

namespace nRoute.Components.Messaging
{
    public abstract class Channel
    {

        #region Declarations

        private readonly static Dictionary<Type, ChannelInvoker> _invokers;
        private readonly static Type _invokerGenericType = typeof(ChannelInvoker<Object>).GetGenericTypeDefinition();
        private readonly static Object _lock = new Object();

        #endregion

        static Channel()
        {
            _invokers = new Dictionary<Type, ChannelInvoker>();
        }

        internal Channel() { }

        #region Publish Related 

        public static void Publish(Type channelType, Object payload)
        {
            Guard.ArgumentNotNull(channelType, "channelType");

            ChannelInvoker _invoker = GetChannelInvoker(channelType);
            _invoker.Publish(payload, false);
        }

        public static void Publish(Type channelType, string key, Object payload)
        {
            Guard.ArgumentNotNull(channelType, "channelType");

            ChannelInvoker _invoker = GetChannelInvoker(channelType);
            _invoker.Publish(key, payload, false);
        }

        public static void Publish(Type channelType, Object payload, bool asynchronously)
        {
            Guard.ArgumentNotNull(channelType, "channelType");

            ChannelInvoker _invoker = GetChannelInvoker(channelType);
            _invoker.Publish(payload, asynchronously);
        }

        public static void Publish(Type channelType, string key, Object payload, bool asynchronously)
        {
            Guard.ArgumentNotNull(channelType, "channelType");

            ChannelInvoker _invoker = GetChannelInvoker(channelType);
            _invoker.Publish(key, payload, asynchronously);
        }

        public static void Publish<T>(T payload)
        {
            Channel<T>.Public.OnNext(payload);
        }

        public static void Publish<T>(T payload, IDispatcher dispatcher)
        {
            Channel<T>.Public.OnNext(payload, dispatcher);
        }

        public static void Publish<T>(string key, T payload)
        {
            Channel<T>.Private[key].OnNext(payload, false);
        }

        public static void Publish<T>(T payload, bool asynchronously)
        {
            Channel<T>.Public.OnNext(payload, asynchronously);
        }

        public static void Publish<T>(string key, T payload, bool asynchronously)
        {
            Channel<T>.Private[key].OnNext(payload, asynchronously);
        }

        #endregion

        #region Publish Error Related 

        public static void PublishError(Type channelType, Exception error)
        {
            Guard.ArgumentNotNull(channelType, "channelType");

            ChannelInvoker _invoker = GetChannelInvoker(channelType);
            _invoker.PublishError(error, false);
        }

        public static void PublishError(Type channelType, string key, Exception error)
        {
            Guard.ArgumentNotNull(channelType, "channelType");

            ChannelInvoker _invoker = GetChannelInvoker(channelType);
            _invoker.PublishError(key, error, false);
        }

        public static void PublishError(Type channelType, Exception error, bool asynchronously)
        {
            Guard.ArgumentNotNull(channelType, "channelType");

            ChannelInvoker _invoker = GetChannelInvoker(channelType);
            _invoker.PublishError(error, asynchronously);
        }

        public static void PublishError(Type channelType, string key, Exception error, bool asynchronously)
        {
            Guard.ArgumentNotNull(channelType, "channelType");

            ChannelInvoker _invoker = GetChannelInvoker(channelType);
            _invoker.PublishError(key, error, asynchronously);
        }

        public static void PublishError<T>(Exception error)
        {
            Channel<T>.Public.OnError(error);
        }

        public static void PublishError<T>(string key, Exception error)
        {
            Channel<T>.Private[key].OnError(error, false);
        }

        public static void PublishError<T>(Exception error, bool asynchronously)
        {
            Channel<T>.Public.OnError(error, asynchronously);
        }

        public static void PublishError<T>(string key, Exception error, bool asynchronously)
        {
            Channel<T>.Private[key].OnError(error, asynchronously);
        }

        #endregion

        #region Subscription Related

        public static IDisposable Subscribe(Type channelType, Object subscriber)
        {
            return Subscribe(channelType, subscriber, ThreadOption.PublisherThread, false);
        }

        public static IDisposable Subscribe(Type channelType, string key, Object subscriber)
        {
            return Subscribe(channelType, key, subscriber, ThreadOption.PublisherThread, false);
        }

        public static IDisposable Subscribe(Type channelType, Object subscriber, bool useWeakReference)
        {
            return Subscribe(channelType, subscriber, ThreadOption.PublisherThread, useWeakReference);
        }

        public static IDisposable Subscribe(Type channelType, string key, Object subscriber, bool useWeakReference)
        {
            return Subscribe(channelType, key, subscriber, ThreadOption.PublisherThread, useWeakReference);
        }

        public static IDisposable Subscribe(Type channelType, Object subscriber, ThreadOption threadOption)
        {
            return Subscribe(channelType, subscriber, threadOption, false);
        }

        public static IDisposable Subscribe(Type channelType, string key, Object subscriber, ThreadOption threadOption)
        {
            return Subscribe(channelType, key, subscriber, threadOption, false);
        }

        public static IDisposable Subscribe(Type channelType, Object subscriber, ThreadOption threadOption, bool useWeakReference)
        {
            Guard.ArgumentNotNull(channelType, "channelType");
            Guard.ArgumentNotNull(subscriber, "subscriber");

            ChannelInvoker _invoker = GetChannelInvoker(channelType);
            return _invoker.Subscribe(subscriber, threadOption, useWeakReference);
        }

        public static IDisposable Subscribe(Type channelType, string key, Object subscriber, ThreadOption threadOption, bool useWeakReference)
        {
            Guard.ArgumentNotNull(channelType, "channelType");
            Guard.ArgumentNotNull(subscriber, "subscriber");
            Guard.ArgumentNotNullOrWhiteSpace(key, "key");

            ChannelInvoker _invoker = GetChannelInvoker(channelType);
            return _invoker.Subscribe(key, subscriber, threadOption, useWeakReference);
        }

        public static IDisposable Subscribe<T>(IObserver<T> subscriber)
        {
            return Channel<T>.Public.Subscribe(subscriber, ThreadOption.PublisherThread, false);
        }

        public static IDisposable Subscribe<T>(string key, IObserver<T> subscriber)

        {
            return Channel<T>.Private[key].Subscribe(subscriber, ThreadOption.PublisherThread, false);
        }

        public static IDisposable Subscribe<T>(IObserver<T> subscriber, bool useWeakReference)
        {
            return Channel<T>.Public.Subscribe(subscriber, ThreadOption.PublisherThread, useWeakReference);
        }

        public static IDisposable Subscribe<T>(string key, IObserver<T> subscriber, bool useWeakReference)
        {
            return Channel<T>.Private[key].Subscribe(subscriber, ThreadOption.PublisherThread, useWeakReference);
        }

        public static IDisposable Subscribe<T>(IObserver<T> subscriber, ThreadOption threadOption)
        {
            return Channel<T>.Public.Subscribe(subscriber, threadOption, false);
        }

        public static IDisposable Subscribe<T>(string key, IObserver<T> subscriber, ThreadOption threadOption)
        {
            return Channel<T>.Private[key].Subscribe(subscriber, threadOption, false);
        }

        public static IDisposable Subscribe<T>(IObserver<T> subscriber, ThreadOption threadOption, bool useWeakReference)
        {
            return Channel<T>.Public.Subscribe(subscriber, threadOption, useWeakReference);
        }

        public static IDisposable Subscribe<T>(string key, IObserver<T> subscriber, ThreadOption threadOption, bool useWeakReference)
        {
            return Channel<T>.Private[key].Subscribe(subscriber, threadOption, useWeakReference);
        }

        #endregion

        #region Private Channels

        public static Object GetChannel(Type channelType)
        {
            Guard.ArgumentNotNull(channelType, "channelType");

            ChannelInvoker _invoker = GetChannelInvoker(channelType);
            return _invoker.GetChannel();
        }

        public static Object GetChannel(Type channelType, string key)
        {
            Guard.ArgumentNotNull(channelType, "channelType");

            ChannelInvoker _invoker = GetChannelInvoker(channelType);
            return _invoker.GetChannel(key);
        }

        public static IChannel<T> GetChannel<T>()
        {
            return Channel<T>.Public;
        }

        public static IChannel<T> GetChannel<T>(string key)
        {
            return Channel<T>.Private[key];
        }

        #endregion

        #region Nested Classes

        private abstract class ChannelInvoker
        {
            //public abstract Object CreateChannel(string key);

            //public abstract Object CreateOrGetChannel(string key);

            public abstract Object GetChannel();

            public abstract Object GetChannel(string key);

            //public abstract bool TryGetChannel(string key, out Object channel);

            public abstract void Publish(object payload, bool asynchronously);

            public abstract void Publish(string key, object payload, bool asynchronously);

            public abstract void PublishError(Exception error, bool asynchronously);

            public abstract void PublishError(string key, Exception error, bool asynchronously);

            public abstract IDisposable Subscribe(object subscriber, ThreadOption threadOption, bool useWeakReference);

            public abstract IDisposable Subscribe(string key, object subscriber, ThreadOption threadOption, bool useWeakReference);

            //public abstract bool ContainsChannel(string key);
        }

        private class ChannelInvoker<T>
            : ChannelInvoker
        {
            public override Object GetChannel()
            {
                return Channel<T>.Public;
            }

            public override Object GetChannel(string key)
            {
                return Channel<T>.Private[key];
            }

            public override void Publish(object payload, bool asynchronously)
            {
                Channel<T>.Public.OnNext((T)payload, asynchronously);
            }

            public override void Publish(string key, object payload, bool asynchronously)
            {
                Channel<T>.Private[key].OnNext((T)payload, asynchronously);
            }

            public override void PublishError(Exception error, bool asynchronously)
            {
                Channel<T>.Public.OnError(error, asynchronously);
            }

            public override void PublishError(string key, Exception error, bool asynchronously)
            {
                Channel<T>.Private[key].OnError(error, asynchronously);
            }

            public override IDisposable Subscribe(object subscriber, ThreadOption threadOption, bool useWeakReference)
            {
                return Channel<T>.Public.Subscribe((IObserver<T>)subscriber, threadOption, useWeakReference);
            }

            public override IDisposable Subscribe(string key, object subscriber, ThreadOption threadOption, bool useWeakReference)
            {
                return Channel<T>.Private[key].Subscribe((IObserver<T>)subscriber, threadOption, useWeakReference);
            }
        }

        #endregion

        #region Helpers

        private static ChannelInvoker GetChannelInvoker(Type channelType)
        {
            ChannelInvoker _invoker = null;

            lock (_lock)
            {
                if (!_invokers.TryGetValue(channelType, out _invoker))
                {
                    var _channelInvokerType = _invokerGenericType.MakeGenericType(channelType);
                    _invoker = (ChannelInvoker)Activator.CreateInstance(_channelInvokerType);

                    if (!_invokers.ContainsKey(channelType))
                    {
                        _invokers.Add(channelType, _invoker);
                    }
                }
            }

            return _invoker;
        }

        #endregion

    }
}