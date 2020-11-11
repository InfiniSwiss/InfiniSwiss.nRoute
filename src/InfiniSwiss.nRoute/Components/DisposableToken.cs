using System;

namespace nRoute.Components
{
    public sealed class DisposableToken
        : IDisposableToken
    {
        private readonly Object _lock = new Object();
        private Action _disposableCallback;
        private volatile bool _isDisposed;

        public DisposableToken() { }

        public DisposableToken(Action disposableCallback)
        {
            _disposableCallback = disposableCallback;
        }

        #region IDisposableToken Members

        public bool IsDisposed
        {
            get { return _isDisposed; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool withCallback)
        {
            if (!_isDisposed)
            {
                lock (_lock)
                {
                    if (!_isDisposed)
                    {
                        if (withCallback && _disposableCallback != null) _disposableCallback();
                        _disposableCallback = null;
                        _isDisposed = true;
                    }
                }
            }
        }

        #endregion

    }
}
