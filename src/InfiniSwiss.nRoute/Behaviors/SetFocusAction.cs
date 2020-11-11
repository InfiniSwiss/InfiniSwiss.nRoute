using Microsoft.Xaml.Behaviors;
using System.Windows;

namespace nRoute.Behaviors
{
    public class SetFocusAction
        : TriggerAction<UIElement>
    {
        protected override void Invoke(object parameter)
        {
            if (AssociatedObject != null) AssociatedObject.Focus();
        }
    }
}
