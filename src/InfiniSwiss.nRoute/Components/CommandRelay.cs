using System;
using System.ComponentModel;
using System.Windows.Input;

namespace nRoute.Components
{
    public class CommandRelay
         : ICommand, INotifyPropertyChanged
    {
        ICommand _value;

        public event EventHandler<ValueEventArgs<ICommand>> Initialize;

        public ICommand Command
        {
            get { return _value; }
            set
            {
                _value = value;
                PropertyChanged.Notify(() => Command);
            }
        }

        #region ICommand Members

        bool ICommand.CanExecute(object parameter)
        {
            if (_value == null && !InitializeCommand()) return false;
            return _value.CanExecute(parameter);
        }

        event EventHandler ICommand.CanExecuteChanged
        {
            add { if (_value != null || InitializeCommand()) _value.CanExecuteChanged += value; }
            remove { if (_value != null || InitializeCommand()) _value.CanExecuteChanged -= value; }
        }

        void ICommand.Execute(object parameter)
        {
            if (_value != null || InitializeCommand()) _value.Execute(parameter);
        }

        #endregion

        #region INotifyPropertyChanged Related

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Helper

        private bool InitializeCommand()
        {
            if (Command != null) return true;
            if (Initialize != null)
            {
                var _eventArgs = new ValueEventArgs<ICommand>();
                Initialize(this, _eventArgs);
                Command = _eventArgs.Value;
            }
            return (this.Command != null);
        }

        #endregion

    }
}
