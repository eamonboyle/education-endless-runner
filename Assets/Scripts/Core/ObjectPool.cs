using System.Collections.Generic;
using UnityEngine;

namespace MathRunner.Core
{
    /// <summary>
    /// Generic MonoBehaviour-based object pool.
    /// Primarily designed for question boxes but works with any prefab.
    /// Attach to a manager GameObject and assign the prefab in the inspector.
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {
        [Tooltip("Prefab to pool.")]
        [SerializeField] private GameObject prefab;

        [Tooltip("Optional parent transform for pooled objects. Defaults to this transform.")]
        [SerializeField] private Transform poolParent;

        [Tooltip("Number of objects to pre-instantiate on Awake.")]
        [SerializeField] private int initialSize = 0;

        private readonly Queue<GameObject> available = new Queue<GameObject>();

        private void Awake()
        {
            if (poolParent == null)
            {
                poolParent = transform;
            }

            if (initialSize > 0)
            {
                Preload(initialSize);
            }
        }

        /// <summary>
        /// Pre-instantiates <paramref name="count"/> inactive objects and adds them
        /// to the pool. Safe to call multiple times.
        /// </summary>
        /// <param name="count">Number of objects to create.</param>
        public void Preload(int count)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject obj = CreateNewInstance();
                obj.SetActive(false);
                available.Enqueue(obj);
            }
        }

        /// <summary>
        /// Returns an inactive pooled object, or instantiates a new one if the
        /// pool is empty. The returned object is <b>not</b> activated automatically;
        /// the caller should set its position/rotation and call
        /// <c>SetActive(true)</c>.
        /// </summary>
        /// <returns>An inactive <see cref="GameObject"/> from the pool.</returns>
        public GameObject Get()
        {
            while (available.Count > 0)
            {
                GameObject obj = available.Dequeue();
                if (obj != null)
                {
                    return obj;
                }
            }

            return CreateNewInstance();
        }

        /// <summary>
        /// Returns an object from the pool, positioned and rotated as specified,
        /// and already activated.
        /// </summary>
        /// <param name="position">World position.</param>
        /// <param name="rotation">World rotation.</param>
        /// <returns>An active <see cref="GameObject"/>.</returns>
        public GameObject Get(Vector3 position, Quaternion rotation)
        {
            GameObject obj = Get();
            obj.transform.SetPositionAndRotation(position, rotation);
            obj.SetActive(true);
            return obj;
        }

        /// <summary>
        /// Deactivates the object and returns it to the pool for reuse.
        /// </summary>
        /// <param name="obj">The object to return. Must have been obtained via <see cref="Get()"/>.</param>
        public void Return(GameObject obj)
        {
            if (obj == null) return;

            obj.SetActive(false);
            obj.transform.SetParent(poolParent);
            available.Enqueue(obj);
        }

        /// <summary>
        /// Returns the number of inactive objects currently available in the pool.
        /// </summary>
        public int CountAvailable => available.Count;

        private GameObject CreateNewInstance()
        {
            GameObject obj = Instantiate(prefab, poolParent);
            obj.SetActive(false);
            return obj;
        }
    }
}
