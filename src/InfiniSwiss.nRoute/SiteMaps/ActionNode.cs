using System;
using System.Windows.Markup;
using System.Xml.Serialization;

namespace nRoute.SiteMaps
{
    [Serializable]
    [XmlRoot(Namespace = SiteMaps.XMLNAMESPACE)]
    [ContentProperty("ChildNodes")]
    public class ActionNode
         : SiteMapNode
    {
        public event EventHandler Executed;

        #region Overrides

        public override void Execute()
        {
            if (Executed != null) Executed(this, EventArgs.Empty);
        }

        #endregion

    }
}
