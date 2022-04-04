using UnityEngine;

public class KillZone : BasicMonoBehavior
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Kill(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Kill(other.gameObject);
    }

    private void Kill(GameObject gameObject)
    {
        //...
    }
}
