using System;
using System.Xml.Serialization;

namespace nRoute.SiteMaps
{
    /// <summary>
    /// Represents a navigation and  structure for a app, which is provided by one or more site map providers. 
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "SiteMap", Namespace = SiteMaps.XMLNAMESPACE)]
    public class SiteMap
    {
        AreasCollection _areas;
        NavigationNode _rootNode;

        #region Properties

        /// <remarks>
        /// - Specifically we've made the root-node to be a navigation node
        /// </remarks>
        [XmlElement(Namespace = SiteMaps.XMLNAMESPACE)]
        public NavigationNode RootNode
        {
            get { return _rootNode; }
            set { _rootNode = value; }
        }


        [XmlElement(Namespace = SiteMaps.XMLNAMESPACE)]
        public AreasCollection Areas
        {
            get
            {
                if (_areas == null) _areas = new AreasCollection();
                return _areas;
            }
            set
            {
                _areas = value;
            }
        }

        #endregion

    }
}