using nRoute.Components.Composition;
using System;

namespace nRoute.ViewServices.Contracts
{
    [MapAsKnownResource]
    public interface IShowStatusViewService
    {
        IDisposable ShowStatus(string status);

        IDisposable ShowStatus(string status, TimeSpan timeout);

        IDisposable ShowStatus(string status, Action acknowledgementCallback);

        IDisposable ShowStatus(string status, TimeSpan timeout, Action acknowledgementCallback);

        //void HideStatus();
    }
}