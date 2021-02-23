using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace nRoute.Components
{
    /// <summary>
    /// A generic converter using lambdas.
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <remarks>
    /// - based on ideas from http://khason.net/dev/nifty-time-savers-for-wpf-development/
    /// </remarks>
    public class ValueConverter<TIn, TOut>
         : IValueConverter
    {
        public ValueConverter() { }

        public ValueConverter(Func<TIn, TOut> forwardConversion)
        {
            if (forwardConversion != null) ForwardConversion = (v, p) => forwardConversion(v);
        }

        public ValueConverter(Func<TIn, TOut> forwardConversion, Func<TOut, TIn> reverseConversion)
        {
            if (forwardConversion != null) ForwardConversion = (v, p) => forwardConversion(v);
            if (reverseConversion != null) ReverseConversion = (v, p) => reverseConversion(v);
        }

        public ValueConverter(Func<TIn, Object, TOut> forwardConversion)
        {
            ForwardConversion = forwardConversion;
        }

        public ValueConverter(Func<TIn, Object, TOut> forwardConversion, Func<TOut, Object, TIn> reverseConversion)
        {
            ForwardConversion = forwardConversion;
            ReverseConversion = reverseConversion;
        }

        #region Properties

        public Func<TIn, Object, TOut> ForwardConversion { get; set; }

        public Func<TOut, Object, TIn> ReverseConversion { get; set; }

        #endregion

        #region IValueConverter Members

        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (ForwardConversion == null)
            {
                throw new NotSupportedException("Forward conversion is not defined.");
            }

            try
            {
                var in1 = Object.ReferenceEquals(value, DependencyProperty.UnsetValue) ? default(TIn) : (TIn)value;
                return ForwardConversion(in1, parameter);
            }
            catch (Exception ex)
            {
                Debugger.Break();
                return null;
            }
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (ReverseConversion == null)
            {
                throw new NotSupportedException("Reverse conversion is not defined.");
            }

            try
            {
                var out1 = Object.ReferenceEquals(value, DependencyProperty.UnsetValue) ? default(TOut) : (TOut)value;
                return ReverseConversion(out1, parameter);
            }
            catch // (Exception ex)
            {
                Debugger.Break();
                return null;
            }
        }

        #endregion

    }
}