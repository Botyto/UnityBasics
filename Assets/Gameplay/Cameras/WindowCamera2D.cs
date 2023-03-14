using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class WindowCamera2D : MonoBehaviour
{
    public Transform Target;

    [Range(0.0f, 1.0f)]
    public float Width = 0.0f;
    [Range(0.0f, 1.0f)]
    public float Height = 0.0f;
    [Range(0.0f, 1.0f)]
    public float CenterX = 0.5f;
    [Range(0.0f, 1.0f)]
    public float CenterY = 0.5f;

    [FindInThis]
    [SerializeField]
    [HideInInspector]
    private Camera Camera;

    private void Update()
    {
        if (Target == null) { return; }
        if (!Camera.orthographic) { return; }

        //TODO update position
    }

    private Rect GetWindow()
    {
        var camera = Camera;
        if (camera == null) { camera = GetComponent<Camera>(); }
        var center = camera.transform.position;
        var sizeV = camera.orthographicSize * 2;
        var sizeH = sizeV * camera.aspect;

        var left = center.x - sizeH * Width / 2.0f + sizeH * (CenterX - 0.5f);
        var bottom = center.y - sizeV * Height / 2.0f + sizeV * (CenterY - 0.5f);

        return new Rect(left, bottom, sizeH * Width, sizeV * Height);
    }

    private void OnDrawGizmos()
    {
        var camera = GetComponent<Camera>();
        if (!camera.orthographic) { return; }

        Handles.color = Color.white;
        Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;

        var window = GetWindow();
        var z = camera.transform.position.z;
        const float extent = 1.0f;
        Handles.DrawLine(new Vector3(window.xMin, window.yMin - extent, z), new Vector3(window.xMin, window.yMax + extent, z));
        Handles.DrawLine(new Vector3(window.xMin - extent, window.yMax, z), new Vector3(window.xMax + extent, window.yMax, z));
        Handles.DrawLine(new Vector3(window.xMax, window.yMax + extent, z), new Vector3(window.xMax, window.yMin - extent, z));
        Handles.DrawLine(new Vector3(window.xMax + extent, window.yMin, z), new Vector3(window.xMin - extent, window.yMin, z));
    }
}
