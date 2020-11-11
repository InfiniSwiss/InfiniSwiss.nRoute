using nRoute.Components.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace nRoute.Components.Composition.Adapters
{
    internal static class KnownAdapters
    {

        #region Declarations

        private readonly static Type LAZY_GENERIC_TYPE = typeof(Lazy<>);
        private readonly static Type FUTURE_GENERIC_TYPE = typeof(Future<>);
        private readonly static Type LAZYMETA_GENERIC_TYPE = typeof(Lazy<,>);
        private readonly static Type FUNC_GENERIC_TYPE = typeof(Func<>);
        private readonly static Type IENUM_GENERIC_TYPE = typeof(IEnumerable<>);
        private readonly static Type ICOLLECTION_GENERIC_TYPE = typeof(ICollection<>);
        private readonly static Type LIST_GENERIC_TYPE = typeof(List<>);
        private readonly static Type IQUERYABLE_GENERIC_TYPE = typeof(IQueryable<>);
        private readonly static Type CHANNEL_GENERIC_TYPE = typeof(IChannel<>);
        private readonly static Type READONLYOBSCOL_GENERIC_TYPE = typeof(ReadOnlyObservableCollection<>);

        private readonly static Type LAZY_ADAPTER_TYPE = typeof(LazyLocatorAdapter<>);
        private readonly static Type FUTURE_ADAPTER_TYPE = typeof(FutureLocatorAdapter<>);
        private readonly static Type LAZYMETA_ADAPTER_TYPE = typeof(LazyMetadataLocatorAdapter<,>);
        private readonly static Type FUNC_ADAPTER_TYPE = typeof(FuncLocatorAdapter<>);
        private readonly static Type ARRAY_ADAPTER_TYPE = typeof(ArrayLocatorAdapter<>);
        private readonly static Type ENUM_ADAPTER_TYPE = typeof(EnumerableLocatorAdapter<>);
        private readonly static Type COLLECTION_ADAPTER_TYPE = typeof(CollectionLocatorAdapter<>);
        private readonly static Type LIST_ADAPTER_TYPE = typeof(ListLocatorAdapter<>);
        private readonly static Type QUERYABLE_ADAPTER_TYPE = typeof(QueryableLocatorAdapter<>);
        private readonly static Type ENUMLAZY_ADAPTER_TYPE = typeof(EnumerableLazyLocatorAdapter<>);
        private readonly static Type ENUMLAZYMETA_ADAPTER_TYPE = typeof(EnumerableLazyMetaLocatorAdapter<,>);
        private readonly static Type ENUMFUNC_ADAPTER_TYPE = typeof(EnumerableFuncLocatorAdapter<>);
        private readonly static Type READONLYOBSV_ADAPTER_TYPE = typeof(ReadOnlyObservableCollectionLocatorAdapter<>);
        private readonly static Type READONLYOBSVLAZY_ADAPTER_TYPE = typeof(ReadOnlyObservableCollectionLazyLocatorAdapter<>);
        private readonly static Type READONLYOBSVLAZYMETA_ADAPTER_TYPE = typeof(ReadOnlyObservableCollectionLazyMetadataLocatorAdapter<,>);
        private readonly static Type CHANNEL_ADAPTER_TYPE = typeof(ChannelLocatorAdapter<>);

        #endregion

        public static void RegisterAdapters()
        {
            // Lazy<T>
            Resource.RegisterLocatorAdapter(
            (t) => t.IsGenericType &&
                    t.GetGenericTypeDefinition() == LAZY_GENERIC_TYPE &&
                    Resource.IsResourceRegistered(t.GetGenericArguments()[0], true),
            (t) =>
            {
                var _resourceType = t.GetGenericArguments()[0];
                return (ILocatorAdapter)TypeActivator.CreateInstance(LAZY_ADAPTER_TYPE.MakeGenericType(_resourceType));
            });

            // Future<T>
            Resource.RegisterLocatorAdapter(
            (t) => t.IsGenericType &&
                    t.GetGenericTypeDefinition() == FUTURE_GENERIC_TYPE,
            (t) =>
            {
                var _resourceType = t.GetGenericArguments()[0];
                return (ILocatorAdapter)TypeActivator.CreateInstance(FUTURE_ADAPTER_TYPE.MakeGenericType(_resourceType));
            });

            // Lazy<T,TMetadata>
            Resource.RegisterLocatorAdapter(
            (t) => t.IsGenericType &&
                    t.GetGenericTypeDefinition() == LAZYMETA_GENERIC_TYPE &&
                    Resource.IsResourceRegistered(t.GetGenericArguments()[0], true),
            (t) =>
            {
                var _resourceType = t.GetGenericArguments()[0];
                var _metadataType = t.GetGenericArguments()[1];
                return (ILocatorAdapter)TypeActivator.CreateInstance(LAZYMETA_ADAPTER_TYPE.MakeGenericType(_resourceType, _metadataType));
            });

            // Func<T>
            Resource.RegisterLocatorAdapter(
            (t) => t.IsGenericType &&
                    t.GetGenericTypeDefinition() == FUNC_GENERIC_TYPE &&
                    Resource.IsResourceRegistered(t.GetGenericArguments()[0], true),
            (t) =>
            {
                var _resourceType = t.GetGenericArguments()[0];
                return (ILocatorAdapter)TypeActivator.CreateInstance(FUNC_ADAPTER_TYPE.MakeGenericType(_resourceType));
            });

            // T[]
            Resource.RegisterLocatorAdapter(
            (t) => t.IsArray &&
                    Resource.IsResourceRegistered(t.GetElementType(), false),
            (t) =>
            {
                var _resourceType = t.GetElementType();
                return (ILocatorAdapter)TypeActivator.CreateInstance(ARRAY_ADAPTER_TYPE.MakeGenericType(_resourceType));
            });

            // IEnumerable<T>
            Resource.RegisterLocatorAdapter(
            (t) => t.IsGenericType &&
                    t.GetGenericTypeDefinition() == IENUM_GENERIC_TYPE &&
                    Resource.IsResourceRegistered(t.GetGenericArguments()[0], false),
            (t) =>
            {
                var _resourceType = t.GetGenericArguments()[0];
                return (ILocatorAdapter)TypeActivator.CreateInstance(ENUM_ADAPTER_TYPE.MakeGenericType(_resourceType));
            });

            // ICollection<T>
            Resource.RegisterLocatorAdapter(
            (t) => t.IsGenericType &&
                    t.GetGenericTypeDefinition() == ICOLLECTION_GENERIC_TYPE &&
                    Resource.IsResourceRegistered(t.GetGenericArguments()[0], false),
            (t) =>
            {
                var _resourceType = t.GetGenericArguments()[0];
                return (ILocatorAdapter)TypeActivator.CreateInstance(COLLECTION_ADAPTER_TYPE.MakeGenericType(_resourceType));
            });

            // List<T>
            Resource.RegisterLocatorAdapter(
            (t) => t.IsGenericType &&
                    t.GetGenericTypeDefinition() == LIST_GENERIC_TYPE &&
                    Resource.IsResourceRegistered(t.GetGenericArguments()[0], false),
            (t) =>
            {
                var _resourceType = t.GetGenericArguments()[0];
                return (ILocatorAdapter)TypeActivator.CreateInstance(LIST_ADAPTER_TYPE.MakeGenericType(_resourceType));
            });

            // IQueryable<T>
            Resource.RegisterLocatorAdapter(
            (t) => t.IsGenericType &&
                    t.GetGenericTypeDefinition() == IQUERYABLE_GENERIC_TYPE &&
                    Resource.IsResourceRegistered(t.GetGenericArguments()[0], false),
            (t) =>
            {
                var _resourceType = t.GetGenericArguments()[0];
                return (ILocatorAdapter)TypeActivator.CreateInstance(QUERYABLE_ADAPTER_TYPE.MakeGenericType(_resourceType));
            });
            // IEnumerable<Lazy<T>>
            Resource.RegisterLocatorAdapter(
            (t) =>
            {
                // we check if it is IEnumerable<> first
                if (!t.IsGenericType) return false;
                var _genericArgumentType = t.GetGenericTypeDefinition();
                if (_genericArgumentType != IENUM_GENERIC_TYPE) return false;

                // we get the parameter of the generic type and check if it is Lazy<T>
                var _genericParameterType = t.GetGenericArguments()[0];
                return _genericParameterType.IsGenericType &&
                      _genericParameterType.GetGenericTypeDefinition() == LAZY_GENERIC_TYPE &&
                      Resource.IsResourceRegistered(_genericParameterType.GetGenericArguments()[0], false);
            },
            (t) =>
            {
                var _argumentType = t.GetGenericArguments()[0];
                var _resourceType = _argumentType.GetGenericArguments()[0];
                return (ILocatorAdapter)TypeActivator.CreateInstance(ENUMLAZY_ADAPTER_TYPE.MakeGenericType(_resourceType));
            });

            // IEnumerable<Func<T>>
            Resource.RegisterLocatorAdapter(
            (t) =>
            {
                // we check if it is IEnumerable<> first
                if (!t.IsGenericType) return false;
                var _genericArgumentType = t.GetGenericTypeDefinition();
                if (_genericArgumentType != IENUM_GENERIC_TYPE) return false;

                // we get the parameter of the generic type and check if it is Lazy<T>
                var _genericParameterType = t.GetGenericArguments()[0];
                return _genericParameterType.IsGenericType &&
                      _genericParameterType.GetGenericTypeDefinition() == FUNC_GENERIC_TYPE &&
                      Resource.IsResourceRegistered(_genericParameterType.GetGenericArguments()[0], false);
            },
            (t) =>
            {
                var _argumentType = t.GetGenericArguments()[0];
                var _resourceType = _argumentType.GetGenericArguments()[0];
                return (ILocatorAdapter)TypeActivator.CreateInstance(ENUMFUNC_ADAPTER_TYPE.MakeGenericType(_resourceType));
            });

            // IEnumerable<Lazy<T, TMetadata>>
            Resource.RegisterLocatorAdapter(
            (t) =>
            {
                // we check if it is IEnumerable<> first
                if (!t.IsGenericType) return false;
                var _genericArgumentType = t.GetGenericTypeDefinition();
                if (_genericArgumentType != IENUM_GENERIC_TYPE) return false;

                // we get the parameter of the generic type and check if it is Lazy<T, TMetadata>
                var _genericParameterType = t.GetGenericArguments()[0];
                return _genericParameterType.IsGenericType &&
                      _genericParameterType.GetGenericTypeDefinition() == LAZYMETA_GENERIC_TYPE &&
                      Resource.IsResourceRegistered(_genericParameterType.GetGenericArguments()[0], false);
            },
            (t) =>
            {
                var _argumentType = t.GetGenericArguments()[0];
                var _resourceType = _argumentType.GetGenericArguments()[0];
                var _metadataType = _argumentType.GetGenericArguments()[1];
                return (ILocatorAdapter)TypeActivator.CreateInstance(ENUMLAZYMETA_ADAPTER_TYPE.MakeGenericType(_resourceType, _metadataType));
            });

            // ReadOnlyObservableCollection<T>
            Resource.RegisterLocatorAdapter(
            (t) => t.IsGenericType &&
                    t.GetGenericTypeDefinition() == READONLYOBSCOL_GENERIC_TYPE &&
                    Resource.IsResourceRegistered(t.GetGenericArguments()[0], false),
            (t) =>
            {
                var _resourceType = t.GetGenericArguments()[0];
                return (ILocatorAdapter)TypeActivator.CreateInstance(READONLYOBSV_ADAPTER_TYPE.MakeGenericType(_resourceType));
            });

            // ReadOnlyObservableCollection<Lazy<T>>
            Resource.RegisterLocatorAdapter(
            (t) =>
            {
                // we check if it is IEnumerable<> first
                if (!t.IsGenericType) return false;
                var _genericArgumentType = t.GetGenericTypeDefinition();
                if (_genericArgumentType != READONLYOBSCOL_GENERIC_TYPE) return false;

                // we get the parameter of the generic type and check if it is Lazy<T>
                var _genericParameterType = t.GetGenericArguments()[0];
                return _genericParameterType.IsGenericType &&
                      _genericParameterType.GetGenericTypeDefinition() == LAZY_GENERIC_TYPE &&
                      Resource.IsResourceRegistered(_genericParameterType.GetGenericArguments()[0], false);
            },
            (t) =>
            {
                var _argumentType = t.GetGenericArguments()[0];
                var _resourceType = _argumentType.GetGenericArguments()[0];
                return (ILocatorAdapter)TypeActivator.CreateInstance(READONLYOBSVLAZY_ADAPTER_TYPE.MakeGenericType(_resourceType));
            });

            // ReadOnlyObservableCollection<Lazy<T, Metadata>>
            Resource.RegisterLocatorAdapter(
            (t) =>
            {
                // we check if it is IEnumerable<> first
                if (!t.IsGenericType) return false;
                var _genericArgumentType = t.GetGenericTypeDefinition();
                if (_genericArgumentType != READONLYOBSCOL_GENERIC_TYPE) return false;

                // we get the parameter of the generic type and check if it is Lazy<T>
                var _genericParameterType = t.GetGenericArguments()[0];
                return _genericParameterType.IsGenericType &&
                      _genericParameterType.GetGenericTypeDefinition() == LAZYMETA_GENERIC_TYPE &&
                      Resource.IsResourceRegistered(_genericParameterType.GetGenericArguments()[0], false);
            },
            (t) =>
            {
                var _argumentType = t.GetGenericArguments()[0];
                var _resourceType = _argumentType.GetGenericArguments()[0];
                var _metadataType = _argumentType.GetGenericArguments()[1];
                return (ILocatorAdapter)TypeActivator.CreateInstance(READONLYOBSVLAZYMETA_ADAPTER_TYPE.MakeGenericType(_resourceType, _metadataType));
            });

            // IChannel<T>
            Resource.RegisterLocatorAdapter(
            (t) => t.IsGenericType && t.GetGenericTypeDefinition() == CHANNEL_GENERIC_TYPE,
            (t) =>
            {
                var _resourceType = t.GetGenericArguments()[0];
                return (ILocatorAdapter)TypeActivator.CreateInstance(CHANNEL_ADAPTER_TYPE.MakeGenericType(_resourceType));
            });
        }
    }
}
