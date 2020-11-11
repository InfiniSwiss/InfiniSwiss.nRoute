using Microsoft.Xaml.Behaviors;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;

namespace nRoute.Behaviors
{
    public class NullOrEmptySourceDisableBehavior
        : Behavior<UIElement>
    {
        private const double DISABLE_OPACITY = 0.4d;
        private const double ENABLE_OPACITY = 1d;

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(IEnumerable), typeof(NullOrEmptySourceDisableBehavior),
            new PropertyMetadata(null, new PropertyChangedCallback(OnSourceChanged)));

        private bool _negate;
        private double _disableOpacity = DISABLE_OPACITY;

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
        public double DisableOpacity
        {
            get { return _disableOpacity; }
            set { _disableOpacity = value; }
        }

        #endregion

        #region Trigger Related

        protected override void OnAttached()
        {
            base.OnAttached();
            UpdateIsDisabled(Source);
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
            var behavior = (NullOrEmptySourceDisableBehavior)d;
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

            UpdateIsDisabled(newSource);
        }

        private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateIsDisabled((IEnumerable)sender);
        }

        private void UpdateIsDisabled(IEnumerable value)
        {
            if (this.AssociatedObject == null) return;

            var _isDisabled = (value == null || !value.GetEnumerator().MoveNext());
            if (Negate) _isDisabled = !_isDisabled;
            AssociatedObject.IsEnabled = !_isDisabled;
        }

        #endregion

    }
}
