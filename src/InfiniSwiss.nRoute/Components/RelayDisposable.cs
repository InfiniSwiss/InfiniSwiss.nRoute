using System;

namespace nRoute.Components
{
    public sealed class RelayDisposable
         : IDisposable
    {
        private bool _isDisposed;
        private Action _disposeCallback;

        public RelayDisposable() { }

        public RelayDisposable(Action disposeCallback)
        {
            _disposeCallback = disposeCallback;
        }

        #region Properties

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

        #endregion

        #region Overridable

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!_isDisposed)
                {
                    _isDisposed = true;     // this is callbed before calling the callback
                    if (_disposeCallback != null) _disposeCallback();
                    _disposeCallback = null;
                }
            }
        }

        #endregion

    }
}
