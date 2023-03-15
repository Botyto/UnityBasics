using System;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
[Serializable]
public class BezierSpline : MonoBehaviour
{
    public enum BezierControlPointMode
    {
        Free,
        Aligned,
        Mirrored
    }

    [SerializeField]
    private Vector3[] m_ControlPoints;

    [SerializeField]
    private BezierControlPointMode[] m_Modes;

    [SerializeField]
    private bool m_Loop;

    public bool Loop
    {
        get
        {
            return m_Loop;
        }
        set
        {
            m_Loop = value;
            if (value == true)
            {
                m_Modes[m_Modes.Length - 1] = m_Modes[0];
                SetControlPoint(0, m_ControlPoints[0]);
            }
        }
    }

    public int ControlPointCount => m_ControlPoints.Length;
    public int CurveCount => (ControlPointCount - 1) / 3;

    public Vector3 this[int index]
    {
        get { return GetControlPoint(index); }
        set { SetControlPoint(index, value); }
    }

    public Vector3 GetControlPoint(int index)
    {
        return m_ControlPoints[index];
    }

    public void SetControlPoint(int index, Vector3 point)
    {
        if (index % 3 == 0)
        {
            Vector3 delta = point - m_ControlPoints[index];
            if (m_Loop)
            {
                if (index == 0)
                {
                    m_ControlPoints[1] += delta;
                    m_ControlPoints[m_ControlPoints.Length - 2] += delta;
                    m_ControlPoints[m_ControlPoints.Length - 1] = point;
                }
                else if (index == m_ControlPoints.Length - 1)
                {
                    m_ControlPoints[0] = point;
                    m_ControlPoints[1] += delta;
                    m_ControlPoints[index - 1] += delta;
                }
                else
                {
                    m_ControlPoints[index - 1] += delta;
                    m_ControlPoints[index + 1] += delta;
                }
            }
            else
            {
                if (index > 0)
                {
                    m_ControlPoints[index - 1] += delta;
                }
                if (index + 1 < m_ControlPoints.Length)
                {
                    m_ControlPoints[index + 1] += delta;
                }
            }
        }
        m_ControlPoints[index] = point;
        EnforceMode(index);
    }

    public BezierControlPointMode GetControlPointMode(int index)
    {
        return m_Modes[(index + 1) / 3];
    }

    public void SetControlPointMode(int index, BezierControlPointMode mode)
    {
        int modeIndex = (index + 1) / 3;
        m_Modes[modeIndex] = mode;
        if (m_Loop)
        {
            if (modeIndex == 0)
            {
                m_Modes[m_Modes.Length - 1] = mode;
            }
            else if (modeIndex == m_Modes.Length - 1)
            {
                m_Modes[0] = mode;
            }
        }
        EnforceMode(index);
    }

    private void EnforceMode(int index)
    {
        int modeIndex = (index + 1) / 3;
        BezierControlPointMode mode = m_Modes[modeIndex];
        if (mode == BezierControlPointMode.Free || !m_Loop && (modeIndex == 0 || modeIndex == m_Modes.Length - 1))
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
                fixedIndex = m_ControlPoints.Length - 2;
            }
            enforcedIndex = middleIndex + 1;
            if (enforcedIndex >= m_ControlPoints.Length)
            {
                enforcedIndex = 1;
            }
        }
        else
        {
            fixedIndex = middleIndex + 1;
            if (fixedIndex >= m_ControlPoints.Length)
            {
                fixedIndex = 1;
            }
            enforcedIndex = middleIndex - 1;
            if (enforcedIndex < 0)
            {
                enforcedIndex = m_ControlPoints.Length - 2;
            }
        }


        Vector3 middle = m_ControlPoints[middleIndex];
        Vector3 enforcedTangent = middle - m_ControlPoints[fixedIndex];
        if (mode == BezierControlPointMode.Aligned)
        {
            enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, m_ControlPoints[enforcedIndex]);
        }
        m_ControlPoints[enforcedIndex] = middle + enforcedTangent;
    }

    public void Reset()
    {
        SetCircle();
    }

    private void SetCircle()
    {
        var off = (4.0f / 3.0f) * Mathf.Tan(Mathf.PI / 8);
        m_ControlPoints = new Vector3[] {
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
        m_Modes = new BezierControlPointMode[] {
            BezierControlPointMode.Mirrored,
            BezierControlPointMode.Mirrored,
            BezierControlPointMode.Mirrored,
            BezierControlPointMode.Mirrored,
            BezierControlPointMode.Mirrored,
        };
    }

    public Vector3 GetPoint(float t)
    {
        var i = GetCurveControlPointIndex(ref t);
        return transform.TransformPoint(GetPoint(m_ControlPoints[i], m_ControlPoints[i + 1], m_ControlPoints[i + 2], m_ControlPoints[i + 3], t));
    }

    private int GetCurveControlPointIndex(ref float t)
    {
        if (t >= 1f)
        {
            t = 1f;
            return m_ControlPoints.Length - 4;
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
        return transform.TransformPoint(GetFirstDerivative(m_ControlPoints[i], m_ControlPoints[i + 1], m_ControlPoints[i + 2], m_ControlPoints[i + 3], t)) - transform.position;
    }

    public Vector3 GetDirection(float t)
    {
        return GetVelocity(t).normalized;
    }

    public void AddSegment()
    {
        Vector3 point = m_ControlPoints[m_ControlPoints.Length - 1];
        Array.Resize(ref m_ControlPoints, m_ControlPoints.Length + 3);
        point.x += 1f;
        m_ControlPoints[m_ControlPoints.Length - 3] = point;
        point.x += 1f;
        m_ControlPoints[m_ControlPoints.Length - 2] = point;
        point.x += 1f;
        m_ControlPoints[m_ControlPoints.Length - 1] = point;

        Array.Resize(ref m_Modes, m_Modes.Length + 1);
        m_Modes[m_Modes.Length - 1] = m_Modes[m_Modes.Length - 2];

        EnforceMode(m_ControlPoints.Length - 4);

        if (m_Loop)
        {
            m_ControlPoints[m_ControlPoints.Length - 1] = m_ControlPoints[0];
            m_Modes[m_Modes.Length - 1] = m_Modes[0];
            EnforceMode(0);
        }
    }

    public void SplitSegment(int controlPointIndex)
    {
        var newControlPoints = Array.CreateInstance(typeof(Vector3), m_ControlPoints.Length + 3) as Vector3[];
        var copyCount = ((controlPointIndex + 1) / 3) * 3 + 2;
        Array.Copy(m_ControlPoints, 0, newControlPoints, 0, copyCount);
        Array.Copy(m_ControlPoints, copyCount, newControlPoints, copyCount + 3, m_ControlPoints.Length - copyCount);
        var centerT = (((copyCount - 1) / 3) + 0.5f) / CurveCount;
        var point = GetPoint(centerT);
        var fixFactor = 0.2f;
        newControlPoints[copyCount + 0] = transform.InverseTransformPoint(point - GetVelocity(centerT) * fixFactor);
        newControlPoints[copyCount + 1] = transform.InverseTransformPoint(point);
        newControlPoints[copyCount + 2] = transform.InverseTransformPoint(point + GetVelocity(centerT) * fixFactor);
        var newPointModes = Array.CreateInstance(typeof(BezierControlPointMode), m_Modes.Length + 1) as BezierControlPointMode[];
        var modesCopyCount = (copyCount - 1) / 3;
        Array.Copy(m_Modes, 0, newPointModes, 0, modesCopyCount);
        Array.Copy(m_Modes, modesCopyCount, newPointModes, modesCopyCount + 1, m_Modes.Length - modesCopyCount);
        m_Modes[modesCopyCount] = BezierControlPointMode.Aligned;

        // De Casteljau
        var orgStart = copyCount - 2;
        var beta = new Vector3[6];
        beta[0] = Vector3.Lerp(m_ControlPoints[orgStart], m_ControlPoints[orgStart + 1], 0.5f);
        beta[1] = Vector3.Lerp(m_ControlPoints[orgStart + 1], m_ControlPoints[orgStart + 2], 0.5f);
        beta[2] = Vector3.Lerp(m_ControlPoints[orgStart + 2], m_ControlPoints[orgStart + 3], 0.5f);
        beta[3] = Vector3.Lerp(beta[0], beta[1], 0.5f);
        beta[4] = Vector3.Lerp(beta[1], beta[2], 0.5f);
        beta[5] = Vector3.Lerp(beta[3], beta[4], 0.5f);

        newControlPoints[orgStart + 1] = beta[0];
        newControlPoints[orgStart + 2] = beta[3];
        newControlPoints[orgStart + 3] = beta[5];
        newControlPoints[orgStart + 4] = beta[4];
        newControlPoints[orgStart + 5] = beta[2];

        m_ControlPoints = newControlPoints;
        m_Modes = newPointModes;
    }

    private void OnDrawGizmos()
    {
        var myTransform = transform;
        for (int i = 0; i < ControlPointCount - 1; i += 3)
        {
            var p0 = myTransform.TransformPoint(this[i + 0]);
            var p1 = myTransform.TransformPoint(this[i + 1]);
            var p2 = myTransform.TransformPoint(this[i + 2]);
            var p3 = myTransform.TransformPoint(this[i + 3]);
            Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
        }
    }

    private static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return
            oneMinusT * oneMinusT * oneMinusT * p0 +
            3f * oneMinusT * oneMinusT * t * p1 +
            3f * oneMinusT * t * t * p2 +
            t * t * t * p3;
    }

    private static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return
            3f * oneMinusT * oneMinusT * (p1 - p0) +
            6f * oneMinusT * t * (p2 - p1) +
            3f * t * t * (p3 - p2);
    }
}
