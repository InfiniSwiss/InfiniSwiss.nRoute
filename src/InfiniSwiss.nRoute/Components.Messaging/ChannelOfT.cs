using nRoute.Internal;
using nRoute.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace nRoute.Components.Messaging
{
    public sealed class Channel<T>
        : Channel, IChannel<T>
    {

        #region Declarations

        private const string CHANNEL_CANNOTBE_CLOSED = "Channel of type '{0}' cannot be closed (via OnCompleted)";
        private const string CHANNEL_WITHKEY_EXISTS = "Private Channel of '{0}' with key '{1}' already exists";

        private readonly static Channel<T> _publicChannel;
        private readonly static Dictionary<string, IChannel<T>> _privateChannels;
        private readonly static object _channelsLock = new();

        private readonly object _instanceLock = new();
        private readonly List<ChannelSubscriptionBase<T>> _subscriptions;

        #endregion

        #region Constructor

        static Channel()
        {
            _privateChannels = new Dictionary<string, IChannel<T>>(StringComparer.InvariantCultureIgnoreCase);
            _publicChannel = new Channel<T>();
        }

        private Channel()
        {
            _subscriptions = new List<ChannelSubscriptionBase<T>>();
        }

        #endregion

        #region Public Properties
        public static IChannel<T> Public
        {
            get { return _publicChannel; }
        }

        public static Channel<T> Private
        {
            get { return _publicChannel; }
        }

        public IChannel<T> this[string key]
        {
            get
            {
                Guard.ArgumentNotNullOrWhiteSpace(key, "key");

                lock (_channelsLock)
                {
                    if (_privateChannels.ContainsKey(key))
                    {
                        return _privateChannels[key];
                    }
                    return CreateChannel(key);
                }
            }
        }

        #endregion

        #region Private Channels

        private static IChannel<T> CreateChannel(string key)
        {
            //Guard.ArgumentNotNullOrWhiteSpace(key, "key");

            lock (_channelsLock)
            {
                if (_privateChannels.ContainsKey(key))
                {
                    throw new InvalidOperationException(string.Format(CHANNEL_WITHKEY_EXISTS, typeof(T).FullName, key));
                }

                var _channel = new Channel<T>();
                _privateChannels.Add(key, _channel);
                return _channel;
            }
        }

        #endregion

        #region IObserver<T> Members

        void IObserver<T>.OnNext(T value)
        {
            OnNextInternal(value);
        }

        void IObserver<T>.OnError(Exception exception)
        {
            OnErrorInternal(exception);
        }

        void IObserver<T>.OnCompleted()
        {
            throw new InvalidOperationException(string.Format(CHANNEL_CANNOTBE_CLOSED, typeof(T).FullName));
        }

        #endregion

        #region IChannel<T> Extensions

        void IChannel<T>.OnNext(T value, bool asynchronously)
        {
            if (asynchronously)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback((o) => OnNextInternal(value)));
                //(new BackgroundPayloadPublisher<T>(OnNext, value)).RunWorkerAsync();
            }
            else
            {
                OnNextInternal(value);
            }
        }

        void IChannel<T>.OnNext(T value, IDispatcher dispatcher)
        {
            OnNextInternal(value, dispatcher);
        }

        void IChannel<T>.OnError(Exception exception, bool asynchronously)
        {
            Guard.ArgumentNotNull(exception, "exception");

            if (asynchronously)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback((o) => OnErrorInternal(exception)));
                //(new BackgroundPayloadPublisher<Exception>(OnError, exception)).RunWorkerAsync();
            }
            else
            {
                OnErrorInternal(exception);
            }
        }

        #endregion

        #region IObservable<T> Members

        IDisposable IObservable<T>.Subscribe(IObserver<T> subscriber)
        {
            return OnSubscribeInternal(subscriber, ThreadOption.PublisherThread, false);
        }

        #endregion

        #region IChannel<T> Extensions

        IDisposable IChannel<T>.Subscribe(IObserver<T> subscriber, bool useWeakReference)
        {
            return OnSubscribeInternal(subscriber, ThreadOption.PublisherThread, useWeakReference);
        }

        IDisposable IChannel<T>.Subscribe(IObserver<T> subscriber, ThreadOption threadOption)
        {
            return OnSubscribeInternal(subscriber, threadOption, false);
        }

        IDisposable IChannel<T>.Subscribe(IObserver<T> subscriber, ThreadOption threadOption, bool useWeakReference)
        {
            return OnSubscribeInternal(subscriber, threadOption, useWeakReference);
        }

        #endregion

        #region Internal

        internal void Unsubscribe(ChannelSubscriptionBase<T> subscription)
        {
            Guard.ArgumentNotNull(subscription, "subscription");

            lock (_instanceLock)
            {
                var _subscribers = _subscriptions.ToArray();

                foreach (var _subscriber in _subscribers)
                {
                    if (_subscriber.Subscriber == null)
                        _subscriptions.Remove(_subscriber);

                    else if (Object.ReferenceEquals(_subscriber, subscription))
                        _subscriptions.Remove(_subscriber);
                }
            }

        }

        #endregion

        #region Helpers

        private IDisposable OnSubscribeInternal(IObserver<T> subscriber, ThreadOption threadOption, bool useWeakReference)
        {
            Guard.ArgumentNotNull(subscriber, "subscriber");

            ChannelSubscriptionBase<T> _subscription = useWeakReference
                ? new WeakChannelSubscription<T>(threadOption, subscriber)
                : new ChannelSubscription<T>(threadOption, subscriber);
            lock (_instanceLock)
            {
                // we add it to our collection
                _subscriptions.Add(_subscription);

            }

            // we return
            return _subscription;
        }

        private void OnNextInternal(T value, IDispatcher dispatcher = null)
        {
            var _subscriptionsList = (IEnumerable<ChannelSubscriptionBase<T>>)null;
            lock (_instanceLock)
            {
                if (_subscriptions.Count == 0)
                {
                    return;
                }
                _subscriptionsList = _subscriptions.ToArray();
            }

            PublishInternal(value, _subscriptionsList, dispatcher);
        }

        private void OnErrorInternal(Exception exception)
        {
            Guard.ArgumentNotNull(exception, "exception");

            var _subscriptionsList = (IEnumerable<ChannelSubscriptionBase<T>>)null;
            lock (_instanceLock)
            {
                if (_subscriptions.Count == 0)
                {
                    return;
                }
                _subscriptionsList = _subscriptions.ToArray();
            }

            PublishErrorInternal(exception, _subscriptionsList);
        }

        private void PublishInternal(T value, IEnumerable<ChannelSubscriptionBase<T>> subscriptionsList, IDispatcher dispatcher = null)
        {
            var _deadSubscriptions = (List<ChannelSubscriptionBase<T>>)null;

            var _dispatcher = dispatcher ?? this.GetDispatcherProviderService().Dispatcher; // potential problem with this, as it might be setup later than we need it by? to check!!

            foreach (var _subscription in subscriptionsList)
            {
                var _subscriber = _subscription.Subscriber;

                if (_subscriber == null)
                {
                    if (_deadSubscriptions == null)
                    {
                        _deadSubscriptions = new List<ChannelSubscriptionBase<T>>();
                    }
                    _deadSubscriptions.Add(_subscription);
                }
                else
                {
                    switch (_subscription.ThreadOption)
                    {
                        case ThreadOption.PublisherThread:
                            _subscriber.OnNext(value);
                            break;

                        case ThreadOption.UIThread:
                            if (_dispatcher.CheckAccess())
                            {
                                _subscriber.OnNext(value);
                            }
                            else
                            {
                                _dispatcher.BeginInvoke(new Action(() => _subscriber.OnNext(value)));
                            }
                            break;

                        case ThreadOption.BackgroundThread:
                            ThreadPool.QueueUserWorkItem(new WaitCallback((o) => _subscriber.OnNext(value)));
                            break;

                        default:
                            throw new NotSupportedException();
                    }
                }
            }

            if (_deadSubscriptions != null && _deadSubscriptions.Count > 0)
            {
                PurgeSubscriptions(_deadSubscriptions);
            }
        }

        private void PublishErrorInternal(Exception error, IEnumerable<ChannelSubscriptionBase<T>> subscriptionsList)
        {
            var _deadSubscriptions = (List<ChannelSubscriptionBase<T>>)null;
            var _dispatcher = this.GetDispatcherProviderService().Dispatcher; // potential problem with this, as this is setup later than needed?

            foreach (var _subscription in subscriptionsList)
            {
                var _subscriber = _subscription.Subscriber;

                if (_subscriber == null)
                {
                    if (_deadSubscriptions == null)
                    {
                        _deadSubscriptions = new List<ChannelSubscriptionBase<T>>();
                    }
                    _deadSubscriptions.Add(_subscription);
                }
                else
                {
                    try
                    {
                        switch (_subscription.ThreadOption)
                        {
                            case ThreadOption.PublisherThread:
                                _subscriber.OnError(error);
                                break;

                            case ThreadOption.UIThread:
                                if (_dispatcher.CheckAccess())
                                {
                                    _subscriber.OnError(error);
                                }
                                else
                                {
                                    _dispatcher.BeginInvoke(new Action(() => _subscriber.OnError(error)));
                                }
                                break;

                            case ThreadOption.BackgroundThread:
                                ThreadPool.QueueUserWorkItem(new WaitCallback((o) => _subscriber.OnError(error)));
                                break;

                            default:
                                throw new NotSupportedException();
                        }
                    }
                    catch // (Exception ex)
                    {
                        Debugger.Break();
                        throw;
                    }
                }
            }

            if (_deadSubscriptions != null && _deadSubscriptions.Count > 0)
            {
                PurgeSubscriptions(_deadSubscriptions);
            }
        }

        private void PurgeSubscriptions(IEnumerable<ChannelSubscriptionBase<T>> subscriptions)
        {
            lock (_instanceLock)
            {
                foreach (var _subscription in subscriptions)
                {
                    if (_subscriptions.Contains(_subscription))
                    {
                        _subscriptions.Remove(_subscription);
                    }
                }
            }
        }

        private IDispatcherProviderService GetDispatcherProviderService()
        {
            if (ServiceLocator.TryGetService<IDispatcherProviderService>(out var registeredDispatcherProviderService))
            {
                return registeredDispatcherProviderService;
            }

            return new DispatcherProviderService();
        }

        #endregion

    }
}