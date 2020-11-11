using nRoute.Internal;
using System;
using System.Collections.Generic;

namespace nRoute.ViewServices
{
    internal static class ViewServicesInstancesManager
    {
        private readonly static Object _lock = new Object();
        private static readonly Dictionary<Type, WeakReference> _instances;

        static ViewServicesInstancesManager()
        {
            _instances = new Dictionary<Type, WeakReference>();
        }

        public static void RegisterInstance(Type viewServiceType, object serviceInstance)
        {
            Guard.ArgumentNotNull(viewServiceType, "viewServiceType");
            Guard.ArgumentNotNull(serviceInstance, "serviceInstance");

            lock (_lock)
            {
                if (_instances.ContainsKey(viewServiceType))
                {
                    var _instanceReference = _instances[viewServiceType];
                    _instanceReference.Target = null;
                    _instanceReference = null;

                    _instances[viewServiceType] = new WeakReference(serviceInstance);
                }
                else
                {
                    _instances.Add(viewServiceType, new WeakReference(serviceInstance));
                }
            }
        }

        public static void UnregisterInstance(Type viewServiceType, object serviceInstance)
        {
            Guard.ArgumentNotNull(viewServiceType, "viewServiceType");
            Guard.ArgumentNotNull(serviceInstance, "serviceInstance");

            lock (_lock)
            {
                var _instance = default(WeakReference);
                if (_instances.TryGetValue(viewServiceType, out _instance))
                {
                    if (_instance.IsAlive)
                    {
                        var _instanceObject = _instance.Target;
                        if (object.ReferenceEquals(serviceInstance, _instanceObject) || _instanceObject == null)
                        {
                            _instance = null;
                            _instances.Remove(viewServiceType);
                        }
                    }
                    else
                    {
                        _instances.Remove(viewServiceType);
                    }
                }
            }
        }

        public static Object GetRegisteredInstance(Type viewServiceType)
        {
            lock (_lock)
            {
                var _instance = default(WeakReference);
                if (_instances.TryGetValue(viewServiceType, out _instance))
                {
                    if (_instance.IsAlive)
                    {
                        var _instanceObject = _instance.Target;
                        if (_instanceObject == null)
                        {
                            _instances.Remove(viewServiceType);
                        }
                        return _instanceObject;
                    }
                }
                return null;
            }
        }
    }
}
