using System.Collections.Generic;

namespace nRoute.Navigation.Containers
{
    public interface ISupportDirectionalNavigation
    {
        void NavigateBack(IEnumerable<string> toAnyOfUrls = null);

        void NavigateForward();

        bool CanNavigateBack { get; }

        bool CanNavigateForward { get; }
    }
}
