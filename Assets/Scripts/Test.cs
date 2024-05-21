using UnityEngine;
using abouttt.ObjectPool;

public class Test : MonoBehaviour
{
    public GameObject Prefab;
    public Transform Root;

    private Pooler _pooler;

    private void Awake()
    {
        _pooler = GetComponent<Pooler>();
    }

    private void Start()
    {
        PoolManager.CreatePool(Prefab);
        PoolManager.AutoCreate = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            var go = PoolManager.Pop(Prefab);
            if (go != null)
            {
                go.transform.position = new Vector3(Random.Range(0, 5), Random.Range(0, 5), Random.Range(0, 5));
                go.transform.SetParent(Root);
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            var go = PoolManager.Pop(_pooler.Prefab);
            if (go != null)
            {
                go.transform.position = new Vector3(Random.Range(0, 5), Random.Range(0, 5), Random.Range(0, 5));
            }
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            PoolManager.ClearAll();
        }
    }
}
