using Microsoft.Xaml.Behaviors;
using nRoute.Components.TypeConverters;
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;

namespace nRoute.Behaviors.Triggers
{
    public class ValueCompareTrigger
        : TriggerBase<DependencyObject>
    {
        private const string CANNOT_COMPARE = "Cannot compare, please compare IComparable types or specify an IComparer";

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(object), typeof(ValueCompareTrigger),
            new PropertyMetadata(null, new PropertyChangedCallback(OnSourceChanged)));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(ValueCompareTrigger),
            new PropertyMetadata(null));

        public static readonly DependencyProperty ComparerProperty =
            DependencyProperty.Register("Comparer", typeof(IComparer), typeof(ValueCompareTrigger),
            new PropertyMetadata(null));

        public static readonly DependencyProperty EqualityProperty =
            DependencyProperty.Register("Equality", typeof(EqualityType), typeof(ValueCompareTrigger),
            new PropertyMetadata(EqualityType.Equals));

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
        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        [Category("Common Properties")]
        [CustomPropertyValueEditor(CustomPropertyValueEditor.PropertyBinding)]
        public IComparer Comparer
        {
            get { return (IComparer)GetValue(ComparerProperty); }
            set { SetValue(ComparerProperty, value); }
        }

        [Category("Common Properties")]
        public EqualityType Equality
        {
            get { return (EqualityType)GetValue(EqualityProperty); }
            set { SetValue(EqualityProperty, value); }
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
            ((ValueCompareTrigger)d).CompareValue(e.NewValue);
        }

        private void CompareValue(object sourceValue)
        {
            if (this.AssociatedObject == null) return;      // this is to ensure with initial/static set properties the trigger isn't invoked

            // note, we need to match them per the source type hence the conversion
            bool _result = false;

            // if null, we can only match equality
            if (Value == null)
            {
                if (sourceValue == null)
                {
                    _result = (Equality == EqualityType.Equals || Equality == EqualityType.GreaterThanOrEquals ||
                        Equality == EqualityType.LessThanOrEquals);
                }
            }
            else
            {
                // we compare
                int _compare = 0;
                var _matchValue = ConverterHelper.ConvertToType(Value, sourceValue.GetType());

                if (Comparer != null)
                {
                    _compare = Comparer.Compare(_matchValue, sourceValue);
                }
                else if (_matchValue is IComparable)
                {
                    _compare = (_matchValue as IComparable).CompareTo(sourceValue);
                }
                else
                {
                    throw new InvalidOperationException(CANNOT_COMPARE);
                }

                // per the result we match the equality
                if (_compare == 0)
                {
                    _result = (Equality == EqualityType.Equals ||
                        Equality == EqualityType.GreaterThanOrEquals || Equality == EqualityType.LessThanOrEquals);
                }
                else if (_compare > 0)      // note_ we do opposite, as we check against the source
                {
                    _result = (Equality == EqualityType.LessThan || Equality == EqualityType.LessThanOrEquals);
                }
                else    // _compare < 0 - note we do opposite, as we check against the source
                {
                    _result = (Equality == EqualityType.GreaterThan || Equality == EqualityType.GreaterThanOrEquals);
                }
            }

            // we match with negate and invoke
            if (Negate) _result = !_result;
            if (_result) base.InvokeActions(sourceValue);
        }

        #endregion

    }
}
