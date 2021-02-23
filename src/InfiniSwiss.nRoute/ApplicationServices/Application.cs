using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace nRoute.ApplicationServices
{
    public abstract class Application
        : System.Windows.Application
    {

        #region Declarations

        private List<IApplicationService> _applicationLifetimeObjects;
        private ReadOnlyCollection<IApplicationService> _applicationLifetimeObjectsWrapper;
        private ApplicationServiceContext _applicationServiceContext;

        #endregion

        public Application()
            : base() { }

        #region Properties

        public IList ApplicationLifetimeObjects
        {
            get
            {
                if (_applicationLifetimeObjects == null)
                {
                    _applicationLifetimeObjects = new List<IApplicationService>();
                    if (!nRouteApplicationService.IsInitialized)
                    {
                        return _applicationLifetimeObjects;
                    }
                }

                if (_applicationLifetimeObjectsWrapper == null)
                {
                    _applicationLifetimeObjectsWrapper =
                        new ReadOnlyCollection<IApplicationService>(this._applicationLifetimeObjects);
                }
                return _applicationLifetimeObjectsWrapper;
            }
        }

        public static new Application Current
        {
            get { return System.Windows.Application.Current as Application; }
        }
        #endregion

        #region Overriden

        protected override void OnStartup(StartupEventArgs e)
        {
            _applicationServiceContext = new ApplicationServiceContext();   // maybe should parse in the startup args

            foreach (IApplicationService _service in this.ApplicationLifetimeObjects)
            {
                _service.StartService(_applicationServiceContext);

                if (_service is IApplicationLifetimeAware)
                {
                    ((IApplicationLifetimeAware)_service).Starting();
                }
            }

            base.OnStartup(e);

            foreach (IApplicationService service in this.ApplicationLifetimeObjects)
            {
                if (service is IApplicationLifetimeAware)
                {
                    ((IApplicationLifetimeAware)service).Started();
                }
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            foreach (IApplicationService service in this.ApplicationLifetimeObjects)
            {
                if (service is IApplicationLifetimeAware)
                {
                    ((IApplicationLifetimeAware)service).Exiting();
                }
            }

            base.OnExit(e);

            foreach (IApplicationService _service in this.ApplicationLifetimeObjects)
            {
                if (_service is IApplicationLifetimeAware)
                {
                    ((IApplicationLifetimeAware)_service).Exited();
                }
                _service.StopService();
            }
        }

        #endregion

    }
}

