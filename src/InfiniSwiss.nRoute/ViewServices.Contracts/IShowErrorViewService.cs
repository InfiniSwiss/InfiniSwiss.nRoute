using nRoute.Components.Composition;
using System;

namespace nRoute.ViewServices.Contracts
{
    [MapAsKnownResource]
    public interface IShowErrorViewService
    {
        void ShowError(Exception error);

        void ShowError(string title, string message);

        void ShowError(string title, string message, Exception error);
    }
}
