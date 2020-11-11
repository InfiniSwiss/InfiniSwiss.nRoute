using nRoute.Internal;
using System;

namespace nRoute.Navigation.Containers
{
    public class NavigationContainerEventArgs
         : EventArgs
    {
        private readonly INavigationContainer _container;

        public NavigationContainerEventArgs(INavigationContainer container)
        {
            Guard.ArgumentNotNull(container, "container");
            _container = container;
        }

        #region Properties

        public INavigationContainer Container
        {
            get
            {
                return _container;
            }
        }

        #endregion

    }
}
