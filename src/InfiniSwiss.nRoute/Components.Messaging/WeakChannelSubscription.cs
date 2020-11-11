using nRoute.Internal;
using System;

namespace nRoute.Components.Messaging
{
    public class WeakChannelSubscription<T>
         : ChannelSubscriptionBase<T>
    {
        WeakReference _subscriber;

        public WeakChannelSubscription(ThreadOption threadOption, IObserver<T> subscriber)
         : base(threadOption)
        {
            Guard.ArgumentNotNull(subscriber, "subscriber");
            _subscriber = new WeakReference(subscriber);
        }

        #region Overrides

        public override IObserver<T> Subscriber
        {
            get
            {
                if (_subscriber == null || !_subscriber.IsAlive) return null;
                var _subscriberObj = _subscriber.Target as IObserver<T>;
                return _subscriberObj;
            }
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