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

        [Obsolete]
        internal Func<Event<E>, bool> Predicate { get; set; }

        [Obsolete]
        internal Action<HandlerBase<E, H>, Event<E>> PreHandle { get; set; }

        [Obsolete]
        internal Action<HandlerBase<E, H>, Event<E>> PostHandle { get; set; }

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
                if (_removeAction != null) _removeAction(_eventHandler);
                Predicate = null;
                PreHandle = null;
                PostHandle = null;
                _createAction = null;
                _removeAction = null;
                _eventHandler = default(H);
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

                //if (handler.Predicate != null && !handler.Predicate(_event)) return;
                var _preHandler = _handler.PreHandle;
                if (!_handler._isDisposed && _preHandler != null)
                {
                    _preHandler(_handler, _event);
                }

                var _predicate = _handler.Predicate;
                if (!_handler._isDisposed && (_predicate == null || _predicate(_event)))
                {
                    _handler.Handle(s, e);
                }

                var _postHandler = _handler.PostHandle;
                if (!_handler._isDisposed && _postHandler != null)
                {
                    _postHandler(_handler, _event);
                }

                // we check if it is disposed, if so then we set the handler to null 
                if (_handler._isDisposed) handler = null;        // this should remove the lifting
            });

            // we return
            return handler.EventHandler;
        }

        #endregion

    }
}

