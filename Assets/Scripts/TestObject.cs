using System.Collections;
using UnityEngine;

public class TestObject : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(PushToPool());
    }

    private IEnumerator PushToPool()
    {
        yield return new WaitForSeconds(1f);
        PoolManager.Push(gameObject);
    }
}
