using Microsoft.Xaml.Behaviors;
using nRoute.Components.Handlers;
using nRoute.Internal;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace nRoute.Behaviors.Triggers
{
    public class MouseWheelTrigger
         : TriggerBase<UIElement>
    {
        private const string DELTAFACTOR_NOTBE_LESSTHANZERO = "DeltaFactor cannot be less than 0";
        private const string THROTTLE_NOTBE_LESSTHANZERO = "Throttle cannot be zero or negative duration.";
        private const double DEFAULT_DELTAFACTOR = 120d;
        private static readonly Duration DURATION_ZERO = new Duration(TimeSpan.Zero);

        public static readonly DependencyProperty DeltaFactorProperty =
            DependencyProperty.Register("DeltaFactor", typeof(object), typeof(MouseWheelTrigger),
            new PropertyMetadata(DEFAULT_DELTAFACTOR, new PropertyChangedCallback(OnDeltaFactorChanged)));

        public static readonly DependencyProperty ThrottleProperty =
            DependencyProperty.Register("Throttle", typeof(Duration), typeof(MouseWheelTrigger),
            new PropertyMetadata(DURATION_ZERO));

        private Handler<MouseWheelEventArgs, MouseWheelEventHandler> _mouseWheelHandler;

        #region Properties

        /// <summary>
        /// Factors the delta with the specified value.
        /// </summary>
        [Category("Common Properties")]
        public double DeltaFactor
        {
            get { return Convert.ToDouble(GetValue(DeltaFactorProperty)); }
            set { SetValue(DeltaFactorProperty, value); }
        }

        [Category("Common Properties")]
        public Duration Throttle
        {
            get { return (Duration)(GetValue(ThrottleProperty)); }
            set { SetValue(ThrottleProperty, value); }
        }

        #endregion

        #region Overrides

        protected override void OnAttached()
        {
            base.OnAttached();

            _mouseWheelHandler = new Handler<MouseWheelEventArgs, MouseWheelEventHandler>(OnMouseWheel,
                    (h) => AssociatedObject.MouseWheel -= h);
            AssociatedObject.MouseWheel += _mouseWheelHandler;

            // if throttling is required
            if (Throttle.HasTimeSpan && Throttle != TimeSpan.MaxValue && Throttle > TimeSpan.Zero)
                _mouseWheelHandler.Throttle(Throttle.TimeSpan);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            _mouseWheelHandler.Dispose();
        }

        #endregion

        #region Handlers

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            Double _delta = e.Delta / DeltaFactor;
            base.InvokeActions(_delta);
        }

        private static void OnDeltaFactorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var _newValue = Convert.ToDouble(e.NewValue);
            Guard.ArgumentValue((_newValue < 0), "DeltaFactor", DELTAFACTOR_NOTBE_LESSTHANZERO);
        }

        #endregion

    }

}
