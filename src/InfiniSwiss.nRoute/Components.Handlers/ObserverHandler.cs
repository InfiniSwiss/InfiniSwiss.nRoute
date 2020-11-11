using nRoute.Internal;
using System;

namespace nRoute.Components.Handlers
{
    public class ObserverHandler<E, H>
         : HandlerBase<E, H>, IObservable<Event<E>>
        where E
         : EventArgs
    {
        public ObserverHandler(IObserver<Event<E>> observer, Action<H> removeAction)
         : this((a) => _createDelegate(a), observer, removeAction) { }

        public ObserverHandler(Func<Action<Object, E>, H> createAction, IObserver<Event<E>> observer, Action<H> removeAction)
         : base(createAction, removeAction)
        {
            Guard.ArgumentNotNull(observer, "observer");
            Subscribe(observer);
        }

        protected IObserver<Event<E>> Observer { get; set; }

        #region Overrides

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                base.Dispose(true);
                var _observerObj = Observer;
                if (_observerObj != null)
                {
                    _observerObj.OnCompleted();
                    Observer = null;
                }
            }
        }

        protected override void Handle(object sender, E eventArgs)
        {
            if (Observer != null)
                Observer.OnNext(new Event<E>(sender, eventArgs));
            else
                Dispose();
        }

        #endregion

        #region IObservable<Dissmised<E>> Members

        public IDisposable Subscribe(IObserver<Event<E>> observer)
        {
            if (Observer != null)
                throw new InvalidOperationException("Only one observer is supported per ObserverHandler.");
            Observer = observer;
            return this;
        }

        #endregion

    }

    public class WeakObserverHandler<E, H>
         : HandlerBase<E, H>, IObservable<Event<E>>
        where E
         : EventArgs
    {
        WeakReference _observer;

        public WeakObserverHandler(IObserver<Event<E>> observer, Action<H> removeAction)
         : this((a) => _createDelegate(a), observer, removeAction) { }

        public WeakObserverHandler(Func<Action<Object, E>, H> createAction, IObserver<Event<E>> observer,
            Action<H> removeAction)
         : base(createAction, removeAction)
        {
            Guard.ArgumentNotNull(observer, "observer");
            Subscribe(observer);
        }

        protected IObserver<Event<E>> Observer
        {
            get
            {
                if (_observer == null || !_observer.IsAlive) return null;
                var _observerObj = _observer.Target;
                return (IObserver<Event<E>>)_observerObj;
            }
            set
            {
                if (value == null)
                {
                    if (_observer != null) _observer.Target = null;
                    _observer = null;
                }
                else
                    _observer = new WeakReference(value);
            }
        }

        #region Overrides

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                base.Dispose(true);
                var _observerObj = Observer;
                if (_observerObj != null)
                {
                    _observerObj.OnCompleted();
                    Observer = null;
                }
            }
        }

        protected override void Handle(object sender, E eventArgs)
        {
            var _observerObj = Observer;

            if (_observerObj != null)
                _observerObj.OnNext(new Event<E>(sender, eventArgs));
            else
                Dispose();
        }

        #endregion

        #region IObservable<Dissmised<E>> Members

        public IDisposable Subscribe(IObserver<Event<E>> observer)
        {
            if (_observer != null)
                throw new InvalidOperationException("Only one observer is supported per WeakObserverHandler.");
            _observer = new WeakReference(observer);
            return this;
        }

        #endregion

    }
}
