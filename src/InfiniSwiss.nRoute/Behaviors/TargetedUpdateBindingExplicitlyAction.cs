using Microsoft.Xaml.Behaviors;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;

namespace nRoute.Behaviors
{
    public class TargetedUpdateBindingExplicitlyAction
        : TargetedTriggerAction<FrameworkElement>
    {
        private const string PROPERTY_POSTFIX = "Property";
        private const string DP_NOTFOUND_ERROR_FORMAT = "Dependency Property for Property Named '{0}' not found.";

        [Category("Common Properties")]
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.Register("PropertyName", typeof(string), typeof(TargetedUpdateBindingExplicitlyAction),
            new PropertyMetadata(null, new PropertyChangedCallback(OnPropertyNameChanged)));

        private DependencyProperty _dependencyProperty;

        #region Properties

        [Category("Common Properties")]
        public string PropertyName
        {
            get { return Convert.ToString(GetValue(PropertyNameProperty)); }
            set { SetValue(PropertyNameProperty, value); }
        }

        #endregion

        #region Trigger Related

        protected override void Invoke(object parameter)
        {
            if (this.Target == null) return;

            // we use the property name, and 
            if (string.IsNullOrEmpty(PropertyName)) return;
            if (_dependencyProperty == null) ResolveDependnecyProperty();

            // we get the dp and update the source
            var _binding = Target.GetBindingExpression(_dependencyProperty);
            if (_binding != null) _binding.UpdateSource();
        }

        #endregion

        #region Handlers

        private static void OnPropertyNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TargetedUpdateBindingExplicitlyAction)d).ResetDependencyPropertyCache();
        }

        #endregion

        #region Helpers

        private void ResetDependencyPropertyCache()
        {
            _dependencyProperty = null;
        }

        private void ResolveDependnecyProperty()
        {
            // note, since we don't have the target type we can't use DependencyPropertyDescriptor in WPF
            // the public static fields 
            string _propName = PropertyName.EndsWith(PROPERTY_POSTFIX, StringComparison.InvariantCultureIgnoreCase)
                                ? PropertyName : PropertyName + PROPERTY_POSTFIX;

            // get the field
            var _field = Target.GetType().GetField(_propName,
                BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.IgnoreCase);
            if (_field == null)
                throw new InvalidOperationException(string.Format(DP_NOTFOUND_ERROR_FORMAT, PropertyName));

            // and get the dp
            _dependencyProperty = (DependencyProperty)_field.GetValue(Target);
            if (_field == null)
                throw new InvalidOperationException(string.Format(DP_NOTFOUND_ERROR_FORMAT, PropertyName));
        }

        #endregion

    }
}
