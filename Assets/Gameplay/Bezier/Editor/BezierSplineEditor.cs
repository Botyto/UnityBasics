using UnityEngine;
using UnityEditor;
using static BezierSpline;

[CustomEditor(typeof(BezierSpline))]
public class BezierSplineEditor : Editor
{
    private BezierSpline Spline;
    private Transform HandleTransform;
    private Quaternion HandleRotation;
    private int SelectedIndex = -1;

    private void OnSceneGUI()
    {
        Spline = target as BezierSpline;
        HandleTransform = Spline.transform;
        HandleRotation = Tools.pivotRotation == PivotRotation.Local ? HandleTransform.rotation : Quaternion.identity;

        Vector3 p0 = ShowPoint(0);
        for (int i = 1; i < Spline.ControlPointCount; i += 3)
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
        Spline = target as BezierSpline;
        EditorGUI.BeginChangeCheck();
        bool loop = EditorGUILayout.Toggle("Loop", Spline.IsLoop);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(Spline, "Toggle loop");
            EditorUtility.SetDirty(Spline);
            Spline.IsLoop = loop;
        }
        if (SelectedIndex >= 0 && SelectedIndex < Spline.ControlPointCount)
        {
            DrawSelectedPointInspector();
            GUILayout.BeginHorizontal();
            GUI.enabled = SelectedIndex > 1 || Spline.IsLoop; ;
            if (GUILayout.Button("Add before"))
            {
                Undo.RecordObject(Spline, "Add control point");
                EditorUtility.SetDirty(Spline);
                var idx = ((SelectedIndex + 1) / 3) * 3 - 2;
                if (idx < 0) { idx += Spline.ControlPointCount; }
                Spline.SplitSegment(idx);
                SelectedIndex = idx + 2;
            }
            GUI.enabled = SelectedIndex < Spline.ControlPointCount - 2 || Spline.IsLoop;
            if (GUILayout.Button("Add after"))
            {
                Undo.RecordObject(Spline, "Add control point");
                EditorUtility.SetDirty(Spline);
                var idx = ((SelectedIndex + 1) / 3) * 3;
                Spline.SplitSegment(idx);
                SelectedIndex = idx + 3;
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();
        }
        else
        {
            if (GUILayout.Button("Add segment"))
            {
                Undo.RecordObject(Spline, "Add segment");
                Spline.AddSegment();
                EditorUtility.SetDirty(Spline);
            }
        }
    }

    private void DrawSelectedPointInspector()
    {
        GUILayout.Label("Selected control point");
        EditorGUI.BeginChangeCheck();
        Vector3 point = EditorGUILayout.Vector3Field("Position", Spline.GetControlPoint(SelectedIndex));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(Spline, "Move control point");
            EditorUtility.SetDirty(Spline);
            Spline.SetControlPoint(SelectedIndex, point);
        }

        EditorGUI.BeginChangeCheck();
        ControlPointMode mode = (ControlPointMode)
            EditorGUILayout.EnumPopup("Mode", Spline.GetControlPointMode(SelectedIndex));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(Spline, "Change control point mode");
            Spline.SetControlPointMode(SelectedIndex, mode);
            EditorUtility.SetDirty(Spline);
        }
    }

    private const float c_HandleSize = 0.04f;
    private const float c_PickSize = 0.06f;
    private static readonly Color[] s_ModeColors = { Color.white, Color.yellow, Color.cyan };

    private Vector3 ShowPoint(int index)
    {
        Vector3 point = HandleTransform.TransformPoint(Spline.GetControlPoint(index));
        float size = HandleUtility.GetHandleSize(point);
        if (index == 0)
        {
            size *= 2f;
        }
        Handles.color = s_ModeColors[(int)Spline.GetControlPointMode(index)];
        if (Handles.Button(point, HandleRotation, size * c_HandleSize, size * c_PickSize, Handles.DotHandleCap))
        {
            SelectedIndex = index;
            Repaint();
        }
        if (SelectedIndex == index)
        {
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, HandleRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(Spline, "Move Point");
                EditorUtility.SetDirty(Spline);
                Spline.SetControlPoint(index, HandleTransform.InverseTransformPoint(point));
            }
        }
        return point;
    }
}
