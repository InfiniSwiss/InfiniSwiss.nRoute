using Microsoft.Xaml.Behaviors;
using nRoute.Components;
using nRoute.Components.Messaging;
using nRoute.Internal;
using System;
using System.ComponentModel;
using System.Windows;

namespace nRoute.Behaviors.Triggers
{
    public class ChannelTrigger
        : TriggerBase<DependencyObject>
    {
        private static Type CHANNEL_LISTENER_TYPE = typeof(ChannelListener<>);

        public static readonly DependencyProperty ChannelTypeProperty =
            DependencyProperty.Register("ChannelType", typeof(Type), typeof(ChannelTrigger),
            new PropertyMetadata(null, new PropertyChangedCallback(OnChannelTypeChanged)));

        public static readonly DependencyProperty ChannelKeyProperty =
            DependencyProperty.Register("ChannelKey", typeof(string), typeof(ChannelTrigger),
            new PropertyMetadata(null, new PropertyChangedCallback(OnChannelKeyChanged)));

        private ChannelListener _listener;

        #region Properties

        [Category("Common Properties")]
        public Type ChannelType
        {
            get { return (Type)GetValue(ChannelTypeProperty); }
            set { SetValue(ChannelTypeProperty, value); }
        }

        [Category("Common Properties")]
        public string ChannelKey
        {
            get { return Convert.ToString(GetValue(ChannelKeyProperty)); }
            set { SetValue(ChannelKeyProperty, value); }
        }

        #endregion

        #region Overrides

        protected override void OnAttached()
        {
            base.OnAttached();
            if (_listener == null) SetupListener(this.ChannelType, this.ChannelKey);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (_listener != null) _listener.Dispose();
        }

        #endregion

        #region Handlers

        private static void OnChannelTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var _trigger = (ChannelTrigger)d;
            _trigger.SetupListener(e.NewValue as Type, _trigger.ChannelKey);
        }

        private static void OnChannelKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var _trigger = (ChannelTrigger)d;
            _trigger.SetupListener(_trigger.ChannelType, Convert.ToString(e.NewValue));
        }

        #endregion

        #region Helpers

        private void SetupListener(Type channelType, string channelKey)
        {
            if (_listener != null)
            {
                _listener.Dispose();
                _listener = null;
            }

            if (this.AssociatedObject == null || channelType == null) return;
            _listener = (ChannelListener)TypeActivator.CreateInstance(CHANNEL_LISTENER_TYPE.MakeGenericType(channelType));
            _listener.Subscribe(channelKey, base.InvokeActions);
        }

        #endregion

        #region Internal Classes

        public abstract class ChannelListener
            : IDisposable
        {

            #region Must Override

            protected internal abstract void Subscribe(string key, Action<object> onMessage);

            protected internal abstract void Dispose(bool disposing);

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            #endregion

        }

        public class ChannelListener<T>
            : ChannelListener
        {
            private const string CHANNEL_ALREADYSUBSCRIBED = "ChannelListener can only be subscribe once";

            private ChannelObserver<T> _observer;

            protected internal override void Subscribe(string key, Action<object> onMessage)
            {
                Guard.ArgumentNotNull(onMessage, "onMessage");
                if (_observer != null) throw new InvalidOperationException(CHANNEL_ALREADYSUBSCRIBED);

                _observer = string.IsNullOrEmpty(key) ? new ChannelObserver<T>((t) => onMessage(t)) :
                    new ChannelObserver<T>(key, (t) => onMessage(t));
                _observer.Subscribe(ThreadOption.UIThread);
            }

            protected internal override void Dispose(bool disposing)
            {
                if (disposing && _observer != null)
                {
                    _observer.Dispose();
                }
            }
        }

        #endregion

    }
}
