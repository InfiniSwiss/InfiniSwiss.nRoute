using nRoute.Internal;

namespace nRoute.Components
{
    public static class CollectionExtensions
    {
        public static bool TryGetValue<T>(this ParametersCollection collection, string key, out T value)
        {
            Guard.ArgumentNotNull(key, "key");

            if (collection != null && collection.ContainsKey(key))
            {
                value = (T)collection[key];
                return true;
            }
            else
            {
                value = default(T);
                return false;
            }
        }

        public static object GetValueOrDefault(this ParametersCollection collection, string key, object defaultValue)
        {
            Guard.ArgumentNotNull(key, "key");

            return (collection != null && collection.ContainsKey(key)) ? collection[key] : defaultValue;
        }

        public static T GetValueOrDefault<T>(this ParametersCollection collection, string key, T defaultValue)
        {
            Guard.ArgumentNotNull(key, "key");

            return (collection != null && collection.ContainsKey(key)) ? (T)collection[key] : defaultValue;
        }


        public static bool TryGetValue<T>(this DependencyParameterCollection collection, string key, out T value)
        {
            Guard.ArgumentNotNull(key, "key");

            if (collection != null && collection.ContainsKey(key))
            {
                value = (T)collection[key];
                return true;
            }
            else
            {
                value = default(T);
                return false;
            }
        }

        public static object GetValueOrDefault(this DependencyParameterCollection collection, string key, object defaultValue)
        {
            Guard.ArgumentNotNull(key, "key");

            return (collection != null && collection.ContainsKey(key)) ? collection[key] : defaultValue;
        }

        public static T GetValueOrDefault<T>(this DependencyParameterCollection collection, string key, T defaultValue)
        {
            Guard.ArgumentNotNull(key, "key");

            return (collection != null && collection.ContainsKey(key)) ? (T)collection[key] : defaultValue;
        }

    }
}