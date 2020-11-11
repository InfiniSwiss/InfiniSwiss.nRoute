using Microsoft.Xaml.Behaviors;
using nRoute.Components.Composition;
using nRoute.ViewServices;
using System;
using System.ComponentModel;
using System.Windows;

namespace nRoute.Behaviors
{
    public class ViewServiceBehaviorBase<T, TViewService>
        : Behavior<T>
        where
            T : DependencyObject
    {
        private const string VIEWSERIVCE_NAME_CHANGED = "A registered ViewService's name cannot be changed";

        [Category("Common Properties")]
        public static readonly DependencyProperty ViewServiceNameProperty =
            DependencyProperty.Register("ViewServiceName", typeof(string), typeof(ViewServiceBehaviorBase<T, TViewService>),
            new PropertyMetadata(null, OnViewServiceNameChanged));

        [Category("Common Properties")]
        public static readonly DependencyProperty IsDefaultProperty =
            DependencyProperty.Register("IsDefault", typeof(bool), typeof(ViewServiceBehaviorBase<T, TViewService>),
            new PropertyMetadata(false));

        #region Properties

        [Category("Common Properties")]
        public string ViewServiceName
        {
            get { return Convert.ToString(GetValue(ViewServiceNameProperty)); }
            set
            {
                SetValue(ViewServiceNameProperty, value);
            }
        }

        [Category("Common Properties")]
        public bool IsDefault
        {
            get { return Convert.ToBoolean(GetValue(IsDefaultProperty)); }
            set { SetValue(IsDefaultProperty, value); }
        }

        #endregion

        #region Overrides

        protected override void OnAttached()
        {
            base.OnAttached();

            // we need to have a name, else we can't register it
            if (string.IsNullOrEmpty(ViewServiceName)) this.ViewServiceName = Guid.NewGuid().ToString();
            if (this.AssociatedObject != null)
            {
                if (!Resource.IsResourceRegistered(typeof(TViewService), this.ViewServiceName))
                {
                    var _meta = new ViewServiceMeta(typeof(TViewService), this.AssociatedObject.GetType(),
                        this.ViewServiceName, InitializationMode.WhenAvailable, ViewServiceLifetime.SelfRegisteredInstance);
                    Resource.RegisterResourceLocator(typeof(TViewService), new DefaultViewServiceLocator(_meta), this.IsDefault);
                }
                ViewServiceLocator.RegisterViewServiceInstance(typeof(TViewService), this.ViewServiceName, this.AssociatedObject);
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (!string.IsNullOrEmpty(ViewServiceName) && this.AssociatedObject != null)
            {
                ViewServiceLocator.UnregisterViewServiceInstance(typeof(TViewService), this.ViewServiceName, this.AssociatedObject);
                Resource.UnregisterResourceLocator(typeof(TViewService), this.ViewServiceName);
            }
        }

        #endregion

        #region Handlers

        protected void UpdateViewServiceName(string oldName, string newName)
        {
            if (!string.IsNullOrEmpty(oldName))
                throw new InvalidOperationException(VIEWSERIVCE_NAME_CHANGED);
        }

        private static void OnViewServiceNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ViewServiceBehaviorBase<T, TViewService>)d).UpdateViewServiceName(Convert.ToString(e.OldValue), Convert.ToString(e.NewValue));
        }

        #endregion

    }
}
