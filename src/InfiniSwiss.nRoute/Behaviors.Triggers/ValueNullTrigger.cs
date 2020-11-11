using Microsoft.Xaml.Behaviors;
using System.ComponentModel;
using System.Windows;

namespace nRoute.Behaviors.Triggers
{
    public class ValueNullTrigger
        : TriggerBase<DependencyObject>
    {
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(object), typeof(ValueNullTrigger),
            new PropertyMetadata(null, new PropertyChangedCallback(OnSourceChanged)));

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
        public bool Negate
        {
            get { return _negate; }
            set { _negate = value; }
        }

        #endregion

        #region Handlers

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ValueNullTrigger)d).CompareValue(e.NewValue);
        }

        private void CompareValue(object value)
        {
            if (this.AssociatedObject == null) return;      // this is to ensure with initial/static set properties the trigger isn't invoked

            var _result = (value == null);
            if (Negate) _result = !_result;
            if (_result) base.InvokeActions(value);
        }

        #endregion

    }
}
