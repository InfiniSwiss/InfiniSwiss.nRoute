using nRoute.ApplicationServices;
using nRoute.Components.Composition;
using nRoute.Internal;

namespace nRoute.Modules
{
    public class DefaultModulesLocator
         : ResourceLocatorBase<IModule, ModuleMeta>
    {
        private IModule _instance;
        private readonly object _lock = new object();

        public DefaultModulesLocator(ModuleMeta meta)
            : base(meta.ModuleName, meta)
        {
            Guard.ArgumentNotNull(meta, "meta");

            if (meta.InitializationMode == InitializationMode.WhenAvailable &&
                nRouteApplicationService.CurrentApplicationState >= ApplicationState.Started)
            {
                InstanciateAndInitializeModule();
            }
        }

        #region IResolveResource Members

        public override IModule GetResourceInstance()
        {
            if (_instance == null) InstanciateAndInitializeModule();
            return _instance;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // TODO: check if locks in disposer are a good idea? 
                lock (_lock)
                {
                    if (_instance != null)
                    {
                        _instance = null;
                    }
                }
            }
        }

        #endregion

        #region Helper

        private void InstanciateAndInitializeModule()
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = CreateModuleTypeInstance();
                    _instance.Initialize();
                }
            }
        }

        protected virtual IModule CreateModuleTypeInstance()
        {
            return (IModule)TypeBuilder.BuildType(ResourceMeta.ModuleType);
        }

        #endregion

    }
}
