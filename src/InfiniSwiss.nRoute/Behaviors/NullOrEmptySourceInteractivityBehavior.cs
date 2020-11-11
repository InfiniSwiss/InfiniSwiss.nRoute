using Microsoft.Xaml.Behaviors;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;

namespace nRoute.Behaviors
{
    public class NullOrEmptySourceInteractivityBehavior
        : Behavior<UIElement>
    {
        private const double NON_INTERACTIVE_OPACITY = 0d;
        private const double INTERACTIVE_OPACITY = 1d;

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(IEnumerable), typeof(NullOrEmptySourceInteractivityBehavior),
            new PropertyMetadata(null, new PropertyChangedCallback(OnSourceChanged)));

        private bool _negate;
        private double _nonInteractiveOpacity = NON_INTERACTIVE_OPACITY;

        #region Properties

        [Category("Common Properties")]
        [CustomPropertyValueEditor(CustomPropertyValueEditor.PropertyBinding)]
        public IEnumerable Source
        {
            get { return (IEnumerable)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        [Category("Common Properties")]
        public bool Negate
        {
            get { return _negate; }
            set { _negate = value; }
        }

        [Category("Common Properties")]
        public double NonInteractiveOpacity
        {
            get { return _nonInteractiveOpacity; }
            set { _nonInteractiveOpacity = value; }
        }

        #endregion

        #region Trigger Related

        protected override void OnAttached()
        {
            base.OnAttached();
            UpdateInteractivity(Source);
        }

        protected override void OnDetaching()
        {
            var notifiable = Source as INotifyCollectionChanged;
            if (notifiable != null)
            {
                notifiable.CollectionChanged -= OnSourceCollectionChanged;
            }
        }
        #endregion

        #region Handlers

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = (NullOrEmptySourceInteractivityBehavior)d;
            behavior.HandleSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue);
        }

        private void HandleSourceChanged(IEnumerable oldSource, IEnumerable newSource)
        {
            var oldNotifiable = oldSource as INotifyCollectionChanged;
            if (oldNotifiable != null)
            {
                oldNotifiable.CollectionChanged -= OnSourceCollectionChanged;
            }

            var newNotifiable = newSource as INotifyCollectionChanged;
            if (newNotifiable != null)
            {
                newNotifiable.CollectionChanged += OnSourceCollectionChanged;
            }

            UpdateInteractivity(newSource);
        }


        private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateInteractivity((IEnumerable)sender);
        }

        private void UpdateInteractivity(IEnumerable value)
        {
            if (this.AssociatedObject == null) return;

            var _isInteractive = (value != null && value.GetEnumerator().MoveNext());
            if (Negate) _isInteractive = !_isInteractive;
            this.AssociatedObject.Opacity = _isInteractive ? INTERACTIVE_OPACITY : NonInteractiveOpacity;
            this.AssociatedObject.IsHitTestVisible = _isInteractive;
        }

        #endregion

    }
}
