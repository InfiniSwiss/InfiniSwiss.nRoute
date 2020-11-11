using nRoute.Components;
using nRoute.Components.Composition;
using nRoute.Internal;
using nRoute.ViewServices.Contracts;
using System;
using System.Windows;

namespace nRoute.ViewServices.Services
{
    [MapViewService(typeof(IShowMessageViewService), ShowMessageViewService.SERVICE_NAME,
         InitializationMode = InitializationMode.OnDemand, Lifetime = ViewServiceLifetime.PerInstance)]
    public class ShowMessageViewService
         : IShowMessageViewService
    {
        private const string SERVICE_NAME = "nRouteMessageBoxViewService";

        #region IShowMessageViewService Members

        public IDisposable ShowMessage(string message, bool isCancellable)
        {
            return ShowMessage(null, message, isCancellable, null);
        }

        public IDisposable ShowMessage(string title, string message, bool isCancellable)
        {
            return ShowMessage(title, message, isCancellable, null);
        }

        public IDisposable ShowMessage(string message, bool isCancellable, Action<bool?> confirmationCallback)
        {
            return ShowMessage(null, message, isCancellable, confirmationCallback);
        }

        public IDisposable ShowMessage(string title, string message, bool isCancellable, Action<bool?> confirmationCallback)
        {
            Guard.ArgumentNotNullOrWhiteSpace(message, "message");

            var _result = MessageBox.Show(message, title, isCancellable ? MessageBoxButton.OKCancel : MessageBoxButton.OK);
            if (confirmationCallback != null)
            {
                var _confirmation = default(bool?);
                if (_result == MessageBoxResult.OK)
                {
                    _confirmation = true;
                }
                else if (_result == MessageBoxResult.Cancel)
                {
                    _confirmation = false;
                }
                confirmationCallback(_confirmation);
            }

            // note because we are using a message box the response is immediate - hence the disposable isn't used.
            return new RelayDisposable();
        }

        #endregion

    }
}
