using Microsoft.Xaml.Behaviors;
using nRoute.Components.TypeConverters;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;

namespace nRoute.Behaviors.Triggers
{
    public class ValueSwitchTrigger
        : TriggerBase<DependencyObject>
    {
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(object), typeof(ValueSwitchTrigger),
            new PropertyMetadata(null, new PropertyChangedCallback(OnSourceChanged)));

        public static readonly DependencyProperty CaseValueProperty =
            DependencyProperty.RegisterAttached("CaseValue", typeof(object), typeof(ValueSwitchTrigger),
            new PropertyMetadata(null));

        public ValueSwitchTrigger()
        {
            ((INotifyCollectionChanged)base.Actions).CollectionChanged +=
                    new NotifyCollectionChangedEventHandler(Actions_CollectionChanged);
        }

        #region Properties

        [Category("Common Properties")]
        [CustomPropertyValueEditor(CustomPropertyValueEditor.PropertyBinding)]
        public object Source
        {
            get { return GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        #endregion

        #region TriggersRelated

        protected override void OnAttached()
        {
            base.OnAttached();
            UpdateCases(this.Source);
        }

        protected override void OnDetaching()
        {
            ((INotifyCollectionChanged)base.Actions).CollectionChanged -=
                    new NotifyCollectionChangedEventHandler(Actions_CollectionChanged);
            base.OnDetaching();
        }

        #endregion

        #region Handlers 

        private void Actions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && base.AssociatedObject != null)
                foreach (var _triggerAction in e.NewItems)
                    ((Microsoft.Xaml.Behaviors.TriggerAction)_triggerAction).IsEnabled = false;

            if (e.OldItems != null)
                foreach (var _triggerAction in e.OldItems)
                    ((Microsoft.Xaml.Behaviors.TriggerAction)_triggerAction).IsEnabled = false;
        }

        #endregion

        #region Handlers

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ValueSwitchTrigger)d).UpdateCases(e.NewValue);
        }

        private void UpdateCases(object sourceValue)
        {
            if (this.AssociatedObject == null) return;      // this is to ensure with initial/static set properties the trigger isn't invoked

            // we can't do switch casing if the source value is null
            if (sourceValue == null)
            {
                // we disable all
                foreach (var _triggerAction in base.Actions)
                {
                    _triggerAction.IsEnabled = false;
                }
                return;
            }

            // we check on each of the actions
            foreach (var _triggerAction in base.Actions)
            {
                // we get the value
                var _value = GetCaseValue(_triggerAction);
                if (_value == null)
                {
                    _triggerAction.IsEnabled = false;
                    continue;
                }

                // we try and match the value also we need to convert it so that the maching can be done
                var _matchValue = ConverterHelper.ConvertToType(_value, sourceValue.GetType());
                _triggerAction.IsEnabled = (object.Equals(_matchValue, sourceValue));
            }

            // and this invokes only the one that maches our value
            base.InvokeActions(sourceValue);
        }

        #endregion

        #region Attached Properties Related

        public static void SetCaseValue(Microsoft.Xaml.Behaviors.TriggerAction triggerAction, object value)
        {
            triggerAction.SetValue(CaseValueProperty, value);
        }

        public static object GetCaseValue(Microsoft.Xaml.Behaviors.TriggerAction triggerAction)
        {
            return triggerAction.GetValue(CaseValueProperty);
        }

        #endregion

    }
}
