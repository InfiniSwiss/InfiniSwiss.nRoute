using Microsoft.Xaml.Behaviors;
using nRoute.Components;
using System.ComponentModel;
using System.Windows;

namespace nRoute.Behaviors
{
    public class BridgeValueBehavior
        : Behavior<DependencyObject>
    {
        public static readonly DependencyProperty ValueRelayProperty =
            DependencyProperty.Register("ValueRelay", typeof(ValueRelay), typeof(BridgeValueBehavior),
                new PropertyMetadata(null, OnValueRelayChanged));

        public static readonly DependencyProperty ValueSourceProperty =
            DependencyProperty.Register("ValueSource", typeof(object), typeof(BridgeValueBehavior),
                new PropertyMetadata(null, OnValueSourceChanged));

        #region Properties

        [Category("Common Properties")]
        public ValueRelay ValueRelay
        {
            get { return (ValueRelay)GetValue(ValueRelayProperty); }
            set { SetValue(ValueRelayProperty, value); }
        }

        [Category("Common Properties")]
        [CustomPropertyValueEditor(CustomPropertyValueEditor.PropertyBinding)]
        public object ValueSource
        {
            get { return GetValue(ValueSourceProperty); }
            set { SetValue(ValueSourceProperty, value); }
        }

        #endregion

        #region Handlers

        private static void OnValueSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BridgeValueBehavior)d).UpdateRelay();
        }

        private static void OnValueRelayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BridgeValueBehavior)d).UpdateRelay();
        }

        #endregion

        #region Overrides

        protected override void OnAttached()
        {
            base.OnAttached();
            UpdateRelay();
        }

        #endregion

        #region Helpers

        private void UpdateRelay()
        {
            if (ValueRelay != null && this.ValueSource != null)
            {
                ValueRelay.Value = ValueSource;
            }
        }

        #endregion

    }
}
