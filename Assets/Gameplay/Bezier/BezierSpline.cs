using System;
using UnityEngine;
using UnityEditor;

[Serializable]
[ExecuteInEditMode]
public class BezierSpline : MonoBehaviour
{
    public enum ControlPointMode
    {
        Free,
        Aligned,
        Mirrored
    }

    [SerializeField]
    private Vector3[] ControlPoints;

    [SerializeField]
    private ControlPointMode[] Modes;

    [FindInThis]
    [SerializeField]
    [HideInInspector]
    private Transform Transform;

    [SerializeField]
    private bool Loop;

    public bool IsLoop
    {
        get
        {
            return Loop;
        }
        set
        {
            Loop = value;
            if (value == true)
            {
                Modes[Modes.Length - 1] = Modes[0];
                SetControlPoint(0, ControlPoints[0]);
            }
        }
    }

    public int ControlPointCount => ControlPoints.Length;
    public int CurveCount => (ControlPointCount - 1) / 3;

    public Vector3 this[int index]
    {
        get { return GetControlPoint(index); }
        set { SetControlPoint(index, value); }
    }

    public Vector3 GetControlPoint(int index)
    {
        return ControlPoints[index];
    }

    public void SetControlPoint(int index, Vector3 point)
    {
        if (index % 3 == 0)
        {
            Vector3 delta = point - ControlPoints[index];
            if (Loop)
            {
                if (index == 0)
                {
                    ControlPoints[1] += delta;
                    ControlPoints[ControlPoints.Length - 2] += delta;
                    ControlPoints[ControlPoints.Length - 1] = point;
                }
                else if (index == ControlPoints.Length - 1)
                {
                    ControlPoints[0] = point;
                    ControlPoints[1] += delta;
                    ControlPoints[index - 1] += delta;
                }
                else
                {
                    ControlPoints[index - 1] += delta;
                    ControlPoints[index + 1] += delta;
                }
            }
            else
            {
                if (index > 0)
                {
                    ControlPoints[index - 1] += delta;
                }
                if (index + 1 < ControlPoints.Length)
                {
                    ControlPoints[index + 1] += delta;
                }
            }
        }
        ControlPoints[index] = point;
        EnforceMode(index);
    }

    public ControlPointMode GetControlPointMode(int index)
    {
        return Modes[(index + 1) / 3];
    }

    public void SetControlPointMode(int index, ControlPointMode mode)
    {
        int modeIndex = (index + 1) / 3;
        Modes[modeIndex] = mode;
        if (Loop)
        {
            if (modeIndex == 0)
            {
                Modes[Modes.Length - 1] = mode;
            }
            else if (modeIndex == Modes.Length - 1)
            {
                Modes[0] = mode;
            }
        }
        EnforceMode(index);
    }

    private void EnforceMode(int index)
    {
        int modeIndex = (index + 1) / 3;
        ControlPointMode mode = Modes[modeIndex];
        if (mode == ControlPointMode.Free || !Loop && (modeIndex == 0 || modeIndex == Modes.Length - 1))
        {
            return;
        }

        int middleIndex = modeIndex * 3;
        int fixedIndex, enforcedIndex;
        if (index <= middleIndex)
        {
            fixedIndex = middleIndex - 1;
            if (fixedIndex < 0)
            {
                fixedIndex = ControlPoints.Length - 2;
            }
            enforcedIndex = middleIndex + 1;
            if (enforcedIndex >= ControlPoints.Length)
            {
                enforcedIndex = 1;
            }
        }
        else
        {
            fixedIndex = middleIndex + 1;
            if (fixedIndex >= ControlPoints.Length)
            {
                fixedIndex = 1;
            }
            enforcedIndex = middleIndex - 1;
            if (enforcedIndex < 0)
            {
                enforcedIndex = ControlPoints.Length - 2;
            }
        }


        Vector3 middle = ControlPoints[middleIndex];
        Vector3 enforcedTangent = middle - ControlPoints[fixedIndex];
        if (mode == ControlPointMode.Aligned)
        {
            enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, ControlPoints[enforcedIndex]);
        }
        ControlPoints[enforcedIndex] = middle + enforcedTangent;
    }

    public void Reset()
    {
        SetCircle();
    }

    private void SetCircle()
    {
        var off = (4.0f / 3.0f) * Mathf.Tan(Mathf.PI / 8);
        ControlPoints = new Vector3[] {
            new Vector3(1.0f, 0.0f, 0.0f),

            new Vector3(1.0f, off,  0.0f),
            new Vector3(off,  1.0f, 0.0f),
            new Vector3(0.0f, 1.0f, 0.0f),

            new Vector3(-off,  1.0f, 0.0f),
            new Vector3(-1.0f, off,  0.0f),
            new Vector3(-1.0f, 0.0f, 0.0f),

            new Vector3(-1.0f, -off,  0.0f),
            new Vector3(-off,  -1.0f, 0.0f),
            new Vector3(0.0f,  -1.0f, 0.0f),

            new Vector3(off,  -1.0f, 0.0f),
            new Vector3(1.0f, -off,  0.0f),
            new Vector3(1.0f,  0.0f, 0.0f),
        };
        Modes = new ControlPointMode[] {
            ControlPointMode.Mirrored,
            ControlPointMode.Mirrored,
            ControlPointMode.Mirrored,
            ControlPointMode.Mirrored,
            ControlPointMode.Mirrored,
        };
        Loop = true;
    }

    public Vector3 GetPoint(float t)
    {
        var i = GetCurveControlPointIndex(ref t);
        return Transform.TransformPoint(GetPoint(ControlPoints[i], ControlPoints[i + 1], ControlPoints[i + 2], ControlPoints[i + 3], t));
    }

    private int GetCurveControlPointIndex(ref float t)
    {
        if (t >= 1f)
        {
            t = 1f;
            return ControlPoints.Length - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * CurveCount;
            var i = (int)t;
            t -= i;
            return i * 3;
        }
    }

    public Vector3 GetVelocity(float t)
    {
        var i = GetCurveControlPointIndex(ref t);
        return Transform.TransformPoint(GetFirstDerivative(ControlPoints[i], ControlPoints[i + 1], ControlPoints[i + 2], ControlPoints[i + 3], t)) - Transform.position;
    }

    public Vector3 GetDirection(float t)
    {
        return GetVelocity(t).normalized;
    }

    public void AddSegment()
    {
        Vector3 point = ControlPoints[ControlPoints.Length - 1];
        Array.Resize(ref ControlPoints, ControlPoints.Length + 3);
        point.x += 1f;
        ControlPoints[ControlPoints.Length - 3] = point;
        point.x += 1f;
        ControlPoints[ControlPoints.Length - 2] = point;
        point.x += 1f;
        ControlPoints[ControlPoints.Length - 1] = point;

        Array.Resize(ref Modes, Modes.Length + 1);
        Modes[Modes.Length - 1] = Modes[Modes.Length - 2];

        EnforceMode(ControlPoints.Length - 4);

        if (Loop)
        {
            ControlPoints[ControlPoints.Length - 1] = ControlPoints[0];
            Modes[Modes.Length - 1] = Modes[0];
            EnforceMode(0);
        }
    }

    public void SplitSegment(int controlPointIndex)
    {
        var newControlPoints = Array.CreateInstance(typeof(Vector3), ControlPoints.Length + 3) as Vector3[];
        var copyCount = ((controlPointIndex + 1) / 3) * 3 + 2;
        Array.Copy(ControlPoints, 0, newControlPoints, 0, copyCount);
        Array.Copy(ControlPoints, copyCount, newControlPoints, copyCount + 3, ControlPoints.Length - copyCount);
        var centerT = (((copyCount - 1) / 3) + 0.5f) / CurveCount;
        var point = GetPoint(centerT);
        var fixFactor = 0.2f;
        newControlPoints[copyCount + 0] = Transform.InverseTransformPoint(point - GetVelocity(centerT) * fixFactor);
        newControlPoints[copyCount + 1] = Transform.InverseTransformPoint(point);
        newControlPoints[copyCount + 2] = Transform.InverseTransformPoint(point + GetVelocity(centerT) * fixFactor);
        var newPointModes = Array.CreateInstance(typeof(ControlPointMode), Modes.Length + 1) as ControlPointMode[];
        var modesCopyCount = (copyCount - 1) / 3;
        Array.Copy(Modes, 0, newPointModes, 0, modesCopyCount);
        Array.Copy(Modes, modesCopyCount, newPointModes, modesCopyCount + 1, Modes.Length - modesCopyCount);
        if (modesCopyCount > 0 && newPointModes[modesCopyCount - 1] == ControlPointMode.Mirrored)
        {
            newPointModes[modesCopyCount - 1] = ControlPointMode.Aligned;
        }
        if (modesCopyCount < newPointModes.Length - 2 && newPointModes[modesCopyCount + 1] == ControlPointMode.Mirrored)
        {
            newPointModes[modesCopyCount + 1] = ControlPointMode.Aligned;
        }
        newPointModes[modesCopyCount] = ControlPointMode.Mirrored;

        // De Casteljau
        var orgStart = copyCount - 2;
        var beta = new Vector3[6];
        beta[0] = Vector3.Lerp(ControlPoints[orgStart], ControlPoints[orgStart + 1], 0.5f);
        beta[1] = Vector3.Lerp(ControlPoints[orgStart + 1], ControlPoints[orgStart + 2], 0.5f);
        beta[2] = Vector3.Lerp(ControlPoints[orgStart + 2], ControlPoints[orgStart + 3], 0.5f);
        beta[3] = Vector3.Lerp(beta[0], beta[1], 0.5f);
        beta[4] = Vector3.Lerp(beta[1], beta[2], 0.5f);
        beta[5] = Vector3.Lerp(beta[3], beta[4], 0.5f);

        newControlPoints[orgStart + 1] = beta[0];
        newControlPoints[orgStart + 2] = beta[3];
        newControlPoints[orgStart + 3] = beta[5];
        newControlPoints[orgStart + 4] = beta[4];
        newControlPoints[orgStart + 5] = beta[2];

        ControlPoints = newControlPoints;
        Modes = newPointModes;
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < ControlPointCount - 1; i += 3)
        {
            var p0 = Transform.TransformPoint(this[i + 0]);
            var p1 = Transform.TransformPoint(this[i + 1]);
            var p2 = Transform.TransformPoint(this[i + 2]);
            var p3 = Transform.TransformPoint(this[i + 3]);
            Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
        }
    }

    private static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        if (t >= 1.0f) { return p3; }
        if (t <= 0.0f) { return p0; }
        float oneMinusT = 1f - t;
        return
            oneMinusT * oneMinusT * oneMinusT * p0 +
            3f * oneMinusT * oneMinusT * t * p1 +
            3f * oneMinusT * t * t * p2 +
            t * t * t * p3;
    }

    private static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        if (t >= 1.0f) { return p3; }
        if (t <= 0.0f) { return p0; }
        float oneMinusT = 1f - t;
        return
            3f * oneMinusT * oneMinusT * (p1 - p0) +
            6f * oneMinusT * t * (p2 - p1) +
            3f * t * t * (p3 - p2);
    }
}
