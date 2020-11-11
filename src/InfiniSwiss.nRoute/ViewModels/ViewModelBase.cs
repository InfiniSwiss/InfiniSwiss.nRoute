using nRoute.Components;
using nRoute.Internal;
using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace nRoute.ViewModels
{
    public abstract class ViewModelBase
         : ISupportView, INotifyPropertyChanged
    {

        #region ISupportView related

        void ISupportView.Loaded()
        {
            OnLoaded();
        }

        async Task ISupportView.LoadedAsync()
        {
            await OnLoadedAsync();
        }

        void ISupportView.Unloaded()
        {
            OnUnloaded();
        }

        #endregion

        #region Overridable

        //[Obsolete("Use OnLoadedAsync instead")]
        protected virtual void OnLoaded() { }

        protected virtual Task OnLoadedAsync() { return Task.CompletedTask; }

        protected virtual void OnUnloaded() { }

        #endregion

        #region INotifyPropertyChanged related

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged<T>(Expression<Func<T>> propertySelector)
        {
            Guard.ArgumentNotNull(propertySelector, "propertySelector");
            PropertyChanged.Notify<T>(propertySelector);
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void NotifyAllPropertiesChanged()
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(string.Empty));
            }
        }

        #endregion

    }
}