using System.Windows.Markup;

namespace nRoute.Controls
{
    [ContentProperty]
    public partial class BrowsingContainer
         : NavigationContainer
    {
        protected override void InitializeComponent()
        {
            // call the base 
            base.InitializeComponent();

            // we use the same style
            this.DefaultStyleKey = typeof(NavigationContainer);
        }
    }
}
