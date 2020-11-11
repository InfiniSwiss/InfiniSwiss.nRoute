using Microsoft.Xaml.Behaviors;
using System;
using System.ComponentModel;
using System.Windows;

namespace nRoute.Behaviors
{
    public class BoolValueInteractivityBehavior
        : Behavior<UIElement>
    {
        private const double NON_INTERACTIVE_OPACITY = 0d;
        private const double INTERACTIVE_OPACITY = 1d;

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(bool), typeof(BoolValueInteractivityBehavior),
            new PropertyMetadata(false, new PropertyChangedCallback(OnValueChanged)));

        private bool _negate;
        private double _nonInteractiveOpacity = NON_INTERACTIVE_OPACITY;

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
            UpdateInteractivity(Value);
        }

        #endregion

        #region Handlers

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BoolValueInteractivityBehavior)d).UpdateInteractivity(Convert.ToBoolean(e.NewValue));
        }

        private void UpdateInteractivity(bool value)
        {
            if (this.AssociatedObject == null) return;

            var _isInteractive = value;
            if (Negate) _isInteractive = !_isInteractive;
            this.AssociatedObject.Opacity = _isInteractive ? INTERACTIVE_OPACITY : NonInteractiveOpacity;
            this.AssociatedObject.IsHitTestVisible = _isInteractive;
        }

        #endregion

    }
}
