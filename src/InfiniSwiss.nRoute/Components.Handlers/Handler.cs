using nRoute.Internal;
using System;

namespace nRoute.Components.Handlers
{
    public class Handler<E, H>
        : HandlerBase<E, H>
        where
            E : EventArgs
    {
        public Handler(Action<Object, E> action, Action<H> removeAction)
            : this((a) => _createDelegate(a), action, removeAction) { }

        public Handler(Func<Action<Object, E>, H> createAction, Action<Object, E> action, Action<H> removeAction)
            : base(createAction, removeAction)
        {
            Guard.ArgumentNotNull(action, "action");
            Action = action;
        }

        protected Action<Object, E> Action { get; set; }

        #region Overrides

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                base.Dispose(true);
                Action = null;
            }
        }

        protected override void Handle(object sender, E eventArgs)
        {
            if (Action != null)
                Action(sender, eventArgs);
            else
                Dispose();      // if we don't have a action thing            
        }

        #endregion

    }

    public class WeakHandler<E, H>
        : HandlerBase<E, H>
        where
            E : EventArgs
    {
        private WeakReference _target;
        private EventDelegateInvoker<E> _invoker;
        private readonly bool _methodIsStatic;

        public WeakHandler(Action<Object, E> action, Action<H> removeAction)
            : this((a) => _createDelegate(a), action, removeAction) { }

        public WeakHandler(Func<Action<Object, E>, H> createAction, Action<Object, E> action, Action<H> removeAction)
            : base(createAction, removeAction)
        {
            Guard.ArgumentNotNull(action, "action");
            Target = action.Target;

            var _methodInfo = action.Method;
            _methodIsStatic = _methodInfo.IsStatic;
            _invoker = new EventDelegateInvoker<E>(action.Target != null ? action.Target.GetType() : null, _methodInfo);
        }

        protected Object Target
        {
            get
            {
                if (_target == null || !_target.IsAlive) return null;
                var _targetObj = _target.Target;
                return _targetObj;
            }
            set
            {
                if (value == null)
                {
                    if (_target != null) _target.Target = null;
                    _target = null;
                }
                else
                {
                    _target = new WeakReference(value);
                }
            }
        }

        #region Overrides

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                base.Dispose(true);
                if (Target != null) Target = null;
                _invoker = null;
            }
        }

        protected override void Handle(object sender, E eventArgs)
        {
            Object _targetObj = Target;

            if (_targetObj != null && _invoker != null)
            {
                _invoker.RaiseEvent(_targetObj, sender, eventArgs);
            }
            else if (_invoker != null && _methodIsStatic)
            {
                _invoker.RaiseEvent(null, sender, eventArgs);
            }
            else
            {
                Dispose();
            }
        }

        #endregion

    }
}
