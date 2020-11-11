using System;
using System.Globalization;
using System.Windows.Data;

namespace nRoute.Components
{
    public class ValueConverterRelay
         : IValueConverter
    {

        IValueConverter _value;

        public event EventHandler<ValueEventArgs<IValueConverter>> Initialize;

        public ValueConverterRelay()
         : this(null) { }

        public ValueConverterRelay(IValueConverter value)
        {
            _value = value;
        }

        public virtual IValueConverter Converter
        {
            get { return _value; }
            set { _value = value; }
        }

        #region Static

        public static ValueConverterRelay Create(IValueConverter value)
        {
            return new ValueConverterRelay(value);
        }

        public static ValueConverterRelay Create<TIn, TOut>(Func<TIn, TOut> forwardConverter)
        {
            return new ValueConverterRelay(new ValueConverter<TIn, TOut>(forwardConverter));
        }

        public static ValueConverterRelay Create<TIn, TOut>(Func<TIn, TOut> forwardConverter, Func<TOut, TIn> reverseConverter)
        {
            return new ValueConverterRelay(new ValueConverter<TIn, TOut>(forwardConverter, reverseConverter));
        }

        #endregion

        #region IValueConverter Members

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (this.Converter == null && !InitializeConverter()) return null;
            return this.Converter.Convert(value, targetType, parameter, culture);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (this.Converter == null && !InitializeConverter()) return null;
            return this.Converter.ConvertBack(value, targetType, parameter, culture);
        }

        #endregion

        #region Helper

        private bool InitializeConverter()
        {
            if (this.Converter != null) return true;
            if (Initialize != null)
            {
                var _eventArgs = new ValueEventArgs<IValueConverter>();
                Initialize(this, _eventArgs);
                _value = _eventArgs.Value;
            }
            return (this.Converter != null);
        }

        #endregion

    }
}