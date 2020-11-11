using Microsoft.Xaml.Behaviors;
using nRoute.Components.Handlers;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace nRoute.Behaviors.Triggers
{
    public class KeyTrigger
         : TriggerBase<UIElement>
    {
        private static readonly Duration DURATION_ZERO = new(TimeSpan.Zero);

        public static readonly DependencyProperty KeyEventProperty =
           DependencyProperty.Register("KeyEvent", typeof(KeyEventType), typeof(KeyTrigger),
           new PropertyMetadata(KeyEventType.KeyUp));

        public static readonly DependencyProperty KeyCodeProperty =
            DependencyProperty.Register("KeyCode", typeof(int), typeof(KeyTrigger),
            new PropertyMetadata(0));

        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register("Key", typeof(Key), typeof(KeyTrigger),
            new PropertyMetadata(Key.None));

        public static readonly DependencyProperty WithAltModifierProperty =
            DependencyProperty.Register("WithAltModifier", typeof(bool), typeof(KeyTrigger),
            new PropertyMetadata(false));

        public static readonly DependencyProperty WithControlModifierProperty =
            DependencyProperty.Register("WithControlModifier", typeof(bool), typeof(KeyTrigger),
            new PropertyMetadata(false));

        public static readonly DependencyProperty WithShiftModifierProperty =
            DependencyProperty.Register("WithShiftModifier", typeof(bool), typeof(KeyTrigger),
            new PropertyMetadata(false));

        public static readonly DependencyProperty WithWindowsModifierProperty =
            DependencyProperty.Register("WithWindowsModifier", typeof(bool), typeof(KeyTrigger),
            new PropertyMetadata(false));

        public static readonly DependencyProperty WithAppleModifierProperty =
            DependencyProperty.Register("WithAppleModifier", typeof(bool), typeof(KeyTrigger),
            new PropertyMetadata(false));

        public static readonly DependencyProperty SetIsHandledProperty =
            DependencyProperty.Register("SetIsHandled", typeof(bool), typeof(KeyTrigger),
            new PropertyMetadata(false));

        public static readonly DependencyProperty ThrottleProperty =
            DependencyProperty.Register("Throttle", typeof(Duration), typeof(KeyTrigger),
            new PropertyMetadata(DURATION_ZERO));

        private Handler<KeyEventArgs, KeyEventHandler> _keyHandler;
        private bool _negate;

        #region Properties

        [Category("Common Properties")]
        public KeyEventType KeyEvent
        {
            get { return (KeyEventType)(GetValue(KeyEventProperty)); }
            set { SetValue(KeyEventProperty, value); }
        }

        [Category("Common Properties")]
        public int KeyCode
        {
            get { return Convert.ToInt32(GetValue(KeyCodeProperty)); }
            set { SetValue(KeyCodeProperty, value); }
        }

        [Category("Common Properties")]
        public Key Key
        {
            get { return (Key)(GetValue(KeyProperty)); }
            set { SetValue(KeyProperty, value); }
        }

        [Category("Common Properties")]
        public bool WithAltModifier
        {
            get { return Convert.ToBoolean(GetValue(WithAltModifierProperty)); }
            set { SetValue(WithAltModifierProperty, value); }
        }

        [Category("Common Properties")]
        public bool WithControlModifier
        {
            get { return Convert.ToBoolean(GetValue(WithControlModifierProperty)); }
            set { SetValue(WithControlModifierProperty, value); }
        }

        [Category("Common Properties")]
        public bool WithShiftModifier
        {
            get { return Convert.ToBoolean(GetValue(WithShiftModifierProperty)); }
            set { SetValue(WithShiftModifierProperty, value); }
        }

        [Category("Common Properties")]
        public bool WithWindowsModifier
        {
            get { return Convert.ToBoolean(GetValue(WithWindowsModifierProperty)); }
            set { SetValue(WithWindowsModifierProperty, value); }
        }

        /// <remarks>Ignored in non-Silverlight Apps</remarks>
        [Category("Common Properties")]
        public bool WithAppleModifier
        {
            get { return Convert.ToBoolean(GetValue(WithAppleModifierProperty)); }
            set { SetValue(WithAppleModifierProperty, value); }
        }

        [Category("Common Properties")]
        public bool SetIsHandled
        {
            get { return Convert.ToBoolean(GetValue(SetIsHandledProperty)); }
            set { SetValue(SetIsHandledProperty, value); }
        }

        [Category("Common Properties")]
        [TypeConverter(typeof(DurationConverter))]
        public Duration Throttle
        {
            get { return (Duration)(GetValue(ThrottleProperty)); }
            set { SetValue(ThrottleProperty, value); }

        }

        [Category("Common Properties")]
        public bool Negate
        {
            get { return _negate; }
            set { _negate = value; }
        }

        #endregion

        #region Overrides

        protected override void OnAttached()
        {
            base.OnAttached();

            if (KeyEvent == KeyEventType.KeyUp)
            {
                _keyHandler = new Handler<KeyEventArgs, KeyEventHandler>(OnKeyEvent,
                    (h) => AssociatedObject.KeyUp -= h);
                AssociatedObject.KeyUp += _keyHandler;
            }
            else
            {
                _keyHandler = new Handler<KeyEventArgs, KeyEventHandler>(OnKeyEvent,
                    (h) => AssociatedObject.KeyDown -= h);
                AssociatedObject.KeyDown += _keyHandler;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            _keyHandler.Dispose();
        }

        #endregion

        #region Handler

        private void OnKeyEvent(object sender, KeyEventArgs e)
        {

            bool _isMatch = (KeyCode != 0) ? (KeyCode == Convert.ToInt32(e.Key)) : (e.Key == Key);

            // match the modifiers
            var _modifiers = Keyboard.Modifiers;

            if (WithAltModifier && (_modifiers & ModifierKeys.Alt) != ModifierKeys.Alt) _isMatch = false;
            if (!WithAltModifier && (_modifiers & ModifierKeys.Alt) == ModifierKeys.Alt) _isMatch = false;

            if (WithControlModifier && (_modifiers & ModifierKeys.Control) != ModifierKeys.Control) _isMatch = false;
            if (!WithControlModifier && (_modifiers & ModifierKeys.Control) == ModifierKeys.Control) _isMatch = false;

            if (WithShiftModifier && (_modifiers & ModifierKeys.Shift) != ModifierKeys.Shift) _isMatch = false;
            if (!WithShiftModifier && (_modifiers & ModifierKeys.Shift) == ModifierKeys.Shift) _isMatch = false;

            if (WithWindowsModifier && (_modifiers & ModifierKeys.Windows) != ModifierKeys.Windows) _isMatch = false;
            if (!WithWindowsModifier && (_modifiers & ModifierKeys.Windows) == ModifierKeys.Windows) _isMatch = false;

            // we flow down till here, as we might need to negate the match
            if (Negate) _isMatch = !_isMatch;
            if (!_isMatch) return;

            // raise
            base.InvokeActions(e.Key);

            // handled
            if (SetIsHandled) e.Handled = true;
        }

        #endregion

    }
}
