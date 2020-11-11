using Microsoft.Xaml.Behaviors;
using nRoute.Components.TypeConverters;
using System;
using System.ComponentModel;
using System.Windows;

namespace nRoute.Behaviors.Triggers
{
    /// <summary>
    /// Represents a trigger that can match a value being equal to the given value.
    /// </summary>
    public class ValueMatchTrigger
        : TriggerBase<DependencyObject>
    {
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(object), typeof(ValueMatchTrigger),
            new PropertyMetadata(null, OnSourceChanged));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(ValueMatchTrigger),
            new PropertyMetadata(null, OnValueChanged));

        private bool _negate;

        #region Properties

        [Category("Common Properties")]
        [CustomPropertyValueEditor(CustomPropertyValueEditor.PropertyBinding)]
        public object Source
        {
            get { return GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

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

        protected override void OnAttached()
        {
            base.OnAttached();
            this.CompareValue();
        }

        #region Handlers

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ValueMatchTrigger)d).CompareValue();
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ValueMatchTrigger)d).CompareValue();
        }

        private void CompareValue()
        {
            if (this.AssociatedObject == null || Source == null)
            {
                return;
            }

            // we need to convert it so that the matching can be done
            var _matchValue = ConverterHelper.ConvertToType(Value, Source.GetType());
            var _result = object.Equals(_matchValue, Source);
            if (Negate) _result = !_result;
            if (_result) base.InvokeActions(Source);
        }

        #endregion

    }
}
