using nRoute.Components.Composition;
using nRoute.Internal;
using System;

namespace nRoute.Modules
{
    public class ModuleMeta
    {
        private readonly string _moduleName;
        private readonly Type _moduleType;
        private readonly InitializationMode _initializationMode;

        public ModuleMeta(Type moduleType, string moduleName, InitializationMode initializationMode)
        {
            Guard.ArgumentNotNull(moduleType, "moduleType");
            if (string.IsNullOrEmpty(moduleName)) moduleName = moduleType.FullName;

            _moduleType = moduleType;
            _moduleName = moduleName;
            _initializationMode = initializationMode;
        }

        #region Properties

        public string ModuleName
        {
            get { return _moduleName; }
        }

        public Type ModuleType
        {
            get { return _moduleType; }
        }

        public InitializationMode InitializationMode
        {
            get { return _initializationMode; }
        }

        #endregion

    }
}
