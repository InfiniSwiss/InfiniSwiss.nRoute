using nRoute.Components.Composition;
using nRoute.ViewServices.Contracts;
using System;
using System.Windows;

namespace nRoute.ViewServices.Services
{
    [MapViewService(typeof(IShowErrorViewService), ShowErrorViewService.SERVICE_NAME,
         InitializationMode = InitializationMode.OnDemand, Lifetime = ViewServiceLifetime.PerInstance)]
    public class ShowErrorViewService
         : IShowErrorViewService
    {
        private const string SERVICE_NAME = "nRouteShowErrorViewService";
        private const string DEFAULT_ERROR_MESSAGE = "An unspecified error occurred.";

        #region IShowErrorService Members

        public void ShowError(Exception error)
        {
            ShowError(null, null, error);
        }

        public void ShowError(string title, string message)
        {
            ShowError(title, message, null);
        }

        public void ShowError(string title, string message, Exception error)
        {
            var _message = message;
            if (string.IsNullOrEmpty(message))
            {
                _message = (error != null) ? error.Message : DEFAULT_ERROR_MESSAGE;
            }

            if (string.IsNullOrEmpty(title))
            {
                MessageBox.Show(_message);
            }
            else
            {
                MessageBox.Show(_message, title, MessageBoxButton.OK);
            }
        }

        #endregion

    }
}