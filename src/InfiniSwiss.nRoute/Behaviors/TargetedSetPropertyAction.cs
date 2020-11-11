using Microsoft.Xaml.Behaviors;
using nRoute.Components.TypeConverters;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;
namespace nRoute.Behaviors
{
    [ContentProperty("Value")]
    public class TargetedSetPropertyAction
        : TargetedTriggerAction<DependencyObject>
    {
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.Register("PropertyName", typeof(string), typeof(TargetedSetPropertyAction),
            new PropertyMetadata(null));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(TargetedSetPropertyAction),
            new PropertyMetadata(null));

        #region Properties

        // it might be possible to de
        //public Binding TargetBinding { get; set; }

        [Category("Common Properties")]
        public string PropertyName
        {
            get { return Convert.ToString(GetValue(PropertyNameProperty)); }
            set { SetValue(PropertyNameProperty, value); }
        }

        [Category("Common Properties")]
        [CustomPropertyValueEditor(CustomPropertyValueEditor.PropertyBinding)]
        public Object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        #endregion

        #region Trigger Related

        protected override void Invoke(object parameter)
        {
            if (string.IsNullOrEmpty(this.PropertyName) || this.Target == null) return;

            // we set the value
            var _propertyInfo = this.Target.GetType().GetProperty(this.PropertyName);
            if (_propertyInfo == null || !_propertyInfo.CanWrite) return;

            // else we convert and set the value
            var _value = Value != null ? ConverterHelper.ConvertToType(Value, _propertyInfo.PropertyType) : null;
            _propertyInfo.SetValue(this.Target, _value, null);
        }

        #endregion

    }
}
