using nRoute.Components.Composition;
using System;

namespace nRoute.ViewServices.Contracts
{
    [MapAsKnownResource]
    public interface IShowIndicatorViewService
    {
        IDisposable ShowIndicator();

        IDisposable ShowIndicator(TimeSpan timeout);

        IDisposable ShowIndicator(string title);

        IDisposable ShowIndicator(string title, TimeSpan timeout);

        //void HideIndicator();
    }
}