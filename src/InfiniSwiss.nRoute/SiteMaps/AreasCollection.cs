using nRoute.Components;
using nRoute.Internal;
using System;
using System.Xml.Serialization;

namespace nRoute.SiteMaps
{
    [Serializable]
    [XmlRoot(ElementName = "Areas", Namespace = SiteMaps.XMLNAMESPACE)]
    public class AreasCollection
         : KeyedObservableCollection<string, SiteArea>
    {
        public AreasCollection()
            : base(StringComparer.InvariantCultureIgnoreCase) { }

        #region Overrides

        protected override string GetKeyForItem(SiteArea item)
        {
            Guard.ArgumentNotNull(item, "item");
            return item.Key;
        }

        #endregion

    }
}
