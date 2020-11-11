using System.ComponentModel;
using System.Windows;

namespace nRoute.Components
{
    public interface IActionCommand
         : IReverseCommand, IWeakEventListener, INotifyPropertyChanged
    {
        bool IsActive { get; set; }

        void RequeryCanExecute();
    }
}
