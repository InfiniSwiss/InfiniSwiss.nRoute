using System;

namespace nRoute.Navigation.Containers
{
    public interface ISupportJournalPurging
    {
        event EventHandler<NavigationContainerEventArgs> JournalPurged;

        bool CanPurgeJournal { get; }

        void PurgeJournal();
    }
}
