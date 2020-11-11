using nRoute.Components.Composition;
using nRoute.Internal;
using nRoute.Services;
using System;

namespace nRoute.Modules
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class MapModuleAttribute
         : MapResourceBaseAttribute
    {
        private const string TYPE_MUST_BE_IMODULE = "{0} type must be an implementation of IModule.";

        private string _name;
        private readonly string[] _moduleDependencies;

        public MapModuleAttribute()
            : this(null, null) { }

        public MapModuleAttribute(string name)
            : this(name, null) { }

        public MapModuleAttribute(string name, params string[] moduleDependencies)
        {
            _name = name;
            _moduleDependencies = moduleDependencies;
            InitializationMode = InitializationMode.WhenAvailable;
        }

        public InitializationMode InitializationMode { get; set; }

        #region Overrides

        protected internal override bool CanInitialize(Type targetType)
        {
            if (_moduleDependencies == null || _moduleDependencies.Length == 0) return true;
            foreach (var _moduleDependency in _moduleDependencies)
            {
                if (!ServiceLocator.IsServiceRegistered(typeof(IModule), _moduleDependency)) return false;
            }
            return true;
        }

        protected internal override Type GetResourceType(Type targetType)
        {
            return typeof(IModule);
        }

        protected internal override IResourceLocator GetResourceLocator(Type targetType)
        {
            Guard.ArgumentNotNull(targetType, "targetType");
            EnsureIsModuleType(targetType);

            if (string.IsNullOrEmpty(_name)) _name = targetType.Name;
            return new DefaultModulesLocator(new ModuleMeta(targetType, _name, InitializationMode));
        }

        #endregion

        #region Helper

        protected static void EnsureIsModuleType(Type targetType)
        {
            if (!typeof(IModule).IsAssignableFrom(targetType)) throw new ArgumentException(
                string.Format(TYPE_MUST_BE_IMODULE, targetType.FullName), "targetType");
        }

        #endregion

    }
}
