using System;
using System.Windows;
using System.Windows.Markup;
using System.Xml.Serialization;

namespace nRoute.SiteMaps
{
    /// <summary>
    /// Represents a <see cref="SiteMap">Site Map</see> leaf, that can have zero or more sub-nodes.
    /// </summary>
    [Serializable]
    [XmlRoot(Namespace = SiteMaps.XMLNAMESPACE)]
    [ContentProperty("ChildNodes")]
    public abstract partial class SiteMapNode
        : DependencyObject
    {
        private const string KEY_CANONLY_BESETONCE = "SiteMapNode with Key ('{0}') cannot be changed once set";

        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register("Key",
            typeof(string), typeof(SiteMapNode), new PropertyMetadata(null));

        public static readonly DependencyProperty ChildNodesProperty = DependencyProperty.Register("ChildNodes",
            typeof(NodesCollection), typeof(SiteMapNode), new PropertyMetadata(null));

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title",
            typeof(string), typeof(SiteMapNode), new PropertyMetadata(null));

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description",
            typeof(string), typeof(SiteMapNode), new PropertyMetadata(null));

        public static readonly DependencyProperty IconPathProperty = DependencyProperty.Register("IconPath",
            typeof(string), typeof(SiteMapNode), new PropertyMetadata(null));

        public static readonly DependencyProperty SiteAreaProperty = DependencyProperty.Register("SiteArea",
            typeof(string), typeof(SiteMapNode), new PropertyMetadata(null));

        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register("IsEnabled",
            typeof(bool), typeof(SiteMapNode), new PropertyMetadata(true));

        public static readonly DependencyProperty IsListedProperty = DependencyProperty.Register("IsListed",
            typeof(bool), typeof(SiteMapNode), new PropertyMetadata(true));

        public static readonly DependencyProperty TagProperty = DependencyProperty.Register("Tag",
            typeof(object), typeof(SiteMapNode), new PropertyMetadata(null));

        #region Properties

        /// <summary>
        /// The unique key identifying the <see cref="SiteMapNode">Site Map Node</see>.
        /// </summary>
        [XmlElement(Namespace = SiteMaps.XMLNAMESPACE)]
        public string Key
        {
            get { return Convert.ToString(GetValue(KeyProperty)); }
            set { SetValue(KeyProperty, value); }
        }

        /// <summary>
        /// The title of the node.
        /// </summary>
        [XmlElement(Namespace = SiteMaps.XMLNAMESPACE)]
        public string Title
        {
            get { return Convert.ToString(GetValue(TitleProperty)); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// The description of the node.
        /// </summary>
        [XmlElement(Namespace = SiteMaps.XMLNAMESPACE)]
        public string Description
        {
            get { return Convert.ToString(GetValue(DescriptionProperty)); }
            set { SetValue(DescriptionProperty, value); }
        }

        /// <summary>
        /// The Icon Path for the node.
        /// </summary>
        [XmlElement(Namespace = SiteMaps.XMLNAMESPACE)]
        public string IconPath
        {
            get { return Convert.ToString(GetValue(IconPathProperty)); }
            set { SetValue(IconPathProperty, value); }
        }

        /// <summary>
        /// Child nodes of the nodes.
        /// </summary>
        [XmlArray(Namespace = SiteMaps.XMLNAMESPACE)]
        public NodesCollection ChildNodes
        {
            get { return (NodesCollection)GetValue(ChildNodesProperty); }
            set { SetValue(ChildNodesProperty, value); }
        }

        /// <summary>
        /// The area that the node belongs to.
        /// </summary>
        [XmlElement(Namespace = SiteMaps.XMLNAMESPACE)]
        public string SiteArea
        {
            get { return Convert.ToString(GetValue(SiteAreaProperty)); }
            set { SetValue(SiteAreaProperty, value); }
        }

        /// <summary>
        /// Indicates as to if the node is enabled.
        /// </summary>
        [XmlElement(Namespace = SiteMaps.XMLNAMESPACE)]
        public bool IsEnabled
        {
            get { return Convert.ToBoolean(GetValue(IsEnabledProperty)); }
            set { SetValue(IsEnabledProperty, value); }
        }

        /// <summary>
        /// Indicates as to if the node is listed in menus and other visual representations of the <see cref="SiteMap">Site Map</see>.
        /// </summary>
        [XmlElement(Namespace = SiteMaps.XMLNAMESPACE)]
        public bool IsListed
        {
            get { return Convert.ToBoolean(GetValue(IsListedProperty)); }
            set { SetValue(IsListedProperty, value); }
        }

        /// <summary>
        /// Allows for custom tag to be associated with the <see cref="SiteMapNode">SiteMapNode</see>
        /// </summary>
        [XmlElement(Namespace = SiteMaps.XMLNAMESPACE)]
        public object Tag
        {
            get { return GetValue(TagProperty); }
            set { SetValue(TagProperty, value); }
        }

        /// <summary>
        /// Indicates as to if the node has one or more children.
        /// </summary>
        [XmlIgnore]
        public bool HasChildNodes
        {
            get { return (this.ChildNodes != null && this.ChildNodes.Count > 0); }
        }

        #endregion

        #region Methods

        public abstract void Execute();

        #endregion

        #region Handlers

        private static void OnKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SiteMapNode)d).UpdateKey(Convert.ToString(e.OldValue), Convert.ToString(e.NewValue));
        }

        private void UpdateKey(string oldKey, string newKey)
        {
            if (!string.IsNullOrEmpty(oldKey))
            {
                throw new InvalidOperationException(KEY_CANONLY_BESETONCE);
            }
        }

        #endregion

    }
}