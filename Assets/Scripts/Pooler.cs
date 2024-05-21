using UnityEngine;

namespace abouttt.ObjectPool
{
    public class Pooler : MonoBehaviour
    {
        [field: SerializeField]
        public GameObject Prefab { get; private set; }
        [SerializeField]
        private int _size;
        [SerializeField]
        private int _max = -1;

        private void Start()
        {
            PoolManager.CreatePool(Prefab, _size, _max);
        }
    }
}
