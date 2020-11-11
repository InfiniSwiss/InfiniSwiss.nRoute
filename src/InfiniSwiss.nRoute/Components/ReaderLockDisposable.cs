using System;
using System.Threading;

namespace nRoute.Components
{
    internal sealed class ReadLockDisposable
         : IDisposable
    {
        private readonly ReaderWriterLock _readerWriterLock;

        public ReadLockDisposable(ReaderWriterLock readerWriterLock)
        {
            this._readerWriterLock = readerWriterLock;
        }

        public IDisposable AcquireReaderLock()
        {
            return AcquireReaderLock(Timeout.Infinite);
        }

        public IDisposable AcquireReaderLock(int millisecondsTimeout)
        {
            this._readerWriterLock.AcquireReaderLock(millisecondsTimeout);
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
                this._readerWriterLock.ReleaseReaderLock();
            }
        }

        #endregion

    }

}
