using nRoute.Components.Messaging;
using nRoute.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace nRoute.Components.Mapping
{
    public static class AssemblyMapper
    {
        private static readonly List<Assembly> _mappedAssemblies;
        private static readonly Object _lock = new Object();

        static AssemblyMapper()
        {
            _mappedAssemblies = new List<Assembly>();
            MapAssembly(typeof(AssemblyMapper).Assembly);
        }

        #region Static Methods

        public static void GetAttributesInMappedAssembly<T>(Assembly assembly, Action<T, Type> mapAction)
            where
                T : Attribute
        {
            Guard.ArgumentNotNull(assembly, "assembly");
            Guard.ArgumentNotNull(mapAction, "mapAction");

            // we get all the attributes type , note_ a type can have more than one instance 
            // of the given attribute applied..
            var _types = assembly.GetTypes();
            var _mappedAttributes = (from _type in _types
                                     let _attribs = _type.GetCustomAttributes(typeof(T), false).Cast<T>().ToList()
                                     where (_attribs != null && _attribs.Count > 0)
                                     select new MappedAttributes<T>()
                                     {
                                         TargetType = _type,
                                         TargetAttributes = _attribs
                                     }).ToList();

            // we specifically add all the assembly-level attributes
            var _assemblyAttributes = assembly.GetCustomAttributes(typeof(T), false);
            if (_assemblyAttributes != null && _assemblyAttributes.Length > 0)
            {
                _mappedAttributes.Add(new MappedAttributes<T>()
                {
                    TargetType = assembly.GetType(),
                    TargetAttributes = _assemblyAttributes.Cast<T>().ToList()
                });
            }

            // we get all the applications of the a given attribute
            foreach (var _mappedAttribute in _mappedAttributes)
            {
                // we need to check for each possible attribute applied 
                foreach (var _attribute in _mappedAttribute.TargetAttributes)
                {
                    mapAction(_attribute, _mappedAttribute.TargetType);
                }
            }
        }

        public static void GetAttributesInMappedAssemblies<T>(Action<T, Type> mapAction)
            where
                T : Attribute
        {
            Guard.ArgumentNotNull(mapAction, "mapAction");

            var _assemblies = MappedAssemblies;
            foreach (var _assembly in _assemblies)
            {
                GetAttributesInMappedAssembly<T>(_assembly, mapAction);
            }
        }

        public static void MapAssembly(Assembly assembly)
        {
            Guard.ArgumentNotNull(assembly, "assembly");

            lock (_lock)
            {
                if (!_mappedAssemblies.Contains(assembly))
                {
                    _mappedAssemblies.Add(assembly);
                    Channel.Publish<MappedAssemblyInfo>(new MappedAssemblyInfo(assembly));
                }
            }
        }

        public static void MapApplicationAssemblies()
        {
            lock (_lock)
            {
                // we get all the included assemblies
                var _applicationAssembly = GetManifestIncludedAssemblies();

                // and map all listed assemblies - note_ the mapping is actually using an observable channel
                foreach (var _assembly in _applicationAssembly)
                {
                    if (!_mappedAssemblies.Contains(_assembly))
                    {
                        _mappedAssemblies.Add(_assembly);
                        Channel.Publish<MappedAssemblyInfo>(new MappedAssemblyInfo(_assembly));
                    }
                }
            }
        }

        public static bool IsAssemblyMapped(Assembly assembly)
        {
            lock (_lock)
            {
                // note_ in SL, we can't unload an assembly once loaded
                return _mappedAssemblies.Contains(assembly);
            }
        }

        public static IEnumerable<Assembly> MappedAssemblies
        {
            get
            {
                Assembly[] _assemblies = null;
                lock (_lock)
                {
                    _assemblies = _mappedAssemblies.ToArray();
                }
                return _assemblies;
            }
        }

        #endregion

        #region Helpers

        public static IEnumerable<Assembly> GetManifestIncludedAssemblies()
        {

            return AppDomain.CurrentDomain.GetAssemblies();

        }

        #endregion

        #region Private Class

        private class MappedAttributes<T>
            where
                T : Attribute
        {
            public Type TargetType { get; set; }

            public IEnumerable<T> TargetAttributes { get; set; }
        }

        #endregion

    }
}