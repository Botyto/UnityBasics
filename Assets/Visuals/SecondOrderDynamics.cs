using System;
using UnityEngine;

public class SecondOrderDynamics
{
    private Vector3 PreviousX;  // Previous input
    private Vector3 Y, DeltaY;  // State variables
    private float W, Z, D, K1, K2, K3;  // Constants

    public SecondOrderDynamics(float f, float z, float r, Vector3 x0)
    {
        PreviousX = x0;
        Y = x0;
        DeltaY = Vector3.zero;

        W = 2 * Mathf.PI * f;
        Z = z;
        D = W * Mathf.Sqrt(Mathf.Abs(z * z - 1.0f));
        K1 = z / (Mathf.PI * f);
        K2 = 1.0f / (W * W);
        K3 = r * z / W;
    }

    public Vector3 Update(float t, Vector3 x)
    {
        Vector3 xd = (x - PreviousX) / t;
        PreviousX = x;
        return Update(t, x, xd);
    }

    public Vector3 Update(float t, Vector3 x, Vector3 xd)
    {
        // float k1Stable;
        float k2Stable;
        if (W * t < Z)  // Clamp k2 to guarantee stability without jitter
        {
            // k1Stable = K1;
            k2Stable = Mathf.Max(K2, (float)(t * t / 2.0f + t * K1 / 2.0), t * K1);
        }
        else  // Use pole matching when the system is very fast
        {
            float t1 = Mathf.Exp(-Z * W * t);
            float alpha = 2 * t1 * (Z <= 1 ? Mathf.Cos(t * D) : (float)Math.Cosh(t * D));
            float beta = t1 * t1;
            float t2 = t / (1 + beta - alpha);
            // k1Stable = (1 - beta) * t2;
            k2Stable = t * t2;
        }
        Y += t * DeltaY;  // Integrate position by velocity
        DeltaY += t * (x + K3 * xd - Y - K1 * DeltaY) / k2Stable;  // Integrate velocity by acceleration
        return Y;
    }
}
