using nRoute.Internal;
using System;
using System.Threading;

namespace nRoute.Components
{
    public class Future<T>
    {
        private const string VALUE_NOT_AVALIABLE = "Value for type '{0}' is not available currently.";
        private readonly Object _lock;

        private T _value = default(T);
        private Func<bool> _valueAvailable;
        private Func<T> _valueFactory = null;
        private volatile bool _isValueCreated = false;
        private volatile bool _isValueAvailable = false;

        public Future(Func<bool> valueAvailable)
            : this(valueAvailable, TypeActivator.CreateInstance<T>) { }

        public Future(Func<bool> valueAvailable, bool isThreadSafe)
            : this(valueAvailable, TypeActivator.CreateInstance<T>, isThreadSafe) { }

        public Future(Func<bool> valueAvailable, Func<T> valueFactory) :
            this(valueAvailable, valueFactory, true)
        { }

        public Future(Func<bool> valueAvailable, Func<T> valueFactory, bool isThreadSafe)
        {
            Guard.ArgumentNotNull(valueFactory, "valueFactory");
            Guard.ArgumentNotNull(valueAvailable, "valueAvailable");

            if (isThreadSafe)
            {
                this._lock = new object();
            }
            _valueAvailable = valueAvailable;
            _valueFactory = valueFactory;
        }

        #region Properties

        public bool IsValueAvailable
        {
            get
            {
                if (!this._isValueAvailable)
                {
                    if (this._lock != null)
                    {
                        Monitor.Enter(this._lock);
                    }

                    try
                    {
                        var _isAvailable = _valueAvailable();
                        if (_isAvailable)
                        {
                            _valueAvailable = null;
                        }
                        Thread.MemoryBarrier();
                        this._isValueAvailable = _isAvailable;
                    }
                    finally
                    {
                        if (this._lock != null)
                        {
                            Monitor.Exit(this._lock);
                        }
                    }
                }
                return this._isValueAvailable;
            }
        }

        public T Value
        {
            get
            {
                if (!IsValueAvailable)
                {
                    throw new InvalidOperationException(string.Format(VALUE_NOT_AVALIABLE, typeof(T).FullName));
                }

                if (!this._isValueCreated)
                {
                    if (this._lock != null)
                    {
                        Monitor.Enter(this._lock);
                    }

                    try
                    {
                        T value = this._valueFactory.Invoke();
                        this._valueFactory = null;
                        Thread.MemoryBarrier();
                        this._value = value;
                        this._isValueCreated = true;
                    }
                    finally
                    {
                        if (this._lock != null)
                        {
                            Monitor.Exit(this._lock);
                        }
                    }
                }
                return this._value;
            }
        }

        #endregion

    }
}