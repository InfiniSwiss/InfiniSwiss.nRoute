using nRoute.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace nRoute.Components.Composition
{
    public static class TypeBuilder
    {
        const string BUILD_MUSTNOTBE_STATIC = "Build is only supported on instance/non-static classes, '{0}' is not supported";
        const string BUILD_MUSTBE_PUBLIC = "Build is only supported on public classes, '{0}' is not supported";
        const string BUILD_MUSTNOTBE_ABSTRACT = "Build is not supported on abstract classes, '{0}' is not supported";
        const string BUILD_NONREFECTED_NOTSUPPORTED = "Only reflected building of Objects is supported with Windows Phone Runtime";

        private readonly static Dictionary<Type, Func<Object>> _buildersCache;
        private readonly static Dictionary<Type, Func<Object>> _buildersReflectedCache;
        private readonly static Object _lock = new Object();

        static TypeBuilder()
        {
            _buildersCache = new Dictionary<Type, Func<object>>();
            _buildersReflectedCache = new Dictionary<Type, Func<object>>();
        }

        /// <summary>
        /// Builds an instance of <see cref="T">Type T</see>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of object to create</typeparam>
        /// <returns>An instance of <see cref="T"/></returns>
        public static T BuildType<T>()
        {
            return (T)BuildType(typeof(T), false);
        }

        public static T BuildType<T>(bool useReflection)
        {
            return (T)BuildType(typeof(T), useReflection);
        }

        /// <summary>
        /// Builds an instance of <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of object to create</typeparam>
        /// <returns>An instance of <paramref name="type"/>.</returns>
        public static Object BuildType(Type type)
        {
            return BuildType(type, false);
        }

        public static Object BuildType(Type type, bool useReflection)
        {
            Guard.ArgumentNotNull(type, "type");
            Guard.ArgumentValue(type.IsSealed && type.IsAbstract, "type", BUILD_MUSTNOTBE_STATIC, type.FullName);
            Guard.ArgumentValue(type.IsAbstract, "type", BUILD_MUSTNOTBE_ABSTRACT, type.FullName);

            // we try and get the activator else we need to 
            Func<Object> _builder = null;
            if (!useReflection)
            {
                if (!_buildersCache.TryGetValue(type, out _builder))
                {
                    _builder = CreateDescriptorBuilder(type);
                    lock (_lock)
                    {
                        if (!_buildersCache.ContainsKey(type))
                        {
                            _buildersCache.Add(type, _builder);
                        }
                    }
                }
            }
            else
            {
                if (!_buildersReflectedCache.TryGetValue(type, out _builder))
                {
                    _builder = CreateReflectedDescriptorBuilder(type);
                    lock (_lock)
                    {
                        if (!_buildersReflectedCache.ContainsKey(type))
                        {
                            _buildersReflectedCache.Add(type, _builder);
                        }
                    }
                }
            }

            // and return
            return _builder();
        }

        #region Helpers

        private static Func<Object> CreateDescriptorBuilder(Type type)
        {
            Guard.ArgumentNotNull(type, "type");

            var _typeDescriptor = new TypeDescriptor(type);
            var _typeBuildActions = new List<Action<Object>>();

            // 1. Constructor Parameters
            var _typeConstructorParameters = new List<Func<Object>>();
            foreach (var _parameterDescriptor in _typeDescriptor.DefaultConstructorDescriptor.ParameterDescriptors)
            {
                var _parameterResolve = _parameterDescriptor.Resolver;
                var _parameterType = _parameterDescriptor.Parameter.ParameterType;
                _typeConstructorParameters.Add(() => _parameterResolve.Resolve(_parameterType));
            }

            // 2. Constructor
            var _typeConstructor = (Func<List<Func<Object>>, Object>)null;
            if (_typeDescriptor.DescriptorType.IsPublic && _typeDescriptor.DefaultConstructorDescriptor.Constructor.IsPublic)
            {
                // using dynamic method
                var _constructorDelegate = _typeDescriptor.DefaultConstructorDescriptor.ConstructorDelegate;
                var _constructorParameterDelegate = _typeDescriptor.DefaultConstructorDescriptor.ConstructorParameterDelegate;
                _typeConstructor = new Func<List<Func<Object>>, Object>((p) =>
                {
                    if (p != null && p.Count > 0)
                    {
                        Object[] _parameters = p.Select((f) => f()).ToArray();
                        return _constructorParameterDelegate(_parameters);
                    }
                    else
                    {
                        return _constructorDelegate();
                    }
                });
            }
            else
            {
                // using reflection
                var _constructorInfo = _typeDescriptor.DefaultConstructorDescriptor.Constructor;
                _typeConstructor = new Func<List<Func<Object>>, Object>((p) =>
                {
                    if (p != null && p.Count > 0)
                    {
                        Object[] _parameters = p.Select((f) => f()).ToArray();
                        return _constructorInfo.Invoke(_parameters);
                    }
                    else
                    {
                        return _constructorInfo.Invoke(new object[] { });
                    }
                });
            }

            // 3. Fields
            if (_typeDescriptor.FieldsDescriptor != null)
            {
                foreach (var _fieldDescriptor in _typeDescriptor.FieldsDescriptor)
                {
                    var _fieldInfo = _fieldDescriptor.Field;
                    var _fieldSetter = _fieldDescriptor.FieldSetterDelegate;
                    var _fieldResolver = _fieldDescriptor.Resolver;
                    var _fieldType = _fieldDescriptor.Field.FieldType;
                    if (_fieldInfo.IsPublic)
                    {
                        _typeBuildActions.Add((o) => _fieldSetter(o, _fieldResolver.Resolve(_fieldType)));
                    }
                    else
                    {
                        _typeBuildActions.Add((o) => _fieldInfo.SetValue(o, _fieldResolver.Resolve(_fieldType)));
                    }
                }
            }

            // 4. Properties - note properties don't use emit so they can work as is
            if (_typeDescriptor.PropertiesDescriptor != null)
            {
                foreach (var _propertyDescriptor in _typeDescriptor.PropertiesDescriptor)
                {
                    var _propertySetter = _propertyDescriptor.PropertySetterDelegate;
                    var _propertyResolver = _propertyDescriptor.Resolver;
                    var _propertyType = _propertyDescriptor.Property.PropertyType;
                    _typeBuildActions.Add((o) => _propertySetter(o, _propertyResolver.Resolve(_propertyType)));
                }
            }

            // and return
            return () =>
            {
                // build object with constructor parameters
                var _object = _typeConstructor(_typeConstructorParameters);

                // build fields and properties
                _typeBuildActions.ForEach((a) => a(_object));

                // and return
                return _object;
            };
        }

        private static Func<Object> CreateReflectedDescriptorBuilder(Type type)
        {
            Guard.ArgumentNotNull(type, "type");

            var _typeDescriptor = new TypeDescriptor(type);
            var _typeBuildActions = new List<Action<Object>>();

            // 1. Constructor Parameters
            var _typeConstructorParameters = new List<Func<Object>>();
            foreach (var _parameterDescriptor in _typeDescriptor.DefaultConstructorDescriptor.ParameterDescriptors)
            {
                var _parameterResolver = _parameterDescriptor.Resolver;
                var _parameterType = _parameterDescriptor.Parameter.ParameterType;
                _typeConstructorParameters.Add(() => _parameterResolver.Resolve(_parameterType));
            }

            // 2. Constructor
            var _constructorInfo = _typeDescriptor.DefaultConstructorDescriptor.Constructor;
            var _typeConstructor = new Func<List<Func<Object>>, Object>((p) =>
            {
                if (p != null && p.Count > 0)
                {
                    Object[] _parameters = p.Select((f) => f()).ToArray();
                    return _constructorInfo.Invoke(_parameters);
                }
                else
                {
                    return _constructorInfo.Invoke(new object[] { });
                }
            });

            // 3. Fields
            if (_typeDescriptor.FieldsDescriptor != null)
            {
                foreach (var _fieldDescriptor in _typeDescriptor.FieldsDescriptor)
                {
                    var _fieldInfo = _fieldDescriptor.Field;
                    var _fieldResolver = _fieldDescriptor.Resolver;
                    var _fieldType = _fieldDescriptor.Field.FieldType;
                    _typeBuildActions.Add((o) => _fieldInfo.SetValue(o, _fieldResolver.Resolve(_fieldType)));
                }
            }

            // 4. Properties
            if (_typeDescriptor.PropertiesDescriptor != null)
            {
                foreach (var _propertyDescriptor in _typeDescriptor.PropertiesDescriptor)
                {
                    var _propertyInfo = _propertyDescriptor.Property;
                    var _propertyResolver = _propertyDescriptor.Resolver;
                    var _propertyType = _propertyDescriptor.Property.PropertyType;
                    _typeBuildActions.Add((o) => _propertyInfo.SetValue(o, _propertyResolver.Resolve(_propertyType), null));
                }
            }

            // and return
            return () =>
            {
                // build object with constructor parameters
                var _object = _typeConstructor(_typeConstructorParameters);

                // build fields and properties
                _typeBuildActions.ForEach((a) => a(_object));

                // and return
                return _object;
            };
        }

        #endregion

    }
}



