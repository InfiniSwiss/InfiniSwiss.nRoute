using nRoute.ViewServices.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace nRoute.Controls
{
    public class WorkIndicator
        : Control, IShowIndicatorViewService
    {
        protected internal const string NONWORKING_STATE = "NonWorking";
        protected internal const string WORKING_STATE = "Working";
        protected internal const string INDICATOR_STATES = "IndicatorStates";
        protected internal const string TITLE_CONTAINER_PART = "TitleContainer";

        public static readonly DependencyProperty IsWorkingProperty = DependencyProperty.Register("IsWorking",
            typeof(bool), typeof(WorkIndicator), new PropertyMetadata(false, OnWorkingStateChanged));

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title",
            typeof(Object), typeof(WorkIndicator), new PropertyMetadata(null));

        public static readonly DependencyProperty TitleStyleProperty = DependencyProperty.Register("TitleStyle",
            typeof(Style), typeof(WorkIndicator), new PropertyMetadata(null));

        public static readonly DependencyProperty ShowTitleProperty = DependencyProperty.Register("ShowTitle",
            typeof(bool), typeof(WorkIndicator), new PropertyMetadata(false, OnShowTitleStateChanged));

        public static readonly DependencyProperty IndicatorForegroundProperty = DependencyProperty.Register("IndicatorForeground",
            typeof(Brush), typeof(WorkIndicator), new PropertyMetadata(null));

        private readonly Object _lock = new Object();
        private Queue<WeakReference> _indicators;
        private IndicatorInfo _currentIndicator;

        private ContentControl _titleContainer;

        public WorkIndicator()
        {
            // we set the default style
            this.DefaultStyleKey = typeof(WorkIndicator);
        }

        #region Properties

        [Category("Common Properties")]
        public bool IsWorking
        {
            get { return Convert.ToBoolean(GetValue(IsWorkingProperty)); }
            set { SetValue(IsWorkingProperty, value); }
        }

        [Category("Common Properties")]
        public object Title
        {
            get { return GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        //[Category("Appearance")]
        public Style TitleStyle
        {
            get { return (Style)GetValue(TitleStyleProperty); }
            set { SetValue(TitleStyleProperty, value); }
        }

        [Category("Appearance")]
        public bool ShowTitle
        {
            get { return Convert.ToBoolean(GetValue(ShowTitleProperty)); }
            set { SetValue(ShowTitleProperty, value); }
        }

        #endregion

        #region Override/able

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (!IsInDesignMode()) UpdateWorkingState(this.IsWorking);

            _titleContainer = this.GetTemplateChild(TITLE_CONTAINER_PART) as ContentControl;
            if (_titleContainer != null && !IsInDesignMode()) UpdateShowTitle(this.ShowTitle);
        }

        /// <summary>
        /// Hides currently visible indicator.
        /// </summary>
        public virtual void HideIndicator()
        {
            lock (_lock)
            {
                if (_currentIndicator != null)
                {
                    _currentIndicator.Dispose();
                }
                else
                {
                    this.Title = null;
                    this.IsWorking = false;
                    ProcessQueue();
                }
            }
        }

        #endregion

        #region IIndicatorViewService related

        IDisposable IShowIndicatorViewService.ShowIndicator()
        {
            return ((IShowIndicatorViewService)this).ShowIndicator(null, TimeSpan.Zero);
        }

        IDisposable IShowIndicatorViewService.ShowIndicator(TimeSpan timeout)
        {
            return ((IShowIndicatorViewService)this).ShowIndicator(null, timeout);
        }

        IDisposable IShowIndicatorViewService.ShowIndicator(string title)
        {
            return ((IShowIndicatorViewService)this).ShowIndicator(title, TimeSpan.Zero);
        }

        IDisposable IShowIndicatorViewService.ShowIndicator(string title, TimeSpan timeout)
        {
            var _indicatorInfo = new IndicatorInfo(title, timeout);

            lock (_lock)
            {
                // lazy load, also note we only keep a weak reference to the indicator info
                if (_indicators == null) _indicators = new Queue<WeakReference>();
                _indicators.Enqueue(new WeakReference(_indicatorInfo));

                ProcessQueue();
            }

            // we initialize and return - note unlike status viewer, the timers are run together and not one-by-one 
            _indicatorInfo.Initialize();
            return _indicatorInfo;
        }

        #endregion

        #region Handlers

        private void CurrentIndicator_Disposed(object sender, EventArgs e)
        {
            lock (_lock)
            {
                if (_currentIndicator != null)
                {
                    _currentIndicator.Disposed -= CurrentIndicator_Disposed;
                    _currentIndicator = null;
                }

                this.Title = null;
                this.IsWorking = false;
                ProcessQueue();
            }
        }

        protected virtual void UpdateWorkingState(bool isWorking)
        {
            VisualStateManager.GoToState(this, isWorking ? WORKING_STATE : NONWORKING_STATE, true);
        }

        private static void OnWorkingStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((WorkIndicator)d).UpdateWorkingState(Convert.ToBoolean(e.NewValue));
        }

        protected virtual void UpdateShowTitle(bool showTitle)
        {
            if (_titleContainer != null) _titleContainer.Visibility = showTitle ? Visibility.Visible : Visibility.Collapsed;
        }

        private static void OnShowTitleStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((WorkIndicator)d).UpdateShowTitle(Convert.ToBoolean(e.NewValue));
        }

        #endregion

        #region Helpers

        private void ProcessQueue()
        {
            lock (_lock)
            {
                if (_currentIndicator != null || _indicators == null || _indicators.Count == 0) return;

                var _indicatorReference = _indicators.Dequeue();
                if (_indicatorReference != null && _indicatorReference.IsAlive)
                {
                    _currentIndicator = (IndicatorInfo)_indicatorReference.Target;
                    if (_currentIndicator != null)
                    {
                        if (_currentIndicator.IsDisposed)
                        {
                            this.Title = null;
                            _currentIndicator = null;
                        }
                        else
                        {
                            this.Title = _currentIndicator.Title;
                            _currentIndicator.Disposed += CurrentIndicator_Disposed;
                            _currentIndicator.Initialize();
                        }
                    }

                    _indicatorReference.Target = null;
                    _indicatorReference = null;
                }

                // if no current active indicator, the process queue
                this.IsWorking = (_currentIndicator != null);
                if (!this.IsWorking) ProcessQueue();
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

        private class IndicatorInfo
            : IDisposable
        {
            private readonly string _title;
            private readonly TimeSpan _timeout;
            private bool _isDisposed;

            public event EventHandler Disposed;

            public IndicatorInfo(string title, TimeSpan timeout)
            {
                _title = title;
                _timeout = timeout;
            }

            #region Public

            public string Title
            {
                get { return _title; }
            }

            public bool IsDisposed
            {
                get { return _isDisposed; }
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

            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (!_isDisposed)
                    {
                        if (Disposed != null) Disposed(this, EventArgs.Empty);
                        Disposed = null;
                        _isDisposed = true;
                    }
                }
            }

            #endregion

        }

        #endregion

    }
}