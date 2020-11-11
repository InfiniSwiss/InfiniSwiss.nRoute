using Microsoft.Xaml.Behaviors;
using System;
using System.ComponentModel;
using System.Windows;

namespace nRoute.Behaviors
{
    public class BoolValueDisableBehavior
        : Behavior<UIElement>
    {
        private const double DISABLE_OPACITY = 0.4d;
        private const double ENABLE_OPACITY = 1d;

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(bool), typeof(BoolValueDisableBehavior),
            new PropertyMetadata(false, new PropertyChangedCallback(OnValueChanged)));

        private bool _negate;
        private double _disableOpacity = DISABLE_OPACITY;

        #region Properties

        [Category("Common Properties")]
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
            ((BoolValueDisableBehavior)d).UpdateIsDisabled(Convert.ToBoolean(e.NewValue));
        }

        private void UpdateIsDisabled(bool value)
        {
            if (this.AssociatedObject == null) return;

            var _isDisabled = value;
            if (Negate) _isDisabled = !_isDisabled;

            if (AssociatedObject is UIElement)
            {
                (AssociatedObject as UIElement).IsEnabled = !_isDisabled;
            }
            else
            {
                this.AssociatedObject.Opacity = !_isDisabled ? ENABLE_OPACITY : DisableOpacity;
                this.AssociatedObject.IsHitTestVisible = !_isDisabled;
            }
        }

        #endregion

    }
}
