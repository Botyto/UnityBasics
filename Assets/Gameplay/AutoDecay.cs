using UnityEngine;

public class AutoDecay : BasicMonoBehavior
{
    [Min(0.0f)]
    public float AfterTime = 0.0f;

    private void Start()
    {
        if (AfterTime < float.Epsilon)
        {
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject, AfterTime);
        }
    }
}
