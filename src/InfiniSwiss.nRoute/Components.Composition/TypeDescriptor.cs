using nRoute.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace nRoute.Components.Composition
{
    internal sealed class TypeDescriptor
    {

        #region Declarations

        private const string RESOLVECONST_MUSTBE_PUBLIC = "ResolveConstructor Attribute is only supported on public constructors for type '{0}'";
        private const string RESOLVECONST_MUSTBE_INSTANCE = "ResolveConstructor Attribute is not supported on static constructors for type '{0}'";
        private const string RESOLVECONST_ONLYONCE = "ResolveConstructor Attribute can only be applied on one constructor for type '{0}'";
        private const string RESOLVECONST_CANNOTBE_ABSTRACT = "ResolveConstructor Attribute cannot be used on an abstract class type '{0}'";
        private const string BUILD_MUSTHAVE_DEFAULTCONST =
            "Cannot resolve a default constructor to use for {0}, please specify using ResolveConstructor Attribute";
        private const string RESOLVEFIELD_MUSTBE_PUBLIC =
            "ResolveConstructor Attribute is only supported on public fields, field '{0}' on type '{1}' is not supported";
        private const string RESOLVEFIELD_MUSTBE_INSTANCE =
            "ResolveConstructor Attribute is not supported on static fields, field '{0}' on type '{1}' is not supported";
        private const string RESOLVEFIELD_MUSTNOTBE_CONSTORREADONLY =
            "ResolveConstructor Attribute is not supported on read-only or constant fields, field '{0}' on type '{1}' is not supported";
        private const string RESOLVEPROP_MUSTBE_PUBLIC =
            "ResolveConstructor Attribute is only supported on public properties, property '{0}' on type '{1}' is not supported";
        private const string RESOLVEPROP_MUSTBE_INSTANCE =
            "ResolveConstructor Attribute is not supported on static properties, property '{0}' on type '{1}' is not supported";
        private const string RESOLVEPROP_MUSTBE_WRITABLE =
            "ResolveConstructor Attribute is only supported on properties with public setter, property '{0}' on type '{1}' is not supported";
        private const string RESOLVEPROP_MUSTNOTBE_PARAMETERIZED =
            "ResolveConstructor Attribute is not supported on properties with parameters, property '{0}' on type '{1}' is not supported";

        private readonly static Type[] EMPTY_PARAMETERS = new Type[] { };

        private readonly Type _profileType;
        private readonly ConstructorDescriptor _constructorDescriptor;
        private readonly IEnumerable<FieldDescriptor> _fieldDescriptors;
        private readonly IEnumerable<PropertyDescriptor> _propertyDescriptors;

        #endregion

        public TypeDescriptor(Type type)
        {
            Guard.ArgumentNotNull(type, "type");

            _profileType = type;
            _constructorDescriptor = ActivatorConstructorDescriptor();
            _fieldDescriptors = ResolveFieldsDescriptor();
            _propertyDescriptors = ResolvePropertiesDescriptor();
        }

        #region Properties

        public Type DescriptorType
        {
            get { return _profileType; }
        }

        public ConstructorDescriptor DefaultConstructorDescriptor
        {
            get { return _constructorDescriptor; }
        }

        public IEnumerable<FieldDescriptor> FieldsDescriptor
        {
            get { return _fieldDescriptors; }
        }

        public IEnumerable<PropertyDescriptor> PropertiesDescriptor
        {
            get { return _propertyDescriptors; }
        }

        #endregion

        #region Helpers

        private ConstructorDescriptor ActivatorConstructorDescriptor()
        {
            var _constructorInfos = DescriptorType.GetConstructors();
            var _resolveConstructor = (ConstructorInfo)null;

            foreach (var _constructorInfo in _constructorInfos)
            {
                if (Attribute.IsDefined(_constructorInfo, typeof(ResolveConstructorAttribute)))
                {
                    if (_constructorInfo.IsStatic)
                        throw new ResolveResourceException(string.Format(RESOLVECONST_MUSTBE_INSTANCE, DescriptorType.FullName), DescriptorType);
                    if (_resolveConstructor != null)
                        throw new ResolveResourceException(string.Format(RESOLVECONST_ONLYONCE, DescriptorType.FullName), DescriptorType);
                    if (DescriptorType.IsAbstract)
                        throw new ResolveResourceException(string.Format(RESOLVECONST_CANNOTBE_ABSTRACT, DescriptorType.FullName), DescriptorType);

                    _resolveConstructor = _constructorInfo;
                }
            }

            // else we try to get a public and parameter-less constructor or if there's a single constructor, we'll take that
            if (_resolveConstructor == null)
            {
                _resolveConstructor = DescriptorType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, EMPTY_PARAMETERS, null);
                if (_resolveConstructor == null)
                    _resolveConstructor = DescriptorType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, EMPTY_PARAMETERS, null);
                if (_resolveConstructor == null && _constructorInfos.Length == 1)
                {
                    _resolveConstructor = _constructorInfos[0];
                }
                if (_resolveConstructor == null)
                    throw new ResolveResourceException(string.Format(BUILD_MUSTHAVE_DEFAULTCONST, DescriptorType.FullName), DescriptorType);
            }

            var _constructor = new ConstructorDescriptor();
            _constructor.Constructor = _resolveConstructor;

            var _parameters = _resolveConstructor.GetParameters();
            if (_parameters.Length == 0)
            {
                _constructor.ConstructorDelegate = TypeActivator.CreateConstructorDelegate(_resolveConstructor);
                _constructor.ParameterDescriptors = Enumerable.Empty<ParameterDescriptor>();
            }
            else
            {
                _constructor.ConstructorParameterDelegate = TypeActivator.CreateConstructorParametersDelegate(_resolveConstructor);
                _constructor.ParameterDescriptors = ResolveConstructorParametersDescriptor(_parameters);
            }
            return _constructor;
        }

        private IEnumerable<ParameterDescriptor> ResolveConstructorParametersDescriptor(IEnumerable<ParameterInfo> parameterInfos)
        {
            Guard.ArgumentNotNull(parameterInfos, "parameterInfos");

            var _parameters = new List<ParameterDescriptor>();
            foreach (var _parameterInfo in parameterInfos)
            {
                if (Attribute.IsDefined(_parameterInfo, typeof(ResolveResourceBaseAttribute), false))
                {
                    var _attribute = Attribute.GetCustomAttribute(_parameterInfo, typeof(ResolveResourceBaseAttribute));
                    _parameters.Add(new ParameterDescriptor()
                    {
                        Parameter = _parameterInfo,
                        Resolver = (IResourceResolver)_attribute
                    });
                }
                else
                {
                    _parameters.Add(new ParameterDescriptor()
                    {
                        Parameter = _parameterInfo,
                        Resolver = TypeResourceResolver.Instance
                    });
                }
            }

            return _parameters;
        }

        private IEnumerable<FieldDescriptor> ResolveFieldsDescriptor()
        {
            var _fields = new List<FieldDescriptor>();
            var _fieldsInfo = _profileType.GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Instance |
               BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var _fieldInfo in _fieldsInfo)
            {
                if (Attribute.IsDefined(_fieldInfo, typeof(ResolveResourceBaseAttribute), false))
                {
                    if (!_fieldInfo.IsPublic)
                        throw new ResolveResourceException(
                            string.Format(RESOLVEFIELD_MUSTBE_PUBLIC, _fieldInfo.Name, DescriptorType), DescriptorType);
                    if (_fieldInfo.IsStatic)
                        throw new ResolveResourceException(
                            string.Format(RESOLVEFIELD_MUSTBE_INSTANCE, _fieldInfo.Name, DescriptorType), DescriptorType);
                    if (_fieldInfo.IsLiteral || _fieldInfo.IsInitOnly)
                        throw new ResolveResourceException(
                            string.Format(RESOLVEFIELD_MUSTNOTBE_CONSTORREADONLY, _fieldInfo.Name, DescriptorType), DescriptorType);

                    var _attribute = Attribute.GetCustomAttribute(_fieldInfo, typeof(ResolveResourceBaseAttribute));
                    _fields.Add(new FieldDescriptor()
                    {
                        Field = _fieldInfo,
                        Resolver = (IResourceResolver)_attribute
                        ,
                        FieldSetterDelegate = TypeActivator.CreateFieldDelegate(_fieldInfo)
                    });
                }
            }

            return _fields;
        }

        private IEnumerable<PropertyDescriptor> ResolvePropertiesDescriptor()
        {
            var _properties = new List<PropertyDescriptor>();
            var _propertiesInfos = _profileType.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Instance |
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var _propertyInfo in _propertiesInfos)
            {
                if (Attribute.IsDefined(_propertyInfo, typeof(ResolveResourceBaseAttribute), false))
                {
                    var _propertySetter = _propertyInfo.GetSetMethod(true);
                    if (!_propertyInfo.CanWrite)
                        throw new ResolveResourceException(
                            string.Format(RESOLVEPROP_MUSTBE_WRITABLE, _propertyInfo.Name, DescriptorType), DescriptorType);
                    if (!_propertySetter.IsPublic)
                        throw new ResolveResourceException(
                            string.Format(RESOLVEPROP_MUSTBE_PUBLIC, _propertyInfo.Name, DescriptorType), DescriptorType);
                    if (_propertySetter.IsStatic)
                        throw new ResolveResourceException(
                            string.Format(RESOLVEPROP_MUSTBE_INSTANCE, _propertyInfo.Name, DescriptorType), DescriptorType);
                    if (_propertyInfo.GetIndexParameters().Length > 0)
                        throw new ResolveResourceException(
                            string.Format(RESOLVEPROP_MUSTNOTBE_PARAMETERIZED, _propertyInfo.Name, DescriptorType), DescriptorType);

                    var _attribute = Attribute.GetCustomAttribute(_propertyInfo, typeof(ResolveResourceBaseAttribute));
                    _properties.Add(new PropertyDescriptor()
                    {
                        Property = _propertyInfo,
                        Resolver = (IResourceResolver)_attribute
                        ,
                        PropertySetterDelegate = TypeActivator.CreatePropertyDelegate(_propertyInfo)
                    });
                }
            }

            return _properties;
        }

        #endregion

        #region Nested Classes

        public sealed class ConstructorDescriptor
        {
            public ConstructorInfo Constructor { get; set; }

            public IEnumerable<ParameterDescriptor> ParameterDescriptors { get; set; }

            public TypeActivator.ConstructorDelegate ConstructorDelegate { get; set; }

            public TypeActivator.ConstructorParamsDelegate ConstructorParameterDelegate { get; set; }
        }

        public sealed class ParameterDescriptor
        {
            public ParameterInfo Parameter { get; set; }

            public IResourceResolver Resolver { get; set; }
        }

        public sealed class FieldDescriptor
        {
            public FieldInfo Field { get; set; }

            public IResourceResolver Resolver { get; set; }

            public TypeActivator.SetterDelegate FieldSetterDelegate { get; set; }
        }

        public sealed class PropertyDescriptor
        {
            public PropertyInfo Property { get; set; }

            public IResourceResolver Resolver { get; set; }

            public TypeActivator.SetterDelegate PropertySetterDelegate { get; set; }
        }

        public sealed class TypeResourceResolver
            : IResourceResolver
        {
            private static readonly TypeResourceResolver _instance = new TypeResourceResolver();

            private TypeResourceResolver() { }

            public static IResourceResolver Instance
            {
                get { return _instance; }
            }

            #region IResolveTarget Members

            public object Resolve(Type targetType)
            {
                Guard.ArgumentNotNull(targetType, "targetType");
                return Resource.GetResource(targetType);
            }

            #endregion

        }

        #endregion

    }
}