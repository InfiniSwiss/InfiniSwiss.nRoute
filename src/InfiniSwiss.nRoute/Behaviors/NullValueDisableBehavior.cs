using Microsoft.Xaml.Behaviors;
using System;
using System.ComponentModel;
using System.Windows;

namespace nRoute.Behaviors
{
    public class NullValueDisableBehavior
        : Behavior<UIElement>
    {
        private const double DISABLE_OPACITY = 0.4d;
        private const double ENABLE_OPACITY = 1d;

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(NullValueDisableBehavior),
            new PropertyMetadata(null, new PropertyChangedCallback(OnValueChanged)));

        private bool _negate;
        private double _disableOpacity = DISABLE_OPACITY;

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
            UpdateIsDisabled(Value);
        }

        #endregion

        #region Handlers

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((NullValueDisableBehavior)d).UpdateIsDisabled(e.NewValue);
        }

        private void UpdateIsDisabled(object value)
        {
            if (this.AssociatedObject == null) return;

            var _isDisabled = (value == null);
            if (Negate) _isDisabled = !_isDisabled;

            AssociatedObject.IsEnabled = !_isDisabled;
        }

        #endregion

    }
}
