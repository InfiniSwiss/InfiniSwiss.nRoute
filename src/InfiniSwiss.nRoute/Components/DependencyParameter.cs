using Microsoft.Xaml.Behaviors;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;

namespace nRoute.Components
{
    [ContentProperty("Value")]
    public partial class DependencyParameter
        : Freezable,
        IEquatable<DependencyParameter>
    {

        #region Declarations

        private const string KEY_SETONCE_ONLY = "BindableParameter's key can only set once";

        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register("Key", typeof(string), typeof(DependencyParameter),
            new PropertyMetadata(null, new PropertyChangedCallback(OnKeyChanged)));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(Object), typeof(DependencyParameter),
            new PropertyMetadata(null));

        #endregion

        public DependencyParameter() { }

        public DependencyParameter(string key, Object value)
        {
            this.Key = key;
            this.Value = value;
        }

        #region Properties

        [Category("Common Properties")]
        public string Key
        {
            get { return Convert.ToString(GetValue(KeyProperty)); }
            set { SetValue(KeyProperty, value); }
        }

        [Category("Common Properties")]
        [CustomPropertyValueEditor(CustomPropertyValueEditor.PropertyBinding)]
        public Object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        #endregion

        #region Handlers

        private static void OnKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Convert.ToString(e.OldValue)))
            {
                throw new ArgumentException(KEY_SETONCE_ONLY, "Key");
            }
        }

        #endregion

#if (!SILVERLIGHT)

        #region Overrides

        protected override Freezable CreateInstanceCore()
        {
            return new DependencyParameter();
        }

        #endregion

#endif

        #region IEquatable<DependencyParameter> Members

        public bool Equals(DependencyParameter other)
        {
            if (other == null) return false;
            return string.Equals(Key, other.Key, StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion

    }
}