using nRoute.Components.Messaging;
using System;

namespace nRoute.ApplicationServices
{
    public partial class nRouteApplicationService
        : IApplicationService, IApplicationLifetimeAware
    {
        private const string APPSERVICE_ALREADY_INIT = "nRoute Application Service is already initialized";
        private readonly static object _lock = new object();

        private static ApplicationState _state;
        private static bool _isInitialized;

        #region Properties

        public static bool IsInitialized
        {
            get
            {
                lock (_lock)
                {
                    return _isInitialized;
                }
            }
        }

        public static ApplicationState CurrentApplicationState
        {
            get
            {
                lock (_lock)
                {
                    return _state;
                }
            }
        }

        #endregion

        #region IApplicationService Members

        public void StartService(ApplicationServiceContext context)
        {
            if (!IsInitialized) Initialize();
        }

        public void StopService()
        {
            // do nothing
        }

        #endregion

        #region IApplicationLifetimeAware Members

        public void Exited()
        {
            PublishApplicationState(ApplicationState.Exited);
        }

        public void Exiting()
        {
            PublishApplicationState(ApplicationState.Exiting);
        }

        public void Started()
        {
            PublishApplicationState(ApplicationState.Started);
        }

        public void Starting()
        {
            PublishApplicationState(ApplicationState.Starting);
        }

        #endregion

        #region Helpers

        private static void Initialize()
        {
            // we check
            if (_isInitialized)
            {
                throw new InvalidOperationException(APPSERVICE_ALREADY_INIT);
            }

            lock (_lock)
            {
                // double check
                if (_isInitialized) return;

                // initialize the basic stuff
                nRoute.Components.Composition.Resource.Initialize();
                nRoute.Navigation.NavigationService.Initialize();
                nRoute.Controllers.ControllerService.Initialize();
                nRoute.SiteMaps.SiteMapService.Initialize();
                nRoute.Modules.ModuleLocator.Initialize();

                // and set 
                _isInitialized = true;
            }

        }

        private static void PublishApplicationState(ApplicationState state)
        {
            lock (_lock)
            {
                _state = state;
                Channel.Publish<ApplicationStateInfo>(new ApplicationStateInfo(state));
            }
        }

        #endregion

    }
}