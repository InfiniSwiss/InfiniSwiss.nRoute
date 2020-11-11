using System;
using System.Threading;

namespace nRoute.Components
{
    internal sealed class WriteLockDisposable
         : IDisposable
    {
        private readonly ReaderWriterLock _readerWriterLock;

        public WriteLockDisposable(ReaderWriterLock readerWriterLock)
        {
            this._readerWriterLock = readerWriterLock;
        }

        public IDisposable AcquireWriterLock()
        {
            return AcquireWriterLock(Timeout.Infinite);
        }

        public IDisposable AcquireWriterLock(int millisecondsTimeout)
        {
            this._readerWriterLock.AcquireWriterLock(millisecondsTimeout);
            return this;
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
                this._readerWriterLock.ReleaseWriterLock();
            }
        }

        #endregion

    }

}
