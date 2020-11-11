using nRoute.Internal;
using System;

namespace nRoute.Components
{
    public class RelayObserver<T>
         : IObserver<T>
    {
        private readonly Action<T> _onNext;
        private readonly Action<Exception> _onError;
        private readonly Action _onCompleted;

        public RelayObserver(Action<T> onNext)
         : this(onNext, null, null) { }

        public RelayObserver(Action<T> onNext, Action onCompleted)
         : this(onNext, null, onCompleted) { }

        public RelayObserver(Action<T> onNext, Action<Exception> onError)
         : this(onNext, onError, null) { }

        public RelayObserver(Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            Guard.ArgumentNotNull(onNext, "onNext");
            _onNext = onNext;
            _onError = onError;
            _onCompleted = onCompleted;
        }

        protected bool IsStopped { get; set; }

        #region IObserver<T> Members

        public void OnCompleted()
        {
            if (!IsStopped && _onCompleted != null)
            {
                IsStopped = true;
                _onCompleted();
            }
        }

        public void OnError(Exception exception)
        {
            Guard.ArgumentNotNull(exception, "exception");
            if (!IsStopped)
            {
                IsStopped = true;
                if (_onError != null)
                {
                    _onError(exception);
                }
                else
                {
                    throw exception;
                }
            }
        }

        public void OnNext(T value)
        {
            if (!IsStopped) _onNext(value);
        }

        #endregion

    }
}
