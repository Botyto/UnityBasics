using System;
using UnityEngine;

public class SecondOrderDynamics
{
    private Vector3 xp;  // Previous input
    private Vector3 y, yd;  // State variables
    private float _w, _z, _d, k1, k2, k3;  // Constants

    public SecondOrderDynamics(float f, float z, float r, Vector3 x0)
    {
        xp = x0;
        y = x0;
        yd = Vector3.zero;

        _w = 2 * Mathf.PI * f;
        _z = z;
        _d = _w * Mathf.Sqrt(Mathf.Abs(z * z - 1.0f));
        k1 = z / (Mathf.PI * f);
        k2 = 1.0f / (_w * _w);
        k3 = r * z / _w;
    }

    public Vector3 Update(float t, Vector3 x)
    {
        Vector3 xd = (x - xp) / t;
        xp = x;
        return Update(t, x, xd);
    }

    public Vector3 Update(float t, Vector3 x, Vector3 xd)
    {
        float k1Stable, k2Stable;
        if (_w * t < _z)  // Clamp k2 to guarantee stability without jitter
        {
            k1Stable = k1;
            k2Stable = Mathf.Max(k2, (float)(t * t / 2.0f + t * k1 / 2.0), t * k1);
        }
        else  // Use pole matching when the system is very fast
        {
            float t1 = Mathf.Exp(-_z * _w * t);
            float alpha = 2 * t1 * (_z <= 1 ? Mathf.Cos(t * _d) : (float)Math.Cosh(t * _d));
            float beta = t1 * t1;
            float t2 = t / (1 + beta - alpha);
            k1Stable = (1 - beta) * t2;
            k2Stable = t * t2;
        }
        y += t * yd;  // Integrate position by velocity
        yd += t * (x + k3 * xd - y - k1 * yd) / k2Stable;  // Integrate velocity by acceleration
        return y;
    }
}
