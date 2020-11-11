using nRoute.Components;
using nRoute.Navigation;
using System;
using System.Threading.Tasks;

namespace nRoute.ViewModels
{
    public abstract class NavigationViewModelBase
         : ViewModelBase, ISupportNavigationLifecycle, ISupportNavigationState
    {
        private string _title;

        #region ISupportNavigationLifecycle Members

        string ISupportNavigationLifecycle.Title
        {
            get { return Title; }
        }

        void ISupportNavigationLifecycle.Initialize(ParametersCollection requestParameters)
        {
            OnIntialize(requestParameters);
        }

        async Task ISupportNavigationLifecycle.InitializeAsync(ParametersCollection requestParameters)
        {
            await OnInitializeAsync(requestParameters);
        }

        void ISupportNavigationLifecycle.Closing(Action<bool> confirmCallback)
        {
            OnClosing(confirmCallback);
        }

        #endregion

        #region INavigationStateManager Members

        void ISupportNavigationState.RestoreState(ParametersCollection state)
        {
            OnRestoreState(state);
        }

        ParametersCollection ISupportNavigationState.SaveState()
        {
            return OnSaveState();
        }

        #endregion

        #region Properties

        protected string Title
        {
            get { return _title; }
            set
            {
                if (!string.Equals(_title, value, StringComparison.InvariantCultureIgnoreCase))
                {
                    _title = value;
                    NotifyPropertyChanged(() => Title);
                }
            }
        }

        #endregion

        #region Overridables

        //[Obsolete("Use OnInitializeAsync instead")]
        protected virtual void OnIntialize(ParametersCollection state) { }

        protected virtual Task OnInitializeAsync(ParametersCollection state) { return Task.CompletedTask; }

        protected virtual void OnClosing(Action<bool> confirmCallback)
        {
            if (confirmCallback != null) confirmCallback(true);     // by default always true
        }

        protected virtual void OnRestoreState(ParametersCollection state) { }

        protected virtual ParametersCollection OnSaveState()
        {
            return null;
        }

        #endregion

    }
}
