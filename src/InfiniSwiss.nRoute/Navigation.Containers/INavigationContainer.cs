using System;

namespace nRoute.Navigation.Containers
{
    public interface INavigationContainer
        : INavigationHandler
    {
        event EventHandler<NavigatingCancelEventArgs> Navigating;
        event EventHandler<NavigatedEventArgs> Navigated;
        event EventHandler<NavigationFailedEventArgs> NavigationFailed;

        string Title { get; }

        string Url { get; set; }

        Object Content { get; }

        void Navigate(NavigationRequest request);
    }
}
