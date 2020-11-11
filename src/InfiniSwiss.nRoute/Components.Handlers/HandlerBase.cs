using nRoute.Internal;
using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace nRoute.Components.Handlers
{
    public abstract class HandlerBase<E, H>
        : IDisposable
        where
            E : EventArgs
    {

        #region Declarations

        private Func<Action<Object, E>, H> _createAction;
        private Action<H> _removeAction;
        private H _eventHandler;
        private bool _isDisposed;

        #endregion

        #region Static Per Type

        protected readonly static Func<Action<Object, E>, H> _createDelegate;

        static HandlerBase()
        {
            Expression<Func<Action<Object, E>, H>> _createExpr
                = (a) => (H)(Object)Delegate.CreateDelegate(typeof(H), a.Target, a.Method);
            _createDelegate = _createExpr.Compile();
        }

        #endregion

        public HandlerBase(Action<H> removeAction)
            : this((a) => _createDelegate(a), removeAction) { }

        public HandlerBase(Func<Action<Object, E>, H> createAction, Action<H> removeAction)
        {
            Guard.ArgumentNotNull(createAction, "createAction");
            _createAction = createAction;
            _removeAction = removeAction;
        }

        #region Properties

        protected Func<Action<Object, E>, H> CreateAction
        {
            get { return _createAction; }
            set { _createAction = value; }
        }

        protected Action<H> RemoveAction
        {
            get { return _removeAction; }
            set { _removeAction = value; }
        }

        protected H EventHandler
        {
            get { return _eventHandler; }
            set { _eventHandler = value; }
        }

        #endregion

        protected abstract void Handle(Object sender, E eventArgs);

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
                _removeAction?.Invoke(_eventHandler);
                _createAction = null;
                _removeAction = null;
                _eventHandler = default;
                _isDisposed = true;
            }
        }

        #endregion

        #region Operator

        public static implicit operator H(HandlerBase<E, H> handler)
        {
            handler.EventHandler = handler.CreateAction((s, e) =>
            {
                Debug.Assert(handler != null, "Handler should have been detached, as handler is null.");
                if (handler == null) return;
                var _handler = handler;
                var _event = new Event<E>(s, e);

                // we check if it is disposed, if so then we set the handler to null 
                if (_handler._isDisposed) handler = null;        // this should remove the lifting
            });

            // we return
            return handler.EventHandler;
        }

        #endregion

    }
}

