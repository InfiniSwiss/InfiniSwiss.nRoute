using nRoute.Components;
using nRoute.Internal;
using System;
using System.Xml.Serialization;

namespace nRoute.SiteMaps
{
    [Serializable]
    [XmlRoot(ElementName = "AreaInfos", Namespace = SiteMaps.XMLNAMESPACE)]
    public class AreaInfosCollection
        : KeyedObservableCollection<string, SiteAreaInfo>
    {
        public AreaInfosCollection()
            : base(StringComparer.InvariantCultureIgnoreCase) { }

        #region Overrides

        protected override string GetKeyForItem(SiteAreaInfo item)
        {
            Guard.ArgumentNotNull(item, "item");
            return item.Key;
        }

        #endregion

    }
}
