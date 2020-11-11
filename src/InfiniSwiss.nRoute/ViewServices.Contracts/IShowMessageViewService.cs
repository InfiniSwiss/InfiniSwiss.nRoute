using nRoute.Components.Composition;
using System;

namespace nRoute.ViewServices.Contracts
{
    [MapAsKnownResource]
    public interface IShowMessageViewService
    {
        IDisposable ShowMessage(string message, bool isCancellable);

        IDisposable ShowMessage(string title, string message, bool isCancellable);

        IDisposable ShowMessage(string message, bool isCancellable, Action<bool?> confirmationCallback);

        IDisposable ShowMessage(string title, string message, bool isCancellable, Action<bool?> confirmationCallback);
    }
}
