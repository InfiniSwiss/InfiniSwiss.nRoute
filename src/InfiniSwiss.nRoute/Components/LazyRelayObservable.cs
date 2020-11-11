using nRoute.Internal;
using System;

namespace nRoute.Components
{
    public class LazyRelayObservable<T>
         : RelayObservable<T>
    {
        private readonly Object _lazyLock = new Object();
        private Action<LazyRelayObservable<T>> _lazyInitializer;

        public LazyRelayObservable(Action<LazyRelayObservable<T>> initializer)
        {
            Guard.ArgumentNotNull(initializer, "initializer");
            _lazyInitializer = initializer;
        }

        #region Overrides

        public override IDisposable Subscribe(IObserver<T> observer)
        {
            Guard.ArgumentNotNull(observer, "observer");

            // we intialize on first subscribe
            if (_lazyInitializer != null)
            {
                // get the token first, because in some cases the initializing will immediately 
                // push out the response, and so we need to have the subscriber in place before
                // and _note we also clear all references to the initializer
                var _token = base.Subscribe(observer);

                lock (_lazyLock)
                {
                    if (_lazyInitializer != null)
                    {
                        _lazyInitializer.Invoke(this);
                        _lazyInitializer = null;
                    }
                }

                // and then return
                return _token;
            }
            else
            {
                return base.Subscribe(observer);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _lazyInitializer = null;
            }
            base.Dispose(disposing);
        }

        #endregion

    }
}
