using Microsoft.Xaml.Behaviors;
using nRoute.Components.TypeConverters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Markup;

namespace nRoute.Behaviors
{
    [ContentProperty("Value")]
    public class SetPropertyAction
        : TriggerAction<DependencyObject>
    {
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.Register("PropertyName", typeof(string), typeof(SetPropertyAction),
            new PropertyMetadata(null));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(SetPropertyAction),
            new PropertyMetadata(null));

        #region Properties

        // it might be possible to do
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
            if (string.IsNullOrEmpty(this.PropertyName) || this.AssociatedObject == null)
            {
                return;
            }

            if (this.PropertyName.StartsWith("(") && this.PropertyName.EndsWith(")"))
            {
                this.SetAttachedPropertyValue();
            }
            else
            {
                this.SetRegularPropertyValue();
            }
        }

        private void SetAttachedPropertyValue()
        {
            var propertyPath = this.PropertyName.Substring(1, this.PropertyName.Length - 2);
            var propertyParts = propertyPath.Split('.');
            var ownerTypeName = propertyParts[0];
            var propertyName = propertyParts[1];

            if (!dependencyPropertyOwnerTypeCache.TryGetValue(ownerTypeName, out var ownerType))
            {
                ownerType = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).SelectMany(a => a.GetTypes()).FirstOrDefault(t => t.Name.Equals(ownerTypeName));
                dependencyPropertyOwnerTypeCache.Add(ownerTypeName, ownerType);
            }

            if (ownerType == null)
            {
                return;
            }

            var propertyDescriptor = DependencyPropertyDescriptor.FromName(propertyName, ownerType, this.AssociatedObject.GetType());
            if (propertyDescriptor != null)
            {
                var value = Value != null ? ConverterHelper.ConvertToType(Value, propertyDescriptor.PropertyType) : null;
                propertyDescriptor.SetValue(this.AssociatedObject, value);
            }
        }

        private void SetRegularPropertyValue()
        {
            // we set the value
            var propertyInfo = this.AssociatedObject.GetType().GetProperty(this.PropertyName);
            if (propertyInfo == null || !propertyInfo.CanWrite)
            {
                return;
            }

            // else we convert and set the value
            var value = Value != null ? ConverterHelper.ConvertToType(Value, propertyInfo.PropertyType) : null;
            propertyInfo.SetValue(this.AssociatedObject, value, null);
        }
        #endregion

        private static readonly Dictionary<string, Type> dependencyPropertyOwnerTypeCache = new Dictionary<string, Type>();
    }
}
