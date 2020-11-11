using System;

namespace nRoute.Components.Messaging
{
    public abstract class ChannelSubscriptionBase<T>
         : IDisposable
    {
        private readonly ThreadOption _threadOption;

        protected ChannelSubscriptionBase(ThreadOption threadOption)
        {
            _threadOption = threadOption;
        }

        #region Properties

        public abstract IObserver<T> Subscriber { get; }

        public ThreadOption ThreadOption
        {
            get { return _threadOption; }
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

        protected virtual void Dispose(bool disposing) { }

        #endregion

    }
}