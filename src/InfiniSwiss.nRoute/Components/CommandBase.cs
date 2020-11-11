﻿using System;

namespace nRoute.Components
{
    public abstract class CommandBase
        : CommandBase<Object>
    {
        public CommandBase() : this(true) { }

        public CommandBase(bool isActive)
            : base(isActive) { }


        #region Additional Methods

        public virtual bool CanExecute()
        {
            return IsActive && OnCanExecute();
        }

        public virtual void Execute()
        {
            // here we go directly to OnExecute - also we call OnCommandExecuted as this is not called from the base class
            if (CanExecute())
            {
                OnExecute();
                OnCommandExecuted(new CommandEventArgs(null));
            }
        }

        public override void Execute(object parameter)
        {
            Execute();      // we redirect this to the non-parameter version, this also ensures we don't call OnCommandExecuted twice
        }

        #endregion

        #region Abstract Methods

        protected abstract bool OnCanExecute();

        protected abstract void OnExecute();

        #endregion

        #region Overrides

        protected override bool OnCanExecute(object parameter)
        {
            return CanExecute();      // ignores the parameter
        }

        protected override void OnExecute(object parameter)
        {
            Execute();              // ignores the parameter
        }

        #endregion

    }
}
