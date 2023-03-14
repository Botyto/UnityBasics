using UnityEngine;
using UnityEditor;
using static BezierSpline;

[CustomEditor(typeof(BezierSpline))]
public class BezierSplineEditor : Editor
{
    private BezierSpline m_Spline;
    private Transform m_HandleTransform;
    private Quaternion m_HandleRotation;
    private int m_SelectedIndex = -1;

    private void OnSceneGUI()
    {
        m_Spline = target as BezierSpline;
        m_HandleTransform = m_Spline.transform;
        m_HandleRotation = Tools.pivotRotation == PivotRotation.Local ? m_HandleTransform.rotation : Quaternion.identity;

        Vector3 p0 = ShowPoint(0);
        for (int i = 1; i < m_Spline.ControlPointCount; i += 3)
        {
            Vector3 p1 = ShowPoint(i);
            Vector3 p2 = ShowPoint(i + 1);
            Vector3 p3 = ShowPoint(i + 2);

            Handles.color = Color.gray;
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p2, p3);
            
            p0 = p3;
        }
    }

    public override void OnInspectorGUI()
    {
        m_Spline = target as BezierSpline;
        EditorGUI.BeginChangeCheck();
        bool loop = EditorGUILayout.Toggle("Loop", m_Spline.Loop);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(m_Spline, "Toggle loop");
            EditorUtility.SetDirty(m_Spline);
            m_Spline.Loop = loop;
        }
        if (m_SelectedIndex >= 0 && m_SelectedIndex < m_Spline.ControlPointCount)
        {
            DrawSelectedPointInspector();
        }
        if (GUILayout.Button("Add segment"))
        {
            Undo.RecordObject(m_Spline, "Add segment");
            m_Spline.AddSegment();
            EditorUtility.SetDirty(m_Spline);
        }
    }

    private void DrawSelectedPointInspector()
    {
        GUILayout.Label("Selected control point");
        EditorGUI.BeginChangeCheck();
        Vector3 point = EditorGUILayout.Vector3Field("Position", m_Spline.GetControlPoint(m_SelectedIndex));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(m_Spline, "Move control point");
            EditorUtility.SetDirty(m_Spline);
            m_Spline.SetControlPoint(m_SelectedIndex, point);
        }

        EditorGUI.BeginChangeCheck();
        BezierControlPointMode mode = (BezierControlPointMode)
            EditorGUILayout.EnumPopup("Mode", m_Spline.GetControlPointMode(m_SelectedIndex));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(m_Spline, "Change control point mode");
            m_Spline.SetControlPointMode(m_SelectedIndex, mode);
            EditorUtility.SetDirty(m_Spline);
        }
    }

    private const float c_HandleSize = 0.04f;
    private const float c_PickSize = 0.06f;
    private static Color[] s_ModeColors = { Color.white, Color.yellow, Color.cyan };

    private Vector3 ShowPoint(int index)
    {
        Vector3 point = m_HandleTransform.TransformPoint(m_Spline.GetControlPoint(index));
        float size = HandleUtility.GetHandleSize(point);
        if (index == 0)
        {
            size *= 2f;
        }
        Handles.color = s_ModeColors[(int)m_Spline.GetControlPointMode(index)];
        if (Handles.Button(point, m_HandleRotation, size * c_HandleSize, size * c_PickSize, Handles.DotHandleCap))
        {
            m_SelectedIndex = index;
            Repaint();
        }
        if (m_SelectedIndex == index)
        {
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, m_HandleRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(m_Spline, "Move Point");
                EditorUtility.SetDirty(m_Spline);
                m_Spline.SetControlPoint(index, m_HandleTransform.InverseTransformPoint(point));
            }
        }
        return point;
    }
}
