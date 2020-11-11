using System;
using System.Windows;
using System.Xml.Serialization;

namespace nRoute.SiteMaps
{
    [Serializable]
    [XmlRoot(ElementName = "Nodes", Namespace = SiteMaps.XMLNAMESPACE)]
    public class NodesCollection
        : FreezableCollection<SiteMapNode>
    {

    }
}
