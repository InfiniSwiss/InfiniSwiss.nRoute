using nRoute.Internal;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace nRoute.Components.Handlers
{
    public sealed class EventDelegateInvoker<E>
        where
            E : EventArgs
    {

        #region Declarations

        private const string DELEGATE_NAME_FORMAT = "_____DelegateInvoker{0}";

        private static readonly Dictionary<Type, Func<MethodInfo, DelegateInvoker<E>>> _activators;
        private static readonly Type _invokerGenericType = typeof(DelegateInvoker<Object, E>).GetGenericTypeDefinition();
        private static readonly Object _lock = new Object();

        private DelegateInvoker<E> _invoker;
        private readonly Type _targetType;
        private readonly MethodInfo _method;

        #endregion

        static EventDelegateInvoker()
        {
            _activators = new Dictionary<Type, Func<MethodInfo, DelegateInvoker<E>>>();
        }

        public EventDelegateInvoker(Type targetType, MethodInfo method)
        {
            Guard.ArgumentNotNull(method, "method");
            _targetType = targetType;
            _method = method;
        }

        #region Main Method

        public void RaiseEvent(Object target, Object sender, E args)
        {
            if (_invoker == null) CreateInvoker();
            _invoker.Invoke(target, sender, args);
        }

        #endregion

        #region Helpers

        private void CreateInvoker()
        {
            // lazy load in a way
            if (_targetType != null)
            {
                lock (_lock)
                {
                    if (_activators.ContainsKey(_targetType))
                        _invoker = _activators[_targetType](_method);
                    else
                    {
                        var _func = CreateActivator();
                        _activators.Add(_targetType, _func);
                        _invoker = _func(_method);
                    }
                }
            }
            else
            {
                _invoker = new DelegateInvoker<E>(_method);
            }
        }

        private Func<MethodInfo, DelegateInvoker<E>> CreateActivator()
        {
            // adopted from http://beaucrawford.net/post/Constructor-invocation-with-DynamicMethod.aspx
            var type = _invokerGenericType.MakeGenericType(typeof(E), _targetType, typeof(E));
            var _constParms = new Type[] { typeof(MethodInfo) };
            var _dynamicMethod = new DynamicMethod(string.Format(DELEGATE_NAME_FORMAT, type.Name), type, _constParms, type);
            var _ilGenerator = _dynamicMethod.GetILGenerator();

            var constructor = type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, _constParms, null);
            _ilGenerator.Emit(OpCodes.Nop);
            _ilGenerator.Emit(OpCodes.Ldarg_0);
            _ilGenerator.Emit(OpCodes.Newobj, constructor);
            _ilGenerator.Emit(OpCodes.Ret);

            // note_ we actually create DelegateInvoker<T, E> but get it back as DelegateInvoker<T>
            var _delegate = _dynamicMethod.CreateDelegate(typeof(DelegateActivator<DelegateInvoker<E>, MethodInfo>))
                as DelegateActivator<DelegateInvoker<E>, MethodInfo>;
            Expression<Func<MethodInfo, DelegateInvoker<E>>> _exp = (m) => _delegate(m);
            return _exp.Compile();
        }

        #endregion

        #region Internal Classes

        public delegate T DelegateActivator<T, A>(A arg1)
            where
                T : class;

        public class DelegateInvoker<E>
            where
                E : EventArgs
        {
            private readonly EventDelegate<E> _delegate;

            protected DelegateInvoker() { }

            public DelegateInvoker(MethodInfo method)
            {
                Guard.ArgumentNotNull(method, "method");
                _delegate = (EventDelegate<E>)Delegate.CreateDelegate(typeof(EventDelegate<E>), method, true);
            }

            public virtual void Invoke(Object target, Object sender, E args)
            {
                _delegate(sender, args);
            }
        }

        public class DelegateInvoker<T, E>
            : DelegateInvoker<E>
            where
                E : EventArgs
        {
            private readonly EventDelegate<T, E> _delegate;

            public DelegateInvoker(MethodInfo method)
                : base()
            {
                Guard.ArgumentNotNull(method, "method");
                _delegate = (EventDelegate<T, E>)Delegate.CreateDelegate(typeof(EventDelegate<T, E>), method, true);
            }

            public override void Invoke(object target, object sender, E args)
            {
                this.Invoke((T)target, sender, args);
            }

            public void Invoke(T target, object sender, E args)
            {
                _delegate(target, sender, args);
            }
        }

        #endregion

        //#endif

    }
}
