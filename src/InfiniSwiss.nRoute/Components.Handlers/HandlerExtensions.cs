using nRoute.Internal;
using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Threading;

namespace nRoute.Components.Handlers
{
    [Obsolete]
    internal static class HandlerExtensions
    {
        private const string HANDLECOUNT_OUTOFRANGE = "Handle Exactly count cannot be less than 1";
        private const string HANDLER_PERIODLESSTHANZERO = "Handler's Period cannot be less than or equal to zero timespan";

        // When applies if the predicate is true

        public static HandlerBase<E, H> When<E, H>(this HandlerBase<E, H> handler, Func<Event<E>, bool> predicate)
            where
                E : EventArgs
        {
            Guard.ArgumentNotNull(handler, "handler");
            Guard.ArgumentNotNull(predicate, "predicate");

            if (handler.Predicate != null)
            {
                var _existingPredicate = handler.Predicate;
                handler.Predicate = (e) => _existingPredicate(e) && predicate(e);
            }
            else
                handler.Predicate = predicate;
            return handler;
        }

        public static T When<T, E, H>(this T handler, Func<Event<E>, bool> predicate)
            where
                T : HandlerBase<E, H>
            where
                E : EventArgs
        {
            HandlerExtensions.When<E, H>(handler, predicate);
            return handler;
        }

        // Skip is the oppose of when

        public static HandlerBase<E, H> SkipWhen<E, H>(this HandlerBase<E, H> handler, Func<Event<E>, bool> predicate)
            where
                E : EventArgs
        {
            Guard.ArgumentNotNull(handler, "handler");
            Guard.ArgumentNotNull(predicate, "predicate");

            if (handler.Predicate != null)
            {
                var _existingPredicate = handler.Predicate;
                handler.Predicate = (e) => _existingPredicate(e) && !predicate(e);
            }
            else
                handler.Predicate = (e) => !predicate(e);
            return handler;
        }

        public static T SkipWhen<T, E, H>(this T handler, Func<Event<E>, bool> predicate)
            where
                T : HandlerBase<E, H>
            where
                E : EventArgs
        {
            HandlerExtensions.SkipWhen<E, H>(handler, predicate);
            return handler;
        }

        // Handle Once, allows the handled to be called once and thereafer it will be disposed off

        public static HandlerBase<E, H> HandleOnce<E, H>(this HandlerBase<E, H> handler)
            where
                E : EventArgs
        {
            Guard.ArgumentNotNull(handler, "handler");

            if (handler.PostHandle != null)
            {
                var _existingHandle = handler.PostHandle;
                handler.PostHandle = (h, e) => { _existingHandle(h, e); h.Dispose(); };
            }
            else
                handler.PostHandle = (h, e) => h.Dispose();
            return handler;
        }

        public static T HandleOnce<T, E, H>(this T handler)
            where
                T : HandlerBase<E, H>
            where
                E : EventArgs
        {
            HandlerExtensions.HandleOnce<E, H>(handler);
            return handler;
        }

        // Handle exactly - allows the handle to be called an exact number of times

        public static HandlerBase<E, H> HandleExactly<E, H>(this HandlerBase<E, H> handler, int count)
            where
                E : EventArgs
        {
            Guard.ArgumentNotNull(handler, "handler");
            Guard.ArgumentOutOfRange((count < 1), "count", HANDLECOUNT_OUTOFRANGE);

            int _count = count - 1;
            if (handler.PostHandle != null)
            {
                var _existingHandle = handler.PostHandle;
                handler.PostHandle = (h, e) =>
                {
                    _existingHandle(h, e);
                    if (_count == 0)
                        h.Dispose();
                    else
                        _count -= 1;
                };
            }
            else
                handler.PostHandle = (h, e) =>
                {
                    if (_count == 0)
                        h.Dispose();
                    else
                        _count -= 1;
                };
            return handler;
        }

        public static T HandleExactly<T, E, H>(this T handler, int count)
            where
                T : HandlerBase<E, H>
            where
                E : EventArgs
        {
            HandlerExtensions.HandleExactly<E, H>(handler, count);
            return handler;
        }

        // While - it will handle while the condition is true, once false - note this checked pre-handling

        public static HandlerBase<E, H> While<E, H>(this HandlerBase<E, H> handler, Func<Event<E>, bool> condition)
            where
                E : EventArgs
        {
            Guard.ArgumentNotNull(handler, "handler");

            if (handler.PreHandle != null)
            {
                var _existingHandle = handler.PreHandle;
                handler.PreHandle = (h, e) =>
                {
                    _existingHandle(h, e);
                    if (!condition(e)) h.Dispose();
                };
            }
            else
                handler.PreHandle = (h, e) => { if (!condition(e)) h.Dispose(); };
            return handler;
        }

        public static T While<T, E, H>(this T handler, Func<Event<E>, bool> condition)
            where
                T : HandlerBase<E, H>
            where
                E : EventArgs
        {
            HandlerExtensions.While<E, H>(handler, condition);
            return handler;
        }

        // Until - it will handle till the condition is true, note this 

        public static HandlerBase<E, H> Until<E, H>(this HandlerBase<E, H> handler, Func<Event<E>, bool> condition)
            where
                E : EventArgs
        {
            Guard.ArgumentNotNull(handler, "handler");

            if (handler.PostHandle != null)
            {
                var _existingHandle = handler.PostHandle;
                handler.PostHandle = (h, e) =>
                {
                    _existingHandle(h, e);
                    if (!condition(e)) h.Dispose();
                };
            }
            else
                handler.PostHandle = (h, e) => { if (!condition(e)) h.Dispose(); };
            return handler;
        }

        public static T Until<T, E, H>(this T handler, Func<Event<E>, bool> condition)
            where
                T : HandlerBase<E, H>
            where
                E : EventArgs
        {
            HandlerExtensions.Until<E, H>(handler, condition);
            return handler;
        }

        // For - specifically for property changed event args

        public static HandlerBase<PropertyChangedEventArgs, PropertyChangedEventHandler> HandleFor
            (this HandlerBase<PropertyChangedEventArgs, PropertyChangedEventHandler> handler, Expression<Func<Object>> propertySelector)
        {
            Guard.ArgumentNotNull(handler, "handler");
            Guard.ArgumentNotNull(propertySelector, "propertySelector");

            // get the property name
            var _propertyName = PropertyChangedExtensions.GetPropertyName(propertySelector);

            if (handler.Predicate != null)
            {
                var _existingPredicate = handler.Predicate;
                handler.Predicate = (e) => _existingPredicate(e) &&
                    (string.Equals(e.EventArgs.PropertyName, _propertyName, StringComparison.InvariantCulture));
            }
            else
                handler.Predicate = (e) => string.Equals(e.EventArgs.PropertyName, _propertyName, StringComparison.InvariantCulture);
            return handler;
        }

        public static T HandleFor<T>(this T handler, Expression<Func<Object>> propertySelector)
            where
                T : HandlerBase<PropertyChangedEventArgs, PropertyChangedEventHandler>
        {
            HandlerExtensions.HandleFor((HandlerBase<PropertyChangedEventArgs, PropertyChangedEventHandler>)handler, propertySelector);
            return handler;
        }

        // Throttle     

        public static HandlerBase<E, H> Throttle<E, H>(this HandlerBase<E, H> handler, TimeSpan period)
            where
                E : EventArgs
        {
            Guard.ArgumentNotNull(handler, "handler");
            Guard.ArgumentOutOfRange((period <= TimeSpan.Zero), "period", HANDLER_PERIODLESSTHANZERO);

            var _lastAllow = DateTime.MinValue;

            if (handler.Predicate != null)
            {
                var _existingPredicate = handler.Predicate;
                handler.Predicate = (e) =>
                {
                    var _allow = (DateTime.Now.Subtract(_lastAllow) > period);
                    if (_allow) _lastAllow = DateTime.Now;
                    return _existingPredicate(e) && _allow;
                };
            }
            else
            {
                handler.Predicate = (e) =>
                {
                    var _allow = (DateTime.Now.Subtract(_lastAllow) > period);
                    if (_allow) _lastAllow = DateTime.Now;
                    return _allow;
                };
            }
            return handler;
        }

        public static T Throttle<T, E, H>(this T handler, TimeSpan period)
            where
                T : HandlerBase<E, H>
            where
                E : EventArgs
        {
            HandlerExtensions.Throttle<E, H>(handler, period);
            return handler;
        }

        // Timeout     

        public static HandlerBase<E, H> Timeout<E, H>(this HandlerBase<E, H> handler, TimeSpan period)
            where
                E : EventArgs
        {
            return Timeout(handler, period, null);
        }

        public static HandlerBase<E, H> Timeout<E, H>(this HandlerBase<E, H> handler, TimeSpan period, Action onTimeout)
            where
                E : EventArgs
        {
            Guard.ArgumentNotNull(handler, "handler");
            Guard.ArgumentOutOfRange((period <= TimeSpan.Zero), "period", HANDLER_PERIODLESSTHANZERO);

            // if the timer finishes before the first event, it will dispose the handler
            var _timer = new DispatcherTimer { Interval = period };
            _timer.Tick += (s, e) =>
            {
                if (handler != null) handler.Dispose();
                handler = null;
                if (onTimeout != null) onTimeout();
                onTimeout = null;
            };

            if (handler.PreHandle != null)
            {
                var _existingHandle = handler.PreHandle;
                handler.PreHandle = (h, e) =>
                {
                    _existingHandle(h, e);
                    if (_timer != null)
                    {
                        _timer.Stop();
                        _timer = null;
                        onTimeout = null;
                    }
                };
            }
            else
                handler.PreHandle = (h, e) =>
                {
                    if (_timer != null)
                    {
                        _timer.Stop();
                        _timer = null;
                    }
                };

            _timer.Start();
            return handler;
        }

        public static T Timeout<T, E, H>(this T handler, TimeSpan period)
            where
                T : HandlerBase<E, H>
            where
                E : EventArgs
        {
            HandlerExtensions.Timeout<E, H>(handler, period);
            return handler;
        }

        public static T Timeout<T, E, H>(this T handler, TimeSpan period, Action onTimeout)
            where
                T : HandlerBase<E, H>
            where
                E : EventArgs
        {
            HandlerExtensions.Timeout<E, H>(handler, period, onTimeout);
            return handler;
        }

        // Recurring Timeout     

        public static HandlerBase<E, H> RecurringTimeout<E, H>(this HandlerBase<E, H> handler, TimeSpan period)
            where
                E : EventArgs
        {
            return RecurringTimeout(handler, period, null);
        }

        public static HandlerBase<E, H> RecurringTimeout<E, H>(this HandlerBase<E, H> handler, TimeSpan period, Action onTimeout)
            where
                E : EventArgs
        {
            Guard.ArgumentNotNull(handler, "handler");
            Guard.ArgumentOutOfRange((period <= TimeSpan.Zero), "period", HANDLER_PERIODLESSTHANZERO);

            // if the timer finishes before the first event, it will dispose the handler
            var _timer = new DispatcherTimer();
            _timer.Interval = period;
            _timer.Tick += (s, e) =>
            {
                if (handler != null) handler.Dispose();
                handler = null;
                if (onTimeout != null) onTimeout();
                onTimeout = null;
            };

            if (handler.PreHandle != null)
            {
                var _existingHandle = handler.PreHandle;
                handler.PreHandle = (h, e) =>
                {
                    _existingHandle(h, e);
                    if (_timer != null)
                    {
                        _timer.Stop();
                        _timer.Start();
                    }
                };
            }
            else
                handler.PreHandle = (h, e) =>
                {
                    if (_timer != null)
                    {
                        _timer.Stop();
                        _timer.Start();
                    }
                };

            _timer.Start();
            return handler;
        }

        public static T RecurringTimeout<T, E, H>(this T handler, TimeSpan period)
            where
                T : HandlerBase<E, H>
            where
                E : EventArgs
        {
            HandlerExtensions.RecurringTimeout<E, H>(handler, period);
            return handler;
        }

        public static T RecurringTimeout<T, E, H>(this T handler, TimeSpan period, Action onTimeout)
            where
                T : HandlerBase<E, H>
            where
                E : EventArgs
        {
            HandlerExtensions.RecurringTimeout<E, H>(handler, period, onTimeout);
            return handler;
        }
    }

}