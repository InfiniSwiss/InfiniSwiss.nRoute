using nRoute.Internal;
using System;
using System.Collections.Generic;

namespace nRoute.Components
{
    public class RelayObservable<T>
         : ISubject<T>
    {
        private readonly Object _lock = new object();
        private readonly List<IObserver<T>> _observers;

        public RelayObservable()
        {
            _observers = new List<IObserver<T>>();
        }

        protected bool IsStopped { get; set; }

        #region IObservable<T> Members

        public virtual IDisposable Subscribe(IObserver<T> observer)
        {
            Guard.ArgumentNotNull(observer, "observer");
            lock (_lock)
            {
                _observers.Add(observer);
                return new DisposableToken(this, observer);
            }
        }

        #endregion

        #region IObserver<T> Members

        public virtual void OnCompleted()
        {
            if (IsStopped) return;

            ForEachObserver((o) => o.OnCompleted());
            IsStopped = true;
        }

        public virtual void OnError(Exception exception)
        {
            Guard.ArgumentNotNull(exception, "exception");
            if (IsStopped) return;

            ForEachObserver((o) => o.OnError(exception));
            IsStopped = true;
        }

        public virtual void OnNext(T value)
        {
            if (IsStopped) return;
            ForEachObserver((o) => o.OnNext(value));
        }

        #endregion

        #region Helpers

        private void Unsubscribe(IObserver<T> observer)
        {
            lock (_lock)
            {
                if (_observers.Contains(observer))
                {
                    _observers.Remove(observer);
                }
            }
        }

        private void ForEachObserver(Action<IObserver<T>> forEach)
        {
            // get the observers
            IObserver<T>[] _observersArray = null;
            lock (_lock)
            {
                _observersArray = _observers.ToArray();
            }

            // and say completed
            if (_observersArray.Length > 0)
            {
                foreach (var _observer in _observersArray)
                {
                    forEach(_observer);
                }
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.OnCompleted();
                lock (_lock)
                {
                    _observers.Clear();
                    // we don't _observers = null;
                }
            }
        }

        #endregion

        #region Internal Class

        private sealed class DisposableToken
            : IDisposable
        {
            private IObserver<T> _observer;
            private RelayObservable<T> _subject;

            public DisposableToken(RelayObservable<T> subject, IObserver<T> observer)
            {
                _subject = subject;
                _observer = observer;
            }

            #region IDisposable Members

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            #endregion

            #region Overridable

            private void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (_subject != null)
                    {
                        _subject.Unsubscribe(_observer);
                        _subject = null;
                        _observer = null;
                    }
                }
            }

            #endregion

        }

        #endregion

    }
}