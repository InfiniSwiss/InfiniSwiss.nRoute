using System;
using System.Xml.Serialization;

namespace nRoute.SiteMaps
{
    [Serializable]
    [XmlRoot(Namespace = SiteMaps.XMLNAMESPACE)]
    public class SiteAreaInfo
    {
        [XmlElement(Namespace = SiteMaps.XMLNAMESPACE)]
        public string Key { get; set; }

        [XmlIgnore]
        public bool IsInitialized
        {
            get { return InternalIsInitialzied; }
        }

        [XmlIgnore]
        internal bool InternalIsInitialzied { get; set; }
    }
}
