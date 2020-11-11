using nRoute.Components.Composition;
using nRoute.Internal;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml.Serialization;

namespace nRoute.SiteMaps
{
    [Serializable]
    [XmlRoot(Namespace = SiteMaps.XMLNAMESPACE)]
    [ContentProperty("ChildNodes")]
    public class CommandNode
         : SiteMapNode
    {
        private const string COMMANDORNAME_MUSTBE_SPECIFIED = "A Command or Command Name must be specified";
        private const string COMMAND_NOT_RESOLVED = "Command with Name '{0}' could be resolved for CommandNode";
        private const string AREA_COULDNOT_INITIALIZE = "Could not initialize Site Area '{0}' to execute CommandNode with Name '{1}'";

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command",
            typeof(ICommand), typeof(CommandNode), new PropertyMetadata(null));

        public static readonly DependencyProperty CommandNameProperty = DependencyProperty.Register("CommandName",
            typeof(string), typeof(CommandNode), new PropertyMetadata(null));

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter",
            typeof(object), typeof(CommandNode), new PropertyMetadata(null));

        #region Properties

        [XmlIgnore]
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        [XmlElement(Namespace = SiteMaps.XMLNAMESPACE)]
        public string CommandName
        {
            get { return Convert.ToString(GetValue(CommandNameProperty)); }
            set { SetValue(CommandNameProperty, value); }
        }

        [XmlElement(Namespace = SiteMaps.XMLNAMESPACE)]
        public Object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        #endregion

        public override void Execute()
        {
            Guard.ArgumentValue(Command == null && string.IsNullOrEmpty(CommandName), "Command", COMMANDORNAME_MUSTBE_SPECIFIED);

            if (!this.IsEnabled) return;

            var _command = Command ?? ResourceLocator.GetResource<ICommand>(CommandName);
            Guard.ArgumentNotNull(_command, "Command", COMMAND_NOT_RESOLVED, CommandName);

            if (!string.IsNullOrEmpty(SiteArea))
            {
                SiteMapService.InitializeSiteArea(SiteArea, (b) =>
                {
                    if (!b) throw new SiteMapException(string.Format(AREA_COULDNOT_INITIALIZE, SiteArea, CommandName), SiteArea);
                    if (_command.CanExecute(CommandParameter))
                    {
                        _command.Execute(CommandParameter);
                    }
                });
            }
            else
            {
                if (_command.CanExecute(CommandParameter))
                {
                    _command.Execute(CommandParameter);
                }
            }
        }

    }
}