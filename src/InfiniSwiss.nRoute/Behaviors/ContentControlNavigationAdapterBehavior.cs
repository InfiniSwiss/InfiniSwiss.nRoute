using System;
using System.Windows.Controls;

namespace nRoute.Behaviors
{
    public class ContentControlNavigationAdapterBehavior
        : NavigationAdapterBehaviorBase<ContentControl>
    {
        private Object _content;

        public override object Content
        {
            get { return this.AssociatedObject != null ? this.AssociatedObject.Content : _content; }
        }

        #region Overrides

        protected override void OnSetNavigationContent(object content)
        {
            if (this.AssociatedObject != null)
            {
                this.AssociatedObject.Content = content;
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
                this.AssociatedObject.Content = _content;
                _content = null;
            }
        }

        #endregion

    }
}
