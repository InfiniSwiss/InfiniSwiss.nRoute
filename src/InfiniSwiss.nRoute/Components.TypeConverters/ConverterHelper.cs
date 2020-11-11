using System;
using System.ComponentModel;
using System.Globalization;

namespace nRoute.Components.TypeConverters
{
    public static class ConverterHelper
    {
        public static object ConvertToType(object value, Type type)
        {
            if (value != null)
            {
                if (type.IsAssignableFrom(value.GetType()))
                {
                    return value;
                }
                var _typeConverter = GetTypeConverter(type);
                if ((_typeConverter != null) && _typeConverter.CanConvertFrom(value.GetType()))
                {
                    value = _typeConverter.ConvertFrom(null, CultureInfo.InvariantCulture, value);
                    return value;
                }
            }
            return null;
        }

        public static TypeConverter GetTypeConverter(Type type)
        {
            var _attribute = (TypeConverterAttribute)Attribute.GetCustomAttribute(type, typeof(TypeConverterAttribute), false);
            if (_attribute != null)
            {
                try
                {
                    Type _converterType = Type.GetType(_attribute.ConverterTypeName, false);
                    if (_converterType != null)
                    {
                        return (Activator.CreateInstance(_converterType) as TypeConverter);
                    }
                }
                catch
                {
                    // note we absorb this
                }
            }

            return TypeDescriptor.GetConverter(type);
        }
    }
}
