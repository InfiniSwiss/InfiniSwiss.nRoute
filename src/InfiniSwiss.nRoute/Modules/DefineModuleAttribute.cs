using nRoute.Components.Composition;
using nRoute.Internal;
using System;

namespace nRoute.Modules
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Assembly,
        AllowMultiple = true, Inherited = false)]
    public class DefineModuleAttribute
         : MapModuleAttribute
    {
        private readonly Type _moduleType;

        public DefineModuleAttribute(Type moduleType, string name)
         : this(moduleType, name, null) { }

        public DefineModuleAttribute(Type moduleType, string name, params string[] moduleDependencies)

         : base(name, moduleDependencies)
        {
            Guard.ArgumentNotNull(moduleType, "moduleType");

            EnsureIsModuleType(moduleType);
            _moduleType = moduleType;
        }

        #region Overrides

        protected internal override IResourceLocator GetResourceLocator(Type targetType)
        {
            // we ovveride the target type
            return base.GetResourceLocator(_moduleType);
        }

        #endregion

    }
}
