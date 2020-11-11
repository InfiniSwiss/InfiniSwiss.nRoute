using Microsoft.Xaml.Behaviors;
using nRoute.ApplicationServices;
using nRoute.Services;
using nRoute.ViewModels;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace nRoute.Behaviors
{
    public class BridgeViewModelBehavior
        : Behavior<FrameworkElement>
    {

        #region Declarations

        private const string VIEW_NOT_RESOLVABLE = "Cannot resolve ViewModel for View of type {0}.";
        private const string APPSERVICE_NOT_INITIALIZED = "nRouteApplicationService has not been initialized.";

        public static readonly DependencyProperty LoadedCommandProperty =
            DependencyProperty.Register("LoadedCommand", typeof(ICommand), typeof(BridgeViewModelBehavior),
            new PropertyMetadata(null));

        public static readonly DependencyProperty LoadedCommandParameterProperty =
            DependencyProperty.Register("LoadedCommandParameter", typeof(object), typeof(BridgeViewModelBehavior),
            new PropertyMetadata(null));

        public static readonly DependencyProperty UnloadedCommandProperty =
            DependencyProperty.Register("UnloadedCommand", typeof(ICommand), typeof(BridgeViewModelBehavior),
            new PropertyMetadata(null));

        public static readonly DependencyProperty UnloadedCommandParameterProperty =
            DependencyProperty.Register("UnloadedCommandParameter", typeof(object), typeof(BridgeViewModelBehavior),
            new PropertyMetadata(null));

        private bool _isLoaded;

        #endregion

        #region Properties

        [Category("Common Properties")]
        [CustomPropertyValueEditor(CustomPropertyValueEditor.PropertyBinding)]
        public ICommand LoadedCommand
        {
            get { return (ICommand)GetValue(LoadedCommandProperty); }
            set { SetValue(LoadedCommandProperty, value); }
        }

        [Category("Common Properties")]
        [CustomPropertyValueEditor(CustomPropertyValueEditor.PropertyBinding)]
        public object LoadedCommandParameter
        {
            get { return GetValue(LoadedCommandParameterProperty); }
            set { SetValue(LoadedCommandParameterProperty, value); }
        }

        [Category("Common Properties")]
        [CustomPropertyValueEditor(CustomPropertyValueEditor.PropertyBinding)]
        public ICommand UnloadedCommand
        {
            get { return (ICommand)GetValue(UnloadedCommandProperty); }
            set { SetValue(UnloadedCommandProperty, value); }
        }

        [Category("Common Properties")]
        [CustomPropertyValueEditor(CustomPropertyValueEditor.PropertyBinding)]
        public object UnloadedCommandParameter
        {
            get { return GetValue(UnloadedCommandParameterProperty); }
            set { SetValue(UnloadedCommandParameterProperty, value); }
        }

        #endregion

        #region Overrides

        protected override void OnAttached()
        {
            base.OnAttached();
            var _type = AssociatedObject.GetType();

            // we check if we can
            if (!nRouteApplicationService.IsInitialized)
                throw new InvalidOperationException(APPSERVICE_NOT_INITIALIZED);

            // we get and inject that into the data context
            var _viewModelMapping = ServiceLocator.GetService(typeof(IViewModelProvider), _type.FullName) as IViewModelProvider;
            if (_viewModelMapping == null) throw new InvalidOperationException(string.Format(VIEW_NOT_RESOLVABLE, _type.FullName));
            // we set the view mode
            AssociatedObject.DataContext = _viewModelMapping.CreateViewModelInstance();

            AssociatedObject.Loaded += new RoutedEventHandler(AssociatedObject_LoadedAsync);
            AssociatedObject.Unloaded += new RoutedEventHandler(AssociatedObject_Unloaded);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
        }

        #endregion

        #region Handlers


        private async void AssociatedObject_LoadedAsync(object sender, RoutedEventArgs e)
        {
            // we don't unlink on the first call because the loaded event could be called multiple times
            AssociatedObject.Loaded -= new RoutedEventHandler(AssociatedObject_LoadedAsync);

            if (this.LoadedCommand != null && this.LoadedCommand.CanExecute(null))
            {
                this.LoadedCommand.Execute(null);
            }

            if (this.AssociatedObject != null)
            {
                var _viewSupporter = this.AssociatedObject.DataContext as ISupportView;
                if (_viewSupporter != null)
                {
                    _viewSupporter.Loaded();
                    await _viewSupporter.LoadedAsync();
                }
            }
        }


        private void AssociatedObject_Unloaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.Unloaded -= new RoutedEventHandler(AssociatedObject_Unloaded);

            if (this.UnloadedCommand != null && this.UnloadedCommand.CanExecute(this.UnloadedCommandParameter))
            {
                this.UnloadedCommand.Execute(this.UnloadedCommandParameter);
            }

            if (this.AssociatedObject != null)
            {
                var _viewSupporter = this.AssociatedObject.DataContext as ISupportView;
                if (_viewSupporter != null) _viewSupporter.Unloaded();
            }
        }

        protected override async void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (_isLoaded || this.AssociatedObject == null) return;

            // we use the property changed thing because the bindings to the attached behaviors are not resolved until after
            // the events (loaded/initialized) have passed.. and this is also the reason we have removed initialized command thing

            if (e.Property == LoadedCommandProperty && this.LoadedCommand != null)
            {
                if (this.LoadedCommandParameter != null || !DependencyPropertyHelper.GetValueSource(this, LoadedCommandParameterProperty).IsExpression)
                {
                    _isLoaded = true;
                    if (this.LoadedCommand.CanExecute(this.LoadedCommandParameter))
                    {
                        this.LoadedCommand.Execute(this.LoadedCommandParameter);
                    }

                    if (this.AssociatedObject != null)
                    {
                        var _viewSupporter = this.AssociatedObject.DataContext as ISupportView;
                        if (_viewSupporter != null)
                        {
                            _viewSupporter.Loaded();
                            await _viewSupporter.LoadedAsync();
                        }
                    }
                }
            }
            else if (e.Property == LoadedCommandParameterProperty && this.LoadedCommandParameter != null)
            {
                if (this.LoadedCommand != null)
                {
                    _isLoaded = true;
                    if (this.LoadedCommand.CanExecute(this.LoadedCommandParameter))
                    {
                        this.LoadedCommand.Execute(this.LoadedCommandParameter);
                    }

                    if (this.AssociatedObject != null)
                    {
                        var _viewSupporter = this.AssociatedObject.DataContext as ISupportView;
                        if (_viewSupporter != null)
                        {
                            _viewSupporter.Loaded();
                            await _viewSupporter.LoadedAsync();
                        }
                    }
                }
            }
            else if (this.LoadedCommand == null && !DependencyPropertyHelper.GetValueSource(this, LoadedCommandProperty).IsExpression)
            {
                _isLoaded = true;
                if (this.AssociatedObject != null)
                {
                    var _viewSupporter = this.AssociatedObject.DataContext as ISupportView;
                    if (_viewSupporter != null)
                    {
                        _viewSupporter.Loaded();
                        await _viewSupporter.LoadedAsync();
                    }
                }
            }

            //if (e.Property == InitializedCommandProperty && this.InitializedCommand != null)
            //{
            //    if (this.InitializedCommand != null || !DependencyPropertyHelper.GetValueSource(this, InitializedCommandParameterProperty).IsExpression)
            //    {
            //        if (this.InitializedCommand.CanExecute(this.InitializedCommandParameter))
            //            this.InitializedCommand.Execute(this.InitializedCommandParameter);
            //    }
            //}
            //else if (e.Property == InitializedCommandParameterProperty && this.InitializedCommandParameter != null)
            //{
            //    if (this.InitializedCommand != null)
            //    {
            //        if (this.InitializedCommand.CanExecute(this.InitializedCommandParameter))
            //            this.InitializedCommand.Execute(this.InitializedCommandParameter);
            //    }
            //}
        }


        #endregion

    }
}