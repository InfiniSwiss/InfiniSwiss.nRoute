using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace nRoute.Controls
{
    [ContentProperty]
    public partial class NavigationContainer
         : NavigationContentControl
    {
        protected internal const string UNNAVIGATED_STATE = "UnNavigated";                  // NavigationState.UnNavigated
        protected internal const string NAVIGATING_STATE = "Navigating";                     // NavigationState.Navigating
        protected internal const string NAVIGATED_STATE = "Navigated";                      // NavigationState.Navigated
        protected internal const string NAVIGATION_FAILED_STATE = "NavigationFailed";       // NavigationState.NavigationFailed
        protected internal const string NAVIGATION_STATES = "NavigationStates";

        #region Initialize Related

        protected override void InitializeComponent()
        {
            // call the base 
            base.InitializeComponent();

            // we set the default style
            this.DefaultStyleKey = typeof(NavigationContainer);

            // we also indicate the default state
            TransitionToNavigationState(NavigationState.UnNavigated, true);
        }

        #endregion

        #region Helpers

        protected override void TransitionToNavigationState(NavigationState navigationState)
        {
            this.TransitionToNavigationState(navigationState, true);
        }

        protected virtual void TransitionToNavigationState(NavigationState navigationState, bool useTransitions)
        {
            VisualStateManager.GoToState(this as Control, navigationState.ToString(), useTransitions);
            base.TransitionToNavigationState(navigationState);
        }

        #endregion

    }
}


