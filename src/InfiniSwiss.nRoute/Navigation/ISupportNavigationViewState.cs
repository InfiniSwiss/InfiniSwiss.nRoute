using nRoute.Components;

namespace nRoute.Navigation
{
    /// <summary>
    /// Provides support for restoring and saving visual state whilst navigating
    /// </summary>
    public interface ISupportNavigationViewState
    {
        /// <summary>
        /// Restores the content state of the page.
        /// </summary>
        void RestoreState(ParametersCollection state);

        /// <summary>
        /// Returns the content state of the page.
        /// </summary>
        ParametersCollection SaveState();
    }
}