using System.Collections;
using UnityEngine;

public class SpawnPoint : BasicMonoBehavior
{
    public const bool DestroySelf = true;
    public GameObject Prefab;
    [Min(0.0f)]
    public float Delay = 0.0f;
    [Min(1)]
    public int Count = 1;

    private void Start()
    {
        if (Prefab != null)
        {
            if (Delay < float.Epsilon)
            {
                Spawn(Prefab, Count);
                if (DestroySelf)
                {
                    DoDestroySelf();
                }
            }
            else
            {
                StartCoroutine(InstantiateRoutine(Delay, Count, Prefab));
            }
        }
        else if (DestroySelf)
        {
            DoDestroySelf();
        }
    }

    private IEnumerator InstantiateRoutine(float delay, int count, GameObject prefab)
    {
        yield return new WaitForSeconds(delay);
        Spawn(prefab, count);
        if (DestroySelf)
        {
            DoDestroySelf();
        }
    }

    private void Spawn(GameObject prefab, int count)
    {
        for (var i = 0; i < count; ++i)
        {
            var newObject = Instantiate(prefab, transform.position, transform.rotation);
            SendMessage("PrefabSpawned", newObject, SendMessageOptions.DontRequireReceiver);
        }
    }

    private void DoDestroySelf()
    {
        if (gameObject.GetComponents<MonoBehaviour>().Length == 2)
        {
            Destroy(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }
}
