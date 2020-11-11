using System.Collections.Generic;

namespace nRoute.Navigation.Containers
{
    public interface IStatefulContainer
         : INavigationContainer, ISupportJournalPurging
    {
        IEnumerable<PageContentState> NavigatedPagesJournal { get; }

        bool HasState(string url);
    }
}
