using nRoute.Internal;
using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Input;

namespace nRoute.Components
{
    public abstract class CommandBase<T>
        : IActionCommand
    {
        protected const string ERROR_EXPECTED_TYPE = "Expected parameter for command ({0}) must be of {1} type";

        private EventHandler _canExecuteHandler;
        private bool _isActive = true;

        public CommandBase() { }

        public CommandBase(bool isActive) : this()
        {
            _isActive = isActive;
        }

        #region Additional Methods

        public virtual bool CanExecute(T parameter)
        {
            return IsActive && OnCanExecute(parameter);
        }

        public virtual void Execute(T parameter)
        {
            if (CanExecute(parameter))
            {
                OnExecute(parameter);
                OnCommandExecuted(new CommandEventArgs(parameter));
            }
        }

        protected virtual void OnRequeryCanExecute()
        {
            if (_canExecuteHandler != null) _canExecuteHandler(this, EventArgs.Empty);
        }

        #endregion

        #region Abstract Methods

        protected abstract bool OnCanExecute(T parameter);

        protected abstract void OnExecute(T parameter);

        #endregion

        #region IActionCommand Members

        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    PropertyChanged.Notify(() => IsActive);
                    RequeryCanExecute();
                }
            }
        }

        public void RequeryCanExecute()
        {
            OnRequeryCanExecute();
        }

        #endregion

        #region IReverseCommand Members

        public event EventHandler<CommandEventArgs> CommandExecuted;

        #endregion

        #region ICommand Members

        bool ICommand.CanExecute(object parameter)
        {
            CheckParameterType(parameter);
            return CanExecute(ParseParameter(parameter, typeof(T)));
        }

        event EventHandler ICommand.CanExecuteChanged
        {
            add { _canExecuteHandler += value; }
            remove { _canExecuteHandler -= value; }
        }

        void ICommand.Execute(object parameter)
        {
            CheckParameterType(parameter);
            Execute(ParseParameter(parameter, typeof(T)));
        }

        #endregion

        #region IWeakEventListener Members

        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            RequeryCanExecute();
            return true;        // as in always listening
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged<P>(Expression<Func<P>> propertySelector)
        {
            Guard.ArgumentNotNull(propertySelector, "propertySelector");
            PropertyChanged.Notify<P>(propertySelector);
        }

        #endregion

        #region Helpers

        protected void OnCommandExecuted(CommandEventArgs args)
        {
            if (CommandExecuted != null) CommandExecuted(this, args);
        }

        protected virtual T ParseParameter(object parameter, Type parseAsType)
        {
            if (parameter == null) return default(T);
            if (parseAsType.IsEnum)
            {
                return (T)Enum.Parse(parseAsType, Convert.ToString(parameter), true);
            }
            else if (parseAsType.IsValueType)
            {
                return (T)Convert.ChangeType(parameter, parseAsType, null);
            }
            else
            {
                return (T)parameter;
            }
        }

        protected void CheckParameterType(object parameter)
        {
            if (parameter == null) return;
            if (typeof(T).IsValueType) return;

            Guard.ArgumentValue((!typeof(T).IsAssignableFrom(parameter.GetType())), "parameter", ERROR_EXPECTED_TYPE,
                this.GetType().FullName, typeof(T).FullName);
        }

        #endregion

    }
}