using nRoute.Internal;
using System;

namespace nRoute.Components.Messaging
{
    public class ChannelSubscription<T>
         : ChannelSubscriptionBase<T>
    {
        IObserver<T> _subscriber;

        public ChannelSubscription(ThreadOption threadOption, IObserver<T> subscriber) :
            base(threadOption)
        {
            Guard.ArgumentNotNull(subscriber, "subscriber");
            _subscriber = subscriber;
        }

        #region Overrides

        public override IObserver<T> Subscriber
        {
            get { return _subscriber; }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_subscriber != null)
                {
                    ((Channel<T>)Channel<T>.Public).Unsubscribe(this);
                }
                _subscriber = null;
            }
        }

        #endregion

    }
}
