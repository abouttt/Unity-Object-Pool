using System.Collections.Generic;
using UnityEngine;

public class PoolManager
{
    #region Pool
    private class Pool
    {
        public GameObject Prefab { get; private set; }
        public Transform Root { get; private set; }
        public int Size { get; private set; }
        public int Max { get; private set; }

        private readonly HashSet<GameObject> _activePool = new();
        private readonly List<GameObject> _deactivePool = new();

        public Pool(GameObject prefab, int size, int max)
        {
            Prefab = prefab;
            Root = new GameObject($"{Prefab.name}_Root").transform;
            Max = max;

            for (int i = 0; i < size; i++)
            {
                var go = Create();
                if (go == null)
                {
                    break;
                }

                PushToDeactiveContainer(go);
            }
        }

        public bool Push(GameObject go)
        {
            if (!_activePool.Remove(go))
            {
                return false;
            }

            PushToDeactiveContainer(go);
            return true;
        }

        public GameObject Pop(Transform parent)
        {
            GameObject go;

            int lastIndex = _deactivePool.Count - 1;
            if (lastIndex >= 0)
            {
                go = _deactivePool[lastIndex];
                _deactivePool.RemoveAt(lastIndex);
            }
            else
            {
                go = Create();
                if (go == null)
                {
                    return null;
                }
            }

            go.SetActive(true);
            go.transform.SetParent(parent == null ? Root : parent);
            _activePool.Add(go);
            return go;
        }

        public void Clear()
        {
            foreach (var go in _activePool)
            {
                Object.Destroy(go);
            }

            foreach (var go in _deactivePool)
            {
                Object.Destroy(go);
            }

            Object.Destroy(Root.gameObject);

            Prefab = null;
            Root = null;

            _activePool.Clear();
            _deactivePool.Clear();
        }

        private GameObject Create()
        {
            if (Size == Max)
            {
                return null;
            }

            Size++;
            var go = Object.Instantiate(Prefab);
            go.name = Prefab.name;
            return go;
        }

        private void PushToDeactiveContainer(GameObject go)
        {
            go.transform.SetParent(Root);
            go.SetActive(false);
            _deactivePool.Add(go);
        }
    }
    #endregion

    #region Singleton
    public static PoolManager Instance
    {
        get
        {
            s_instance ??= new();

            if (s_instance._root == null)
            {
                var root = GameObject.Find("Pool_Root");
                s_instance._root = root == null ? new GameObject("Pool_Root").transform : root.transform;
            }

            return s_instance;
        }
    }
    #endregion

    /// <summary>
    /// If a pool does not exist when the Pop method is called, a pool is automatically created.
    /// </summary>
    public static bool AutoCreate { get; set; }

    private static PoolManager s_instance;
    private readonly Dictionary<string, Pool> _pools = new();
    private Transform _root;

    public static void CreatePool(GameObject prefab, int size = 1, int max = -1)
    {
        if (prefab == null)
        {
            Debug.Log($"[PoolManager/CreatePool] Prefab is null.");
            return;
        }

        if (Instance._pools.ContainsKey(prefab.name))
        {
            Debug.Log($"[PoolManager/CreatePool] {prefab.name} pool already exist.");
            return;
        }

        var pool = new Pool(prefab, size, max);
        pool.Root.SetParent(Instance._root);
        Instance._pools.Add(prefab.name, pool);
    }

    public static bool Push(GameObject go)
    {
        if (go == null)
        {
            Debug.Log($"[PoolManager/Push] GameObject is null.");
            return false;
        }

        if (!Instance._pools.ContainsKey(go.name))
        {
            Debug.Log($"[PoolManager/Push] {go.name} pool no exist.");
            return false;
        }

        Instance._pools[go.name].Push(go);
        return true;
    }

    public static GameObject Pop(GameObject prefab, Transform parent = null)
    {
        if (!Instance._pools.TryGetValue(prefab.name, out var pool))
        {
            if (AutoCreate)
            {
                CreatePool(prefab);
                pool = Instance._pools[prefab.name];
            }
            else
            {
                Debug.Log($"[PoolManager/Pop] {prefab.name} pool no exist.");
                return null;
            }
        }

        return pool.Pop(parent);
    }

    public static void ClearPool(string name)
    {
        if (Instance._pools.TryGetValue(name, out var pool))
        {
            pool.Clear();
            Instance._pools.Remove(name);
        }
        else
        {
            Debug.Log($"[PoolManager/ClearPool] {name} pool no exist.");
        }
    }

    public static void ClearAll()
    {
        foreach (var kvp in Instance._pools)
        {
            kvp.Value.Clear();
        }

        Instance._pools.Clear();
    }
}
