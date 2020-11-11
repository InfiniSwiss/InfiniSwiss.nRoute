using nRoute.Internal;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace nRoute.Components
{
    /// <summary>
    /// A type-instance activator, that initializes types using only public and parameter-less constructors.
    /// </summary>
    /// <remarks>
    /// - This is much more faster than the system CreateType, as it uses dynamic methods.
    /// - Secondly, it also caches the activator so next set of instances don't need to create the instances.
    /// - Adapter from http://www.lostechies.com/blogs/jimmy_bogard/archive/2009/08/05/late-bound-invocations-with-dynamicmethod.aspx
    ///   including comments and from http://beaucrawford.net/post/Constructor-invocation-with-DynamicMethod.aspx and
    ///   http://stackoverflow.com/questions/2353174/c-emitting-dynamic-method-delegate-to-load-parametrized-constructor-problem  
    /// </remarks>
    public static class TypeActivator
    {
        private const string DELEGATE_NAME_FORMAT = "__DelegateInvoker{0}";
        private const string MUSTBE_PARAMETERLESS_CONSTRUCTOR = "Constructor for which the delegate must be parameter less";
        private const string MUSTBE_PARAMETERIZED_CONSTRUCTOR = "Constructor for which the delegate must have one or more parameters";
        private const string MUSTBE_PARAMETERLESS_PROPERTY = "Property for which the delegate must be parameter less";

        private readonly static Type[] OBJECT_ARRAYTYPE_TYPE = new Type[] { typeof(object[]) };
        private readonly static Type[] EMPTY_PARAMETER_TYPE = new Type[] { };

        private readonly static Dictionary<Type, ConstructorDelegate> _activators;
        private readonly static Object _lock = new Object();

        public delegate Object ConstructorDelegate();
        public delegate Object ConstructorParamsDelegate(params object[] parameters);
        public delegate void SetterDelegate(object target, object value);

        static TypeActivator()
        {
            _activators = new Dictionary<Type, ConstructorDelegate>();
        }

        /// <summary>
        /// Creates an instance of <see cref="T">Type T</see>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of object to create</typeparam>
        /// <returns>An instance of <see cref="T"/></returns>
        public static T CreateInstance<T>()
        {
            return (T)CreateInstance(typeof(T));
        }

        /// <summary>
        /// Creates an instance of <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of object to create</typeparam>
        /// <returns>An instance of <paramref name="type"/>.</returns>
        public static object CreateInstance(Type type)
        {
            Guard.ArgumentNotNull(type, "type");

            ConstructorDelegate _activator = null;
            if (!_activators.TryGetValue(type, out _activator))
            {

                var _constructor = type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, EMPTY_PARAMETER_TYPE, null);
                if (_constructor == null)
                    _constructor = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, EMPTY_PARAMETER_TYPE, null);
                _activator = CreateConstructorDelegate(_constructor);
                lock (_lock)
                {
                    // double check and add
                    if (!_activators.ContainsKey(type))
                        _activators.Add(type, _activator);
                }
            }

            // and return
            return _activator();
        }


        #region Delegate Builders

        public static ConstructorDelegate CreateConstructorDelegate(ConstructorInfo constructor)
        {
            Guard.ArgumentNotNull(constructor, "constructor");
            Guard.ArgumentValue(constructor.GetParameters().Length != 0, MUSTBE_PARAMETERLESS_CONSTRUCTOR);

            var _sourceType = constructor.DeclaringType;
            var _method = new DynamicMethod(string.Format(DELEGATE_NAME_FORMAT, _sourceType.Name), _sourceType, EMPTY_PARAMETER_TYPE);
            var _gen = _method.GetILGenerator();

            _gen.Emit(OpCodes.Newobj, constructor);
            _gen.Emit(OpCodes.Ret);

            return (ConstructorDelegate)_method.CreateDelegate(typeof(ConstructorDelegate));
        }

        public static ConstructorParamsDelegate CreateConstructorParametersDelegate(ConstructorInfo constructor)
        {
            Guard.ArgumentNotNull(constructor, "constructor");
            Guard.ArgumentValue(constructor.GetParameters().Length == 0, MUSTBE_PARAMETERIZED_CONSTRUCTOR);

            var _parameters = constructor.GetParameters();
            var _sourceType = constructor.DeclaringType;
            var _method = new DynamicMethod(constructor.Name, _sourceType, OBJECT_ARRAYTYPE_TYPE);
            var _gen = _method.GetILGenerator();

            for (int i = 0; i < _parameters.Length; i++)
            {
                _gen.Emit(OpCodes.Ldarg_0);
                switch (i)
                {
                    case 0:
                        _gen.Emit(OpCodes.Ldc_I4_0);
                        break;
                    case 1:
                        _gen.Emit(OpCodes.Ldc_I4_1);
                        break;
                    case 2:
                        _gen.Emit(OpCodes.Ldc_I4_2);
                        break;
                    case 3:
                        _gen.Emit(OpCodes.Ldc_I4_3);
                        break;
                    case 4:
                        _gen.Emit(OpCodes.Ldc_I4_4);
                        break;
                    case 5:
                        _gen.Emit(OpCodes.Ldc_I4_5);
                        break;
                    case 6:
                        _gen.Emit(OpCodes.Ldc_I4_6);
                        break;
                    case 7:
                        _gen.Emit(OpCodes.Ldc_I4_7);
                        break;
                    case 8:
                        _gen.Emit(OpCodes.Ldc_I4_8);
                        break;
                    default:
                        if (i > -129 && i < 128)
                        {
                            _gen.Emit(OpCodes.Ldc_I4_S, (SByte)i);
                        }
                        else
                        {
                            _gen.Emit(OpCodes.Ldc_I4, i);
                        }
                        break;
                }

                var _parameterType = _parameters[i].ParameterType;
                _gen.Emit(OpCodes.Ldelem_Ref);
                _gen.Emit(_parameterType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, _parameterType);
            }
            _gen.Emit(OpCodes.Newobj, constructor);
            _gen.Emit(OpCodes.Ret); ;

            return (ConstructorParamsDelegate)_method.CreateDelegate(typeof(ConstructorParamsDelegate));
        }

        public static SetterDelegate CreateFieldDelegate(FieldInfo field)
        {
            Guard.ArgumentNotNull(field, "field");

            var _sourceType = field.DeclaringType;
            var _fieldType = field.FieldType;
            var _method = new DynamicMethod("Set" + field.Name, null, new[] { typeof(object), typeof(object) });
            var _gen = _method.GetILGenerator();

            _gen.Emit(OpCodes.Ldarg_0);
            _gen.Emit(OpCodes.Castclass, _sourceType);
            _gen.Emit(OpCodes.Ldarg_1);
            //_gen.Emit(OpCodes.Unbox_Any, field.FieldType);
            _gen.Emit(_fieldType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, _fieldType);
            _gen.Emit(OpCodes.Stfld, field);
            _gen.Emit(OpCodes.Ret);

            return (SetterDelegate)_method.CreateDelegate(typeof(SetterDelegate));
        }

        public static SetterDelegate CreatePropertyDelegate(PropertyInfo property)
        {
            Guard.ArgumentNotNull(property, "property");
            Guard.ArgumentValue(property.GetIndexParameters().Length != 0, MUSTBE_PARAMETERLESS_PROPERTY);

            var _setterType = typeof(Action<,>).MakeGenericType(property.DeclaringType, property.PropertyType);
            var _propertyWriter = typeof(PropertyWriter<,>).MakeGenericType(property.DeclaringType, property.PropertyType);
            var _setterDelegate = Delegate.CreateDelegate(_setterType, property.GetSetMethod(true));
            var _writer = (IPropertyWriter)Activator.CreateInstance(_propertyWriter, _setterDelegate);

            return _writer.SetValue;
        }

        //public static SetterDelegate CreatePropertyDelegate(PropertyInfo property)
        //{
        //    Guard.ArgumentNotNull(property, "property");

        //    var _method = new DynamicMethod("Set" + property.Key, null, new[] { typeof(object), typeof(object) });
        //    var _gen = _method.GetILGenerator();
        //    var _sourceType = property.DeclaringType;
        //    var _propertyType = property.PropertyType;
        //    var _setter = property.GetSetMethod(true);

        //    _gen.Emit(OpCodes.Ldarg_0);
        //    _gen.Emit(OpCodes.Castclass, _sourceType);
        //    _gen.Emit(OpCodes.Ldarg_1);
        //    //_gen.Emit(OpCodes.Unbox_Any, property.PropertyType);
        //    _gen.Emit(_propertyType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, _propertyType);
        //    _gen.Emit(OpCodes.Callvirt, _setter);
        //    _gen.Emit(OpCodes.Ret);

        //    return (SetterDelegate)_method.CreateDelegate(typeof(SetterDelegate));
        //}

        #endregion

        #region Nested

        private interface IPropertyWriter
        {
            void SetValue(object instance, object value);
        }

        private class PropertyWriter<TInstance, TProperty>
            : IPropertyWriter
        {
            private readonly Action<TInstance, TProperty> _setValueDelegate;

            public PropertyWriter(Action<TInstance, TProperty> setValueDelegate)
            {
                _setValueDelegate = setValueDelegate;
            }

            public void SetValue(object instance, object value)
            {
                _setValueDelegate((TInstance)instance, (TProperty)value);
            }
        }

        #endregion

        //#endif

    }
}