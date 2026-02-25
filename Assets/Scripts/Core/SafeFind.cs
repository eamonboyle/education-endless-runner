using UnityEngine;

namespace MathRunner.Core
{
    /// <summary>
    /// Null-safe wrappers around <see cref="GameObject.Find"/> and related methods.
    /// Logs a warning instead of allowing a <see cref="System.NullReferenceException"/>
    /// to propagate through calling code.
    /// </summary>
    public static class SafeFind
    {
        /// <summary>
        /// Finds a <see cref="GameObject"/> by <paramref name="name"/>.
        /// Returns <c>null</c> and logs a warning if not found.
        /// </summary>
        /// <param name="name">The name of the GameObject to find.</param>
        /// <returns>The found GameObject, or <c>null</c>.</returns>
        public static GameObject Find(string name)
        {
            GameObject obj = GameObject.Find(name);
            if (obj == null)
            {
                Debug.LogWarning($"[SafeFind] GameObject.Find(\"{name}\") returned null.");
            }
            return obj;
        }

        /// <summary>
        /// Finds a <see cref="GameObject"/> by <paramref name="tag"/>.
        /// Returns <c>null</c> and logs a warning if not found, instead of
        /// throwing an exception.
        /// </summary>
        /// <param name="tag">The tag to search for.</param>
        /// <returns>The found GameObject, or <c>null</c>.</returns>
        public static GameObject FindWithTag(string tag)
        {
            GameObject obj = null;
            try
            {
                obj = GameObject.FindWithTag(tag);
            }
            catch (UnityException)
            {
                Debug.LogWarning($"[SafeFind] Tag \"{tag}\" is not defined in the Tag Manager.");
                return null;
            }

            if (obj == null)
            {
                Debug.LogWarning($"[SafeFind] GameObject.FindWithTag(\"{tag}\") returned null.");
            }
            return obj;
        }

        /// <summary>
        /// Gets a component of type <typeparamref name="T"/> from
        /// <paramref name="obj"/>. Returns <c>null</c> and logs a warning
        /// if the component is not present or <paramref name="obj"/> is null.
        /// </summary>
        /// <typeparam name="T">Component type to retrieve.</typeparam>
        /// <param name="obj">The source GameObject.</param>
        /// <returns>The component, or <c>null</c>.</returns>
        public static T SafeGetComponent<T>(GameObject obj) where T : Component
        {
            if (obj == null)
            {
                Debug.LogWarning($"[SafeFind] SafeGetComponent<{typeof(T).Name}>() called on a null GameObject.");
                return null;
            }

            T component = obj.GetComponent<T>();
            if (component == null)
            {
                Debug.LogWarning($"[SafeFind] GetComponent<{typeof(T).Name}>() on \"{obj.name}\" returned null.");
            }
            return component;
        }
    }
}
