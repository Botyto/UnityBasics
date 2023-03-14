using System;
using UnityEditor;
using UnityEngine;

public class RotateCollectable2D : BasicMonoBehavior
{
    [Header("Bobbing up & down")]
    [Min(0.0f)]
    public float BobbingDistance = 1.0f;
    [Min(0.0f)]
    public float BobbingPeriod = 2.0f;
    [SerializeField]
    [HideInInspector]
    private float m_EnableTime;
    private const bool BobbingAroundStartingPoint = true;
    private static readonly Func<float, float> BobbingTrigFunc = BobbingAroundStartingPoint ? Mathf.Cos : Mathf.Sin;

    [Header("Rotation")]
    [Min(0.0f)]
    public float RotatePeriod = 2.0f;

    [FindInThis]
    [SerializeField]
    [HideInInspector]
    private Transform Transform;

    public bool BobbingEnabled => (BobbingDistance > float.Epsilon && BobbingPeriod > float.Epsilon);
    public bool RotateEnabled => (RotatePeriod > float.Epsilon);

    private void OnEnable()
    {
        m_EnableTime = Time.fixedTime;
    }

    private void FixedUpdate()
    {
        var internalTimer = Time.fixedTime - m_EnableTime;
        if (BobbingEnabled)
        {
            Transform.Translate(0, Time.fixedDeltaTime * BobbingDistance / BobbingPeriod / Mathf.PI * 10 * BobbingTrigFunc(internalTimer / BobbingPeriod * 2 * Mathf.PI), 0);
        }

        if (RotateEnabled)
        {
            Transform.Rotate(0, 360.0f * Time.fixedDeltaTime / RotatePeriod, 0);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (BobbingEnabled)
        {
            var pos = transform.position;
            if (BobbingAroundStartingPoint)
            {
                pos += Vector3.down * BobbingDistance / 2;
            }

            Handles.color = Color.green;
            Handles.DrawLine(pos, pos + Vector3.up * BobbingDistance);

            if (RotateEnabled)
            {
                const float spiralRadius = 1.0f;
                float spiralTurns = (BobbingPeriod / 2.0f) / RotatePeriod;
                int spiralSteps = Mathf.FloorToInt(spiralTurns * 16);
                for (var i = 1; i <= spiralSteps; ++i)
                {
                    var prevAngle = (90.0f + 360.0f * (i - 1) / spiralSteps * spiralTurns) * Mathf.Deg2Rad;
                    var prevY = BobbingDistance * (i - 1) / spiralSteps;
                    var prevOffset = new Vector3(Mathf.Cos(prevAngle) * spiralRadius, prevY, Mathf.Sin(prevAngle) * spiralRadius);

                    var angle = (90.0f + 360.0f * i / spiralSteps * spiralTurns) * Mathf.Deg2Rad;
                    var y = BobbingDistance * i / spiralSteps;
                    var offset = new Vector3(Mathf.Cos(angle) * spiralRadius, y, Mathf.Sin(angle) * spiralRadius);

                    Handles.DrawLine(pos + prevOffset, pos + offset);
                }
            }
        }
    }
}
