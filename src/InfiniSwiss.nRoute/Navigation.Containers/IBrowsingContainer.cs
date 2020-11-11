using System.Collections.Generic;

namespace nRoute.Navigation.Containers
{
    public interface IBrowsingContainer
         : INavigationContainer, ISupportDirectionalNavigation, ISupportJournalPurging
    {

        //void NavigateBack();

        //void NavigateForward();

        //bool CanNavigateBack { get; }

        //bool CanNavigateForward { get; }

        IEnumerable<PageContentState> BackPagesJournal { get; }

        IEnumerable<PageContentState> ForwardPagesJournal { get; }
    }

}
