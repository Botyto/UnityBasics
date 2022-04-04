using UnityEngine;

public class Collector : BasicMonoBehavior
{
    [ClassName(typeof(BasicMonoBehavior), false)]
    public string CollectableType;
    public bool DestroyCollectable;
    public int Count;

    public static int GetCount<T>(GameObject gameObject)
        where T : MonoBehaviour
    {
        var allCollectors = gameObject.GetComponents<Collector>();
        foreach (var collector in allCollectors)
        {
            if (collector.CollectableType == typeof(T).Name)
            {
                return collector.Count;
            }
        }
        return 0;
    }

    public static int GetCount<T>(MonoBehaviour component)
        where T : MonoBehaviour
    {
        var allCollectors = component.GetComponents<Collector>();
        foreach (var collector in allCollectors)
        {
            if (collector.CollectableType == typeof(T).Name)
            {
                return collector.Count;
            }
        }
        return 0;
    }

    private void Start()
    {
        Count = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryCollect(collision.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryCollect(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryCollect(other.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryCollect(other.gameObject);
    }

    private bool TryCollect(GameObject gameObject)
    {
        if (gameObject.GetComponent(CollectableType) != null)
        {
            SendMessage("OnCollect", gameObject, SendMessageOptions.DontRequireReceiver);
            ++Count;
            gameObject.SendMessage("OnCollected", this, SendMessageOptions.DontRequireReceiver);
            if (DestroyCollectable)
            {
                Destroy(gameObject);
            }
            return true;
        }
        return false;
    }
}
