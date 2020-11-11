using nRoute.Navigation.Containers;
using System;

namespace nRoute.Behaviors
{
    public class PurgeNavigationJournalAction
        : NavigationHandlerActionBase
    {
        private const string HANDLER_NOTFOUND = "Could not resolve navigation handler for PurgeNavigationJournalAction";
        private const string HANDLER_MUSTSUPPORT_PURGING = "Navigation handler does not support purging journal (ISupportJournalPurging)";

        #region Overrides

        protected override void Invoke(object parameter)
        {
            if (this.AssociatedObject == null) return;

            // get the handler and if handler is not found then throw
            var _handler = base.ResolveHandler();
            if (_handler == null)
            {
                throw new InvalidOperationException(HANDLER_NOTFOUND);
            }

            // get the directional handler
            var _journalHandler = _handler as ISupportJournalPurging;
            if (_journalHandler == null)
            {
                throw new InvalidOperationException(HANDLER_MUSTSUPPORT_PURGING);
            }

            // we navigate if can do..
            if (_journalHandler.CanPurgeJournal)
            {
                _journalHandler.PurgeJournal();
            }
        }

        #endregion

    }
}