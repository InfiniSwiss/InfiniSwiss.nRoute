using Microsoft.Xaml.Behaviors;
using System.ComponentModel;
using System.Windows;

namespace nRoute.Behaviors.Triggers
{
    public class ValueChangedTrigger
        : TriggerBase<DependencyObject>
    {
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(object), typeof(ValueChangedTrigger),
            new PropertyMetadata(null, new PropertyChangedCallback(OnSourceChanged)));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(ValueChangedTrigger),
            new PropertyMetadata(null));

        #region Properties

        [Category("Common Properties")]
        [CustomPropertyValueEditor(CustomPropertyValueEditor.PropertyBinding)]
        public object Source
        {
            get { return GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        #endregion

        #region Handlers

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ValueChangedTrigger)d).OnChange(e.NewValue);
        }

        private void OnChange(object sourceValue)
        {
            if (this.AssociatedObject == null) return;      // this is to ensure with initial/static set properties the trigger isn't invoked
            base.InvokeActions(sourceValue);
        }

        #endregion

    }
}
