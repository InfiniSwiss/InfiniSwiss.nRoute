using System;
using System.Windows;
using System.Windows.Controls;

namespace nRoute.Behaviors
{
    public class PanelNavigationAdapterBehavior
        : NavigationAdapterBehaviorBase<Panel>
    {
        private Object _content;

        public override object Content
        {
            get
            {
                if (this.AssociatedObject != null)
                {
                    if (this.AssociatedObject.Children.Count > 0)
                    {
                        return this.AssociatedObject.Children[this.AssociatedObject.Children.Count - 1];
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return _content;
                }
            }
        }

        #region Overrides

        protected override void OnSetNavigationContent(object content)
        {
            if (this.AssociatedObject != null)
            {
                this.AssociatedObject.Children.Add((UIElement)content);
            }
            else
            {
                _content = content;
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            if (_content != null && this.AssociatedObject != null)
            {
                this.AssociatedObject.Children.Add((UIElement)_content);
                _content = null;
            }
        }

        #endregion

    }
}
