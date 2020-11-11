using Microsoft.Xaml.Behaviors;
using System.Windows;

namespace nRoute.Behaviors
{
    [TypeConstraintAttribute(typeof(DependencyObject))]
    public class TargetedSetFocusAction
        : TargetedTriggerAction<UIElement>
    {
        protected override void Invoke(object parameter)
        {
            if (Target != null) Target.Focus();
        }
    }
}
