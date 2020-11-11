using Microsoft.Xaml.Behaviors;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;

namespace nRoute.Behaviors
{
    public class NullOrEmptySourceVisibilityBehavior
        : Behavior<UIElement>
    {
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(IEnumerable), typeof(NullOrEmptySourceVisibilityBehavior),
            new PropertyMetadata(null, new PropertyChangedCallback(OnSourceChanged)));

        private bool _negate;

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

        #endregion

        #region Trigger Related

        protected override void OnAttached()
        {
            base.OnAttached();
            UpdateVisibility(Source);
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
            var behavior = (NullOrEmptySourceVisibilityBehavior)d;
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

            UpdateVisibility(newSource);
        }

        private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateVisibility((IEnumerable)sender);
        }

        private void UpdateVisibility(IEnumerable value)
        {
            if (this.AssociatedObject == null) return;

            var _visible = (value != null && value.GetEnumerator().MoveNext());
            if (Negate) _visible = !_visible;
            this.AssociatedObject.Visibility = _visible ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion

    }
}
