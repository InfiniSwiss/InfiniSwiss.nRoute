using Microsoft.Xaml.Behaviors;
using System;
using System.ComponentModel;
using System.Windows;

namespace nRoute.Behaviors
{
    public class BoolValueVisibilityBehavior
        : Behavior<UIElement>
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(bool), typeof(BoolValueVisibilityBehavior),
            new PropertyMetadata(false, new PropertyChangedCallback(OnValueChanged)));

        private bool _negate;

        #region Properties

        [Category("Common Properties")]
        [CustomPropertyValueEditor(CustomPropertyValueEditor.PropertyBinding)]
        public bool Value
        {
            get { return Convert.ToBoolean(GetValue(ValueProperty)); }
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
            ((BoolValueVisibilityBehavior)d).UpdateVisibility(Convert.ToBoolean(e.NewValue));
        }

        private void UpdateVisibility(bool value)
        {
            if (this.AssociatedObject == null) return;

            var _visible = value;
            if (Negate) _visible = !_visible;
            this.AssociatedObject.Visibility = _visible ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion

    }
}
