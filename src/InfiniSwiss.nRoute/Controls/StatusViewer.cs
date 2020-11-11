using nRoute.Components;
using nRoute.Internal;
using nRoute.ViewServices.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace nRoute.Controls
{
    public class StatusViewer
        : Control, IShowStatusViewService
    {
        protected internal const string HIDDEN_STATE = "Hidden";
        protected internal const string SHOWING_STATE = "Showing";
        protected internal const string STATUS_STATES = "StatusStates";
        protected internal const string STATUS_CONTAINER_PART = "StatusContainer";

        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register("Status",
            typeof(string), typeof(StatusViewer), new PropertyMetadata(null, OnStatusChanged));

        public static readonly DependencyProperty AllowAcknowledgementProperty = DependencyProperty.Register("AllowAcknowledgement",
            typeof(bool), typeof(StatusViewer), new PropertyMetadata(true));

        private readonly Object _lock = new Object();
        private Queue<WeakReference> _statuses;
        private StatusInfo _currentStatus;

        private UIElement _statusContainer;

        public event EventHandler StatusAcknowledged;

        public StatusViewer()
        {
            // we set the default style
            this.DefaultStyleKey = typeof(StatusViewer);
        }

        #region Properties

        [Category("Common Properties")]
        public string Status
        {
            get { return Convert.ToString(GetValue(StatusProperty)); }
            set { SetValue(StatusProperty, value); }
        }

        [Category("Common Properties")]
        public bool AllowAcknowledgement
        {
            get { return Convert.ToBoolean(GetValue(AllowAcknowledgementProperty)); }
            set { SetValue(AllowAcknowledgementProperty, value); }
        }

        #endregion

        #region Override/able

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (!IsInDesignMode()) UpdateStatus(this.Status);

            _statusContainer = this.GetTemplateChild(STATUS_CONTAINER_PART) as UIElement;
            if (_statusContainer != null)
            {
                _statusContainer.MouseLeftButtonUp += StatusContainer_MouseLeftButtonUp;
            }
        }

        /// <summary>
        /// Hides currently visible status.
        /// </summary>
        public virtual void HideStatus()
        {
            lock (_lock)
            {
                if (_currentStatus != null)
                {
                    _currentStatus.Dispose(true);
                }
                else
                {
                    this.Status = null;
                    ProcessQueue();
                }
            }
        }

        #endregion

        #region IShowStatusViewService related

        IDisposable IShowStatusViewService.ShowStatus(string status)
        {
            return ((IShowStatusViewService)this).ShowStatus(status, TimeSpan.Zero, null);
        }

        IDisposable IShowStatusViewService.ShowStatus(string status, TimeSpan timeout)
        {
            return ((IShowStatusViewService)this).ShowStatus(status, timeout, null);
        }

        IDisposable IShowStatusViewService.ShowStatus(string status, Action acknowledgementCallback)
        {
            return ((IShowStatusViewService)this).ShowStatus(status, TimeSpan.Zero, acknowledgementCallback);
        }

        IDisposable IShowStatusViewService.ShowStatus(string status, TimeSpan timeout, Action acknowledgementCallback)
        {
            Guard.ArgumentNotNullOrWhiteSpace(status, "status");

            var _statusInfo = new StatusInfo(status, timeout, acknowledgementCallback);

            lock (_lock)
            {
                // lazy load, also note we only keep a weak reference to the status info
                if (_statuses == null) _statuses = new Queue<WeakReference>();
                _statuses.Enqueue(new WeakReference(_statusInfo));

                ProcessQueue();
            }

            return _statusInfo;
        }

        #endregion

        #region Handlers

        private void StatusContainer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (AllowAcknowledgement && _currentStatus != null)
            {
                if (StatusAcknowledged != null) StatusAcknowledged(this, EventArgs.Empty);
                HideStatus();
            }
        }

        private void CurrentStatus_Disposed(object sender, EventArgs e)
        {
            lock (_lock)
            {
                if (_currentStatus != null)
                {
                    _currentStatus.Disposed -= CurrentStatus_Disposed;
                    _currentStatus = null;
                }

                this.Status = null;
                ProcessQueue();
            }
        }

        protected virtual void UpdateStatus(string status)
        {
            VisualStateManager.GoToState(this, !string.IsNullOrEmpty(status) ? SHOWING_STATE : HIDDEN_STATE, true);
        }

        private static void OnStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StatusViewer)d).UpdateStatus(Convert.ToString(e.NewValue));
        }

        #endregion

        #region Helpers

        private void ProcessQueue()
        {
            lock (_lock)
            {
                if (_currentStatus != null || _statuses == null || _statuses.Count == 0) return;

                var _statusReference = _statuses.Dequeue();
                if (_statusReference != null && _statusReference.IsAlive)
                {
                    _currentStatus = (StatusInfo)_statusReference.Target;
                    if (_currentStatus != null)
                    {
                        if (_currentStatus.IsDisposed)
                        {
                            this.Status = null;
                            _currentStatus = null;
                        }
                        else
                        {
                            this.Status = _currentStatus.Status;
                            _currentStatus.Disposed += CurrentStatus_Disposed;
                            _currentStatus.Initialize();
                        }
                    }

                    _statusReference.Target = null;
                    _statusReference = null;
                }

                // if no current active status, the process queue
                if (_currentStatus == null) ProcessQueue();
            }
        }

        protected bool IsInDesignMode()
        {
            if ((Application.Current != null) && (Application.Current.GetType() != typeof(Application)))
            {
                return DesignerProperties.GetIsInDesignMode(this);
            }
            return true;
        }

        #endregion

        #region Internal

        private class StatusInfo
            : IDisposableToken
        {
            private readonly string _status;
            private readonly TimeSpan _timeout;
            private WeakReference _callbackReference;
            private bool _isDisposed;

            public event EventHandler Disposed;

            public StatusInfo(string status, TimeSpan timeout, Action callback)
            {
                _status = status;
                _timeout = timeout;

                // note, we are using a weak-reference - this is to avoid memory leaks when the status caller is no
                // longer available - however, this also means anonymous callbacks could easily get GC'ed..
                // So the choice is between potential memory leak or a somewhat predictable behavior?
                if (callback != null) _callbackReference = new WeakReference(callback);
            }

            #region Public

            public string Status
            {
                get { return _status; }
            }

            public void Initialize()
            {
                if (_timeout > TimeSpan.Zero)
                {
                    var _dispatchTimer = new DispatcherTimer() { Interval = _timeout };
                    _dispatchTimer.Tick += (s, e) =>
                    {
                        _dispatchTimer.Stop();
                        _dispatchTimer = null;
                        Dispose();
                    };
                    _dispatchTimer.Start();
                }
            }

            #endregion

            #region IDisposable Related

            public bool IsDisposed
            {
                get { return _isDisposed; }
            }

            public void Dispose()
            {
                this.Dispose(false);
                GC.SuppressFinalize(this);
            }

            public void Dispose(bool withCallback)
            {
                if (!_isDisposed)
                {
                    if (_callbackReference != null && _callbackReference.IsAlive)
                    {
                        var _callback = (Action)_callbackReference.Target;
                        if (withCallback && _callback != null) _callback();

                        _callbackReference.Target = null;
                        _callbackReference = null;
                    }

                    if (Disposed != null) Disposed(this, EventArgs.Empty);
                    Disposed = null;
                    _isDisposed = true;
                }
            }

            #endregion

        }

        #endregion

    }
}