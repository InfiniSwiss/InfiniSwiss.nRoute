using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;

namespace nRoute.SiteMaps
{
    public static class SiteMaps
    {
        public const string XMLNAMESPACE = "http://nRoute/schemas/2010/sitemaps/";

        #region Dependency Properties

        public static readonly DependencyProperty PrimaryProperty = DependencyProperty.RegisterAttached(
            "Primary", typeof(NodesCollection), typeof(SiteMaps), new PropertyMetadata(null));

        public static readonly DependencyProperty SecondaryProperty = DependencyProperty.RegisterAttached(
            "Secondary", typeof(NodesCollection), typeof(SiteMaps), new PropertyMetadata(null));

        public static readonly DependencyProperty IsPrimaryEnabledProperty = DependencyProperty.RegisterAttached(
            "IsPrimaryEnabled", typeof(bool), typeof(SiteMaps), new PropertyMetadata(true));

        public static readonly DependencyProperty IsSecondaryEnabledProperty = DependencyProperty.RegisterAttached(
            "IsSecondaryEnabled", typeof(bool), typeof(SiteMaps), new PropertyMetadata(true));

        public static readonly DependencyProperty IsPrimaryVisibleProperty = DependencyProperty.RegisterAttached(
            "IsPrimaryVisible", typeof(bool), typeof(SiteMaps), new PropertyMetadata(true));

        public static readonly DependencyProperty IsSecondaryVisibleProperty = DependencyProperty.RegisterAttached(
            "IsSecondaryVisible", typeof(bool), typeof(SiteMaps), new PropertyMetadata(true));

        #endregion

        #region Getters and Setters

        public static void SetPrimary(DependencyObject dependencyObject, NodesCollection nodes)
        {
            if (nodes != null) ((IAttachedObject)nodes).Attach(dependencyObject);
            dependencyObject.SetValue(PrimaryProperty, nodes);
        }

        public static NodesCollection GetPrimary(DependencyObject dependencyObject)
        {
            if (dependencyObject == null) return null;
            return dependencyObject.GetValue(PrimaryProperty) as NodesCollection;
        }

        public static void SetSecondary(DependencyObject dependencyObject, NodesCollection nodes)
        {
            if (nodes != null) ((IAttachedObject)nodes).Attach(dependencyObject);
            dependencyObject.SetValue(SecondaryProperty, nodes);
        }

        public static NodesCollection GetSecondary(DependencyObject dependencyObject)
        {
            if (dependencyObject == null) return null;
            return dependencyObject.GetValue(SecondaryProperty) as NodesCollection;
        }

        // Is Enabled Related

        public static void SetIsPrimaryEnabled(DependencyObject dependencyObject, bool isEnabled)
        {
            dependencyObject.SetValue(IsPrimaryEnabledProperty, isEnabled);
        }

        public static bool GetIsPrimaryEnabled(DependencyObject dependencyObject)
        {
            if (dependencyObject == null) return false;
            return Convert.ToBoolean(dependencyObject.GetValue(IsPrimaryEnabledProperty));
        }

        public static void SetIsSecondaryEnabled(DependencyObject dependencyObject, bool nodes)
        {
            dependencyObject.SetValue(IsSecondaryEnabledProperty, nodes);
        }

        public static bool GetIsSecondaryEnabled(DependencyObject dependencyObject)
        {
            if (dependencyObject == null) return false;
            return Convert.ToBoolean(dependencyObject.GetValue(IsSecondaryEnabledProperty));
        }

        #endregion

    }
}