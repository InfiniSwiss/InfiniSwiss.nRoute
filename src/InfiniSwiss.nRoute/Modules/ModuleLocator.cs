using nRoute.ApplicationServices;
using nRoute.Components;
using nRoute.Components.Composition;
using nRoute.Components.Messaging;
using nRoute.Internal;
using System.Collections.Specialized;

namespace nRoute.Modules
{
    public static class ModuleLocator
    {

        #region Initialize Related

        internal static void Initialize()
        {
            // once the app has started, we initialize all the when available modules 
            Channel<ApplicationStateInfo>.Public.Subscribe((s) =>
            {
                if (s.CurrentState == ApplicationState.Started)
                {
                    var _moduleLocators = Resource<IModule>.Catalog;
                    foreach (var _moduleLocator in _moduleLocators)
                    {
                        var _moduleMeta = _moduleLocator.ResourceMeta as ModuleMeta;
                        if (_moduleMeta != null && _moduleMeta.InitializationMode == InitializationMode.WhenAvailable)
                            _moduleLocator.GetResourceInstance();
                    }
                }
            });

            // and we also monitor for any new, we don't care about removed ones
            Resource<IModule>.Catalog.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    var _moduleLocator = (IResourceLocator)e.NewItems[0];
                    var _moduleMeta = _moduleLocator.ResourceMeta as ModuleMeta;
                    if (_moduleMeta != null && _moduleMeta.InitializationMode == InitializationMode.WhenAvailable)
                        _moduleLocator.GetResourceInstance();
                }
            };
        }

        #endregion

        #region Static Methods

        public static IModule GetModule(string name)
        {
            Guard.ArgumentNotNullOrWhiteSpace(name, "name");
            return ResourceLocator.GetResource<IModule>(name);
        }

        public static bool TryGetModule(string name, out IModule moduleInstance)
        {
            Guard.ArgumentNotNullOrWhiteSpace(name, "name");
            return ResourceLocator.TryGetResource<IModule>(name, out moduleInstance);
        }

        public static bool IsModuleRegistered(string name)
        {
            Guard.ArgumentNotNullOrWhiteSpace(name, "name");
            return ResourceLocator.IsResourceRegistered<IModule>(name);
        }

        #endregion

    }
}

//#region Module Instance Related

//        public static void RegisterModule(string name, Type moduleType)
//        {
//            RegisterModule(name, moduleType, InitializationMode.OnDemand);
//        }

//        public static void RegisterModule(string name, Type moduleType,
//            InitializationMode initializationMode)
//        {
//            
//            if (string.IsNullOrEmpty(name)) Guard.ArgumentNotNull(, "name");
//            Guard.ArgumentNotNull(moduleType, "moduleType");

//            // we create a locator and using a provider and register it against the IModule
//            var _moduleLocator = new DefaultModulesLocator(moduleType, name, initializationMode);
//            Resource.RegisterResourceLocator(typeof(IModule), _moduleLocator, false);
//        }

//        public static void UnregisterModule(string name)
//        {
//            
//            if (string.IsNullOrEmpty(name)) Guard.ArgumentNotNull(, "name");

//            // we remove the view's registeed view model
//            Resource.UnregisterResourceLocator(typeof(IModule), name);
//        }

//#endregion