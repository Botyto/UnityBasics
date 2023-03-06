using UnityEditor;
using UnityEngine;

public class YDeath : BasicMonoBehaviour
{
    public bool Absolute = true;
    public float Y = 0.0f;

    [SerializeField]
    [HideInInspector]
    private Transform m_Transform;
    [SerializeField]
    [HideInInspector]
    private float m_StartY = float.NegativeInfinity;

    private void Start()
    {
        m_Transform = GetComponent<Transform>();
        m_StartY = m_Transform.position.y;
    }

    void FixedUpdate()
    {
        var minY = Absolute ? Y : m_StartY + Y;
        if ((m_Transform.position.y - minY) * Mathf.Sign(Y) > 0)
        {
            //TODO kill
        }
    }

    const float c_GizmoLinesSpacing = 2.0f;
    private void OnDrawGizmos()
    {
        var z = transform.position.z;
        var minY = Absolute ? Y : (m_StartY == float.NegativeInfinity ? transform.position.y : m_StartY) + Y;
        Handles.color = Color.red;
        Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;
        var currentCamera = Camera.current;
        if (currentCamera.orthographic)
        {
            var bottomLeft = currentCamera.ViewportToWorldPoint(Vector3.zero);
            var topRight = currentCamera.ViewportToWorldPoint(Vector3.one);
            Handles.DrawLine(new Vector3(bottomLeft.x, minY, z), new Vector3(topRight.x, minY, z));
        }
        else
        {
            var center = transform.position;
            center.y = minY;
            var halfSize = Mathf.Max(Mathf.Abs(minY - transform.position.y), 1.0f);
            Handles.DrawLine(center + new Vector3(-halfSize, 0, -halfSize), center + new Vector3(-halfSize, 0, halfSize));
            Handles.DrawLine(center + new Vector3(-halfSize, 0, halfSize), center + new Vector3(halfSize, 0, halfSize));
            Handles.DrawLine(center + new Vector3(halfSize, 0, halfSize), center + new Vector3(halfSize, 0, -halfSize));
            Handles.DrawLine(center + new Vector3(halfSize, 0, -halfSize), center + new Vector3(-halfSize, 0, -halfSize));
        }
    }

    private void OnDrawGizmosSelected()
    {
        var z = transform.position.z;
        var minY = Absolute ? Y : (m_StartY == float.NegativeInfinity ? transform.position.y : m_StartY) + Y;
        Handles.color = Color.red;
        Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;

        var currentCamera = Camera.current;
        if (currentCamera.orthographic)
        {
            var bottomLeft = currentCamera.ViewportToWorldPoint(Vector3.zero);
            var topRight = currentCamera.ViewportToWorldPoint(Vector3.one);
            Handles.DrawLine(new Vector3(bottomLeft.x, minY, z), new Vector3(topRight.x, minY, z));
            var signY = Mathf.Sign(Y);
            var positiveY = signY >= 0;
            var linesEndY = positiveY ? topRight.y : bottomLeft.y;
            var spaceToEnd = Mathf.Abs(linesEndY - minY);
            var linesLeft = bottomLeft.x - spaceToEnd + (-bottomLeft.x + signY * (bottomLeft.y - minY)) % c_GizmoLinesSpacing;
            var linesRight = topRight.x + spaceToEnd;
            for (var x = linesLeft; x < linesRight; x += c_GizmoLinesSpacing)
            {
                Handles.DrawLine(new Vector3(x, minY, z), new Vector3(x - spaceToEnd, linesEndY, z));
                Handles.DrawLine(new Vector3(x, minY, z), new Vector3(x + spaceToEnd, linesEndY, z));
            }
        }
        else
        {
            var center = transform.position;
            center.y = minY;
            var halfSize = Mathf.Max(Mathf.Abs(minY - transform.position.y), 1.0f);
            Handles.DrawLine(center + new Vector3(-halfSize, 0, -halfSize), center + new Vector3(-halfSize, 0, halfSize));
            Handles.DrawLine(center + new Vector3(-halfSize, 0, halfSize), center + new Vector3(halfSize, 0, halfSize));
            Handles.DrawLine(center + new Vector3(halfSize, 0, halfSize), center + new Vector3(halfSize, 0, -halfSize));
            Handles.DrawLine(center + new Vector3(halfSize, 0, -halfSize), center + new Vector3(-halfSize, 0, -halfSize));

            var bottomLeft = center - new Vector3(halfSize, 0, halfSize);
            var topRight = center + new Vector3(halfSize, 0, halfSize);
            for (var offset = 0.0f; offset < halfSize * 2; offset += c_GizmoLinesSpacing)
            {
                Handles.DrawLine(new Vector3(bottomLeft.x + offset, center.y, bottomLeft.z), new Vector3(topRight.x, center.y, topRight.z - offset));
                if (offset > float.Epsilon) { Handles.DrawLine(new Vector3(bottomLeft.x, center.y, bottomLeft.z + offset), new Vector3(topRight.x - offset, center.y, topRight.z)); }
                Handles.DrawLine(new Vector3(topRight.x - offset, center.y, bottomLeft.z), new Vector3(bottomLeft.x, center.y, topRight.z - offset));
                if (offset > float.Epsilon) { Handles.DrawLine(new Vector3(topRight.x, center.y, bottomLeft.z + offset), new Vector3(bottomLeft.x + offset, center.y, topRight.z)); }
            }
        }
    }
}
