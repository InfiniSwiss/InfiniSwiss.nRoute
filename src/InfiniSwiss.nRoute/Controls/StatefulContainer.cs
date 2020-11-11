using System.Windows.Markup;

namespace nRoute.Controls
{
    [ContentProperty]
    public partial class StatefulContainer
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
