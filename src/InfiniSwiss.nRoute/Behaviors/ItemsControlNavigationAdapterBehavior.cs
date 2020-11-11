using Microsoft.Xaml.Behaviors;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace nRoute.Behaviors
{
    public class ItemsControlNavigationAdapterBehavior
        : NavigationAdapterBehaviorBase<ItemsControl>
    {
        public static readonly DependencyProperty ItemsSourceProperty =
           DependencyProperty.Register("ItemsSource", typeof(IList), typeof(ItemsControlNavigationAdapterBehavior),
           new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged)));

        #region Properties

        [Category("Common Properties")]
        [CustomPropertyValueEditor(CustomPropertyValueEditor.PropertyBinding)]
        public IList ItemsSource
        {
            get { return (IList)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        #endregion

        #region Overrides

        public override object Content
        {
            get
            {
                // gets the immediately last item
                if (AssociatedObject == null || AssociatedObject.Items.Count == 0) return null;
                return AssociatedObject.Items[this.AssociatedObject.Items.Count - 1];
            }
        }

        protected override void OnSetNavigationContent(object content)
        {
            if (AssociatedObject == null) return;
            if (this.ItemsSource == null) this.ItemsSource = new ObservableCollection<Object>();

            this.ItemsSource.Add(content);
        }

        #endregion

        #region Handlers

        protected virtual void UpdateItemsSource(IList itemsSource)
        {
            if (AssociatedObject == null) return;
            if (AssociatedObject.ItemsSource != itemsSource)
            {
                AssociatedObject.ItemsSource = itemsSource;
            }
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ItemsControlNavigationAdapterBehavior)d).UpdateItemsSource(e.NewValue as IList);
        }

        #endregion

    }
}