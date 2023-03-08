using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    public UnityEvent OnEnter;
    public UnityEvent OnExit;

    private void OnTriggerEnter(Collider other)
    {
        OnEnter.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        OnExit.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnEnter.Invoke();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        OnExit.Invoke();
    }
}
