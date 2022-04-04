using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class InitialForce2D : BasicMonoBehavior
{
    public const bool DestroySelf = true;

    [Header("Force")]
    public ForceMode2D Mode = ForceMode2D.Impulse;
    public float Force;
    [Range(0.0f, 360.0f)]
    public float Angle = 0.0f;

    [Header("Randomization")]
    [Range(0.0f, 180.0f)]
    public float AngleRange = 0.0f;
    [Min(0.0f)]
    public float Radius = 0.0f;

    private Vector2 ForceVector => Quaternion.AngleAxis(Angle, Vector3.forward) * Vector2.right * Force;
    private bool HasAngle => (Mathf.Abs(AngleRange) > float.Epsilon);
    private bool HasRadius => (Mathf.Abs(Radius) > float.Epsilon);

    private void Start()
    {
        var force = ForceVector;
        if (HasAngle)
        {
            var angle = Random.Range(-AngleRange, AngleRange);
            var quat = Quaternion.AngleAxis(angle, Vector3.forward);
            force = quat * force;
        }

        var relativePoint = Vector2.zero;
        if (HasRadius)
        {
            relativePoint = Random.insideUnitCircle * Radius;
        }

        var rigidbody = GetComponent<Rigidbody2D>();
        if (rigidbody != null)
        {
            rigidbody.AddForceAtPosition(force, (Vector2)transform.position + relativePoint, Mode);
        }

        if (DestroySelf)
        {
            Destroy(this);
        }
    }

    private void OnDrawGizmosSelected()
    {
        var force = ForceVector;
        var quatFrom = Quaternion.AngleAxis(-AngleRange, Vector3.forward);
        var quatTo = Quaternion.AngleAxis(AngleRange, Vector3.forward);
        var pointFrom = quatFrom * force;

        //Outlines
        Handles.color = new Color(0, 0, 1.0f, 1.0f);
        if (HasAngle)
        {
            Handles.DrawWireArc(transform.position, Vector3.forward, pointFrom, AngleRange * 2.0f, Force);
            Handles.DrawLine(transform.position, transform.position + quatFrom * force);
            Handles.DrawLine(transform.position, transform.position + quatTo * force);
        }
        else
        {
            Handles.DrawLine(transform.position, transform.position + (Vector3)force);
        }
        if (HasRadius)
        {
            Handles.DrawWireArc(transform.position, Vector3.forward, Vector3.right * Radius, 360.0f, Radius);
        }

        //Infill
        Handles.color = new Color(0, 0, 1.0f, 0.1f);
        if (HasAngle)
        {
            Handles.DrawSolidArc(transform.position, Vector3.forward, pointFrom, AngleRange * 2.0f, Force);
            Handles.DrawLine(transform.position, transform.position + quatFrom * force);
            Handles.DrawLine(transform.position, transform.position + quatTo * force);
        }
        if (HasRadius)
        {
            Handles.DrawSolidArc(transform.position, Vector3.forward, Vector3.right * Radius, 360.0f, Radius);
        }
    }
}
