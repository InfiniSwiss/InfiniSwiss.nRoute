using System;

namespace nRoute.Components.Disposer
{
    public class Disposer
         : IDisposeRelated, IDisposable
    {

        #region IDisposeRelated Members

        public event EventHandler DisposeRelated;

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Overridable

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (DisposeRelated != null) DisposeRelated(this, EventArgs.Empty);
                DisposeRelated = null;
            }
        }

        #endregion

    }

}
