using nRoute.Components.Handlers;
using nRoute.Internal;
using System;
using System.Linq.Expressions;

namespace nRoute.Components.Disposer
{
    public static class DisposerExtensions
    {

        #region Static Stuff

        private static readonly Action<IDisposable> _disposableDisposer;

        static DisposerExtensions()
        {
            Expression<Action<IDisposable>> _iDisposableExpr = (d) => d.Dispose();
            _disposableDisposer = _iDisposableExpr.Compile();
        }

        #endregion

        public static T DisposeWith<T>(this T resource, IDisposeRelated withResource)
            where
                T : IDisposable
        {
            Guard.ArgumentNotDefault(resource, "resource");

            var _resource = new WeakReference(resource); // this outside to ensure we don't lift the handler itself
            withResource.DisposeRelated += new Handler<EventArgs, EventHandler>((s, e) =>
            {
                DisposerExtensions.Dispose<IDisposable>(_resource, _disposableDisposer);
                _resource = null;
            }, (h) => withResource.DisposeRelated -= h).HandleOnce();
            return resource;
        }

        public static T DisposeWith<T>(this T resource, IDisposeRelated withResource, Action<T> disposeAction)
        {
            Guard.ArgumentNotDefault(resource, "resource");

            var _resource = new WeakReference(resource);
            withResource.DisposeRelated += new Handler<EventArgs, EventHandler>((s, e) =>
            {
                Dispose<T>(_resource, disposeAction);
                _resource = null;
                disposeAction = null;
            }, (h) => withResource.DisposeRelated -= h).HandleOnce();
            return resource;
        }

        #region Helpers

        private static void Dispose<T>(WeakReference resource, Action<T> disposeAction)
        {
            if (resource != null && resource.IsAlive)
            {
                var _target = resource.Target;
                if (_target != null) disposeAction((T)_target);         // we convert as late as possible
            }
            if (resource != null) resource.Target = null;
        }

        #endregion

    }

}
