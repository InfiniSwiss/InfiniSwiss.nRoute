using nRoute.Components.Handlers;
using nRoute.Internal;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Input;

namespace nRoute.Components
{
    public static class ActionCommandExtensions
    {
        public static T RequeryOnPropertyChanged<T>(this T command, INotifyPropertyChanged notifiable)
            where
                T : ICommand, IWeakEventListener
        {
            Guard.ArgumentNotDefault(command, "command");
            Guard.ArgumentNotDefault(notifiable, "notifiable");

            // we attach a weak listener to hear for any changes in the property
            notifiable.PropertyChanged += new WeakListenerHandler<PropertyChangedEventArgs, PropertyChangedEventHandler>
                (command, (h) => notifiable.PropertyChanged -= h);
            return command;
        }

        public static T RequeryOnCommandCanExecuteChanged<T>(this T command, ICommand relatedCommand)
            where
                T : ICommand, IWeakEventListener
        {
            Guard.ArgumentNotDefault(command, "command");
            Guard.ArgumentNotNull(relatedCommand, "relatedCommand");

            // we attach a weak listener to hear for any changes in the other command
            relatedCommand.CanExecuteChanged += new WeakListenerHandler<EventArgs, EventHandler>(command,
                (h) => relatedCommand.CanExecuteChanged -= h);
            return command;
        }

        public static T RequeryOnCommandExecuted<T>(this T command, IActionCommand relatedCommand)
            where
                T : ICommand, IWeakEventListener
        {
            Guard.ArgumentNotDefault(command, "command");
            Guard.ArgumentNotNull(relatedCommand, "relatedCommand");

            // we attach a weak listener to hear for any changes in the other command
            relatedCommand.CommandExecuted += new WeakListenerHandler<CommandEventArgs, EventHandler<CommandEventArgs>>(command,
                (h) => relatedCommand.CommandExecuted -= h);
            return command;
        }

        public static T RequeryWhenExecuted<T>(this T command)
            where
                T : IActionCommand
        {
            Guard.ArgumentNotDefault(command, "command");

            // we attach a weak listener to hear for any changes in the other command
            command.CommandExecuted += new WeakListenerHandler<CommandEventArgs, EventHandler<CommandEventArgs>>(command,
                (h) => command.CommandExecuted -= h);
            return command;
        }

        public static T RequeryOnCollectionChanged<T>(this T command, INotifyCollectionChanged collection)
            where
                T : ICommand, IWeakEventListener
        {
            Guard.ArgumentNotDefault(command, "command");
            Guard.ArgumentNotNull(collection, "collection");

            // we attach a weak listener to hear for any changes in the collection
            collection.CollectionChanged += new WeakListenerHandler<NotifyCollectionChangedEventArgs, NotifyCollectionChangedEventHandler>
                (command, (h) => collection.CollectionChanged -= h);
            return command;
        }
    }
}
