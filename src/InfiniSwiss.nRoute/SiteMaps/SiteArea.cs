using System.Windows.Markup;
using System.Xml.Serialization;

namespace nRoute.SiteMaps
{
    [XmlRoot(Namespace = SiteMaps.XMLNAMESPACE)]
    [ContentProperty("AreaDependencies")]
    public class SiteArea
    {
        private AreaInfosCollection _areaDependencies;

        /// <summary>
        /// The unique name of the SiteArea.
        /// </summary>
        [XmlElement(Namespace = SiteMaps.XMLNAMESPACE, IsNullable = false)]
        public string Key { get; set; }

        /// <summary>
        /// The url of the remote package (xap or dll) to download and map.
        /// </summary>
        [XmlElement(Namespace = SiteMaps.XMLNAMESPACE, IsNullable = false)]
        public string RemoteUrl { get; set; }

        /// <summary>
        /// Indicates as to if the resource is initialized on loading of the application.
        /// </summary>
        [XmlElement(Namespace = SiteMaps.XMLNAMESPACE)]
        public bool InitializeOnLoad { get; set; }

        /// <summary>
        /// Children
        /// </summary>
        [XmlArray(Namespace = SiteMaps.XMLNAMESPACE)]
        public AreaInfosCollection AreaDependencies
        {
            get
            {
                if (_areaDependencies == null) _areaDependencies = new AreaInfosCollection();
                return _areaDependencies;
            }
            set
            {
                _areaDependencies = value;
            }
        }

        [XmlIgnore]
        internal bool InternalIsInitialized { get; set; }

        /// <summary>
        /// Indicates as to if the area has dependencies on other areas
        /// </summary>
        [XmlIgnore]
        public bool HasDependencies
        {
            get { return (_areaDependencies != null && _areaDependencies.Count > 0); }
        }
    }
}
