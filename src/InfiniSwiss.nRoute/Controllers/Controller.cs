using nRoute.Components;
using nRoute.Navigation;
using System;

namespace nRoute.Controllers
{
    public class Controller : ControllerBase, IDisposable
    {

        #region Navigate ViewResult

        protected internal ActionResult Navigate(string url)
        {
            return this.Navigate(null, url, null);
        }

        protected internal ActionResult Navigate(string url, ParametersCollection parameters)
        {
            return this.Navigate(null, url, parameters);
        }

        protected internal ActionResult Navigate(INavigationHandler navigationHandler, string url)
        {
            return this.Navigate(navigationHandler, url, null);
        }

        protected internal ActionResult Navigate(INavigationHandler navigationHandler, string url, ParametersCollection parameters)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Overridable

        protected virtual void Dispose(bool disposing) { }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }
}
