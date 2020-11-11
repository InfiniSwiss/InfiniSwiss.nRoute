using Microsoft.Xaml.Behaviors;
using System;
using System.ComponentModel;
using System.Windows;

namespace nRoute.Behaviors
{
    public class NullValueVisibilityBehavior
        : Behavior<UIElement>
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(NullValueVisibilityBehavior),
            new PropertyMetadata(null, new PropertyChangedCallback(OnValueChanged)));

        private bool _negate;

        #region Properties

        [Category("Common Properties")]
        [CustomPropertyValueEditor(CustomPropertyValueEditor.PropertyBinding)]
        public Object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
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
            UpdateVisibility(Value);
        }

        #endregion

        #region Handlers

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((NullValueVisibilityBehavior)d).UpdateVisibility(e.NewValue);
        }

        private void UpdateVisibility(object value)
        {
            if (this.AssociatedObject == null) return;

            var _visible = (value != null);
            if (Negate) _visible = !_visible;
            this.AssociatedObject.Visibility = _visible ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion

    }
}
