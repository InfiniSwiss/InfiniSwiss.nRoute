using nRoute.Components.Routing;
using System;

namespace nRoute.Navigation
{
    public interface ISupportNavigationFailure
    {
        NavigationRequest Request { get; set; }

        ResponseStatus ResponseStatus { get; set; }

        Exception Error { get; set; }

        NavigationRequest RefererRequest { get; set; }
    }
}
