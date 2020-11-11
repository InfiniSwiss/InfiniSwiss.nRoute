using nRoute.Internal;
using System;

namespace nRoute.Components.Messaging
{
    public class ChannelObserver<T>
        : IObserver<T>, IDisposable
    {
        private readonly string _key;
        private Func<T, bool> _payloadFilter;
        private Action<T> _payloadHandler;
        private Action<Exception> _errorHandler;
        private IDisposable _unsubscriberToken;

        public ChannelObserver(Action<T> payloadHandler)
            : this(null, payloadHandler, null, null) { }

        public ChannelObserver(string key, Action<T> payloadHandler)
            : this(key, payloadHandler, null, null) { }

        public ChannelObserver(Action<T> payloadHandler, Action<Exception> errorHandler)
            : this(null, payloadHandler, errorHandler, null) { }

        public ChannelObserver(string key, Action<T> payloadHandler, Action<Exception> errorHandler)
            : this(key, payloadHandler, errorHandler, null) { }

        public ChannelObserver(Action<T> payloadHandler, Func<T, bool> payloadFilter)
            : this(null, payloadHandler, null, payloadFilter) { }

        public ChannelObserver(string key, Action<T> payloadHandler, Func<T, bool> payloadFilter)
            : this(key, payloadHandler, null, payloadFilter) { }

        public ChannelObserver(Action<T> payloadHandler, Action<Exception> errorHandler, Func<T, bool> payloadFilter)
            : this(null, payloadHandler, errorHandler, payloadFilter) { }

        public ChannelObserver(string key, Action<T> payloadHandler, Action<Exception> errorHandler, Func<T, bool> payloadFilter)
        {
            Guard.ArgumentNotNull(payloadHandler, "payloadHandler");
            _key = key;
            _payloadHandler = payloadHandler;
            _errorHandler = errorHandler;
            _payloadFilter = payloadFilter;
        }

        #region Subscribe/Unsubscribe Related

        public bool IsSubscribed
        {
            get { return (_unsubscriberToken != null); }
        }

        public void Subscribe()
        {
            Subscribe(ThreadOption.PublisherThread, false);
        }

        public void Subscribe(ThreadOption threadOption)
        {
            Subscribe(threadOption, false);
        }

        public void Subscribe(bool keepSubscriberReferenceAlive)
        {
            Subscribe(ThreadOption.PublisherThread, keepSubscriberReferenceAlive);
        }

        public virtual void Subscribe(ThreadOption threadOption, bool keepSubscriberReferenceAlive)
        {
            if (_unsubscriberToken != null) return;
            if (string.IsNullOrEmpty(_key))
            {
                _unsubscriberToken = Channel<T>.Public.Subscribe(this, threadOption, !keepSubscriberReferenceAlive);
            }
            else
            {
                _unsubscriberToken = Channel<T>.Private[_key].Subscribe(this, threadOption, !keepSubscriberReferenceAlive);
            }
        }

        public virtual void Unsubscribe()
        {
            if (_unsubscriberToken == null) return;
            _unsubscriberToken.Dispose();
            _unsubscriberToken = null;
        }

        #endregion

        #region IObserver<T> Members

        void IObserver<T>.OnCompleted()
        {
            throw new NotSupportedException();
        }

        void IObserver<T>.OnError(Exception exception)
        {
            if (_errorHandler != null)
            {
                _errorHandler(exception);
            }
        }

        void IObserver<T>.OnNext(T value)
        {
            if (_payloadHandler != null && (_payloadFilter == null || _payloadFilter(value)))
            {
                _payloadHandler(value);
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Overridable

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (IsSubscribed) Unsubscribe();
                _payloadHandler = null;
                _errorHandler = null;
                _payloadFilter = null;
            }
        }

        #endregion

    }
}
