using UnityEngine;
using System;

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

    public void AddCurve()
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
