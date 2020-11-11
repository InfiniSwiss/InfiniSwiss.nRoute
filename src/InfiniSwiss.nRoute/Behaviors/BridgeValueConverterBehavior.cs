using Microsoft.Xaml.Behaviors;
using nRoute.Components;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace nRoute.Behaviors
{
    public class BridgeValueConverterBehavior
        : Behavior<DependencyObject>
    {
        public static readonly DependencyProperty ValueConverterRelayProperty =
            DependencyProperty.Register("ValueConverterRelay", typeof(ValueConverterRelay), typeof(BridgeValueConverterBehavior),
                new PropertyMetadata(null, OnValueConverterRelayChanged));

        public static readonly DependencyProperty ConverterSourceProperty =
            DependencyProperty.Register("ConverterSource", typeof(IValueConverter), typeof(BridgeValueConverterBehavior),
                new PropertyMetadata(null, OnConverterSourceChanged));

        #region Properties

        [Category("Common Properties")]
        public ValueConverterRelay ValueConverterRelay
        {
            get { return (ValueConverterRelay)GetValue(ValueConverterRelayProperty); }
            set { SetValue(ValueConverterRelayProperty, value); }
        }

        [Category("Common Properties")]
        [CustomPropertyValueEditor(CustomPropertyValueEditor.PropertyBinding)]
        public IValueConverter ConverterSource
        {
            get { return (IValueConverter)GetValue(ConverterSourceProperty); }
            set { SetValue(ConverterSourceProperty, value); }
        }

        #endregion

        #region Handlers

        private static void OnConverterSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BridgeValueConverterBehavior)d).UpdateRelay();
        }

        private static void OnValueConverterRelayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BridgeValueConverterBehavior)d).UpdateRelay();
        }

        #endregion

        #region Overrides

        protected override void OnAttached()
        {
            base.OnAttached();
            UpdateRelay();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
        }

        #endregion

        #region Helpers

        private void UpdateRelay()
        {
            if (ValueConverterRelay != null && this.ConverterSource != null)
            {
                ValueConverterRelay.Converter = ConverterSource;
            }
        }

        #endregion

    }
}
