using UnityEngine;

public class Kernels
{
    private float KernelRadius;
    private float KernelRadiusSquared;
    private float KernelRadiusTo6;
    private float KernelRadiusTo9;

    public Kernels( float KernelRadiusIn )
    {
        KernelRadius = KernelRadiusIn;

        KernelRadiusSquared = KernelRadius * KernelRadius;

        KernelRadiusTo6 = Mathf.Pow(KernelRadius, 6);

        KernelRadiusTo9 = Mathf.Pow(KernelRadius, 9);
    }

    public float Poly6( Particle P1, Particle P2 )
    {
        float DistSqr = (P1.Position - P2.Position).sqrMagnitude;

        if ( KernelRadiusSquared < DistSqr )
        {
            return 0;
        }

        float Coefficient = 315f / (64f * Mathf.PI * KernelRadiusTo9);

        return Coefficient * Mathf.Pow(KernelRadiusSquared - DistSqr, 3);
    }

    public Vector3 SpikyGrad( Particle P1, Particle P2 )
    {
        Vector3 DeltaPos = (P1.Position - P2.Position);

        float Dist = DeltaPos.magnitude;

        if ( KernelRadius < Dist )
        {
            return Vector3.zero;
        }

        float Coefficient = 45f / (Mathf.PI * KernelRadiusTo6);

        return -Coefficient * DeltaPos.normalized * Mathf.Pow(KernelRadius - Dist, 2);
    }

    public float Laplacian( Particle P1, Particle P2 )
    {
        Vector3 DeltaPos = (P1.Position - P2.Position);

        float Dist = DeltaPos.magnitude;

        if ( KernelRadius < Dist )
        {
            return 0;
        }

        float Coefficient = 45f / (Mathf.PI * KernelRadiusTo6);

        return Coefficient * (KernelRadius - Dist);
    }
}
