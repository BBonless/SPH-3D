using DataStructures.ViliWonka.KDTree;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;

static class Util
{
    public static float DistanceSquared(Particle P1, Particle P2)
    {
        return (P1.Position - P2.Position).sqrMagnitude;
    }
}

public class Domain : MonoBehaviour
{
    Particle[]  Particles;
    Parameters  P;
    Kernels     K;
    Stopwatch   S;

    private void Start()
    {
        P = gameObject.GetComponent<Parameters>();
        K = new Kernels(P.SmoothingRadius);
        S = new Stopwatch();
        Particles = new Particle[P.ParticleBlockSize];

        Simulation_Reset();
    }

    public void X(Stopwatch Sx, string MS)
    {
        Sx.Stop();
        print(MS + " " + Sx.Elapsed + " | " + Sx.ElapsedTicks);
    }

    #region Simulation
    public void Simulation_Reset()
    {
        Particles = new Particle[P.ParticleBlockSize];

        Particle_SpawnBlock();
    }

    public void Simulation_Step()
    {
        //Get Neighbors
        S.Restart();
        //foreach ( Particle P1 in Particles )
        //{
        //    Particle_SetNeighborsBruteforce(P1);
        //}

        Parallel.ForEach(Particles, P1 =>
        {
            Particle_SetNeighborsBruteforce(P1);
        });
        X(S, "Finding Neighbors:");

        for ( int I = 0; I < P.SolverIterations; I++ )
        {
            S.Restart();
            //foreach ( Particle P1 in Particles )
            //{
            //    Particle_SetDensityPressure(P1);
            //}

            Parallel.ForEach(Particles, P1 =>
            {
                Particle_SetDensityPressure(P1);
            });
            X(S, "Computing Density & Pressure:");

            S.Restart();
            //foreach ( Particle P1 in Particles )
            //{
            //    Particle_SetNetForce(P1);
            //}

            Parallel.ForEach(Particles, P1 =>
            {
                Particle_SetNetForce(P1);
            });
            X(S, "Computing Forces:");

        }

        S.Restart();
        //foreach ( Particle P1 in Particles )
        //{
        //    Particle_Integrate(P1);
        //}

        Parallel.ForEach(Particles, P1 =>
        {
            Particle_Integrate(P1);
        });
        X(S, "Integrating Particles:");


    }
    #endregion

    #region Particle
    public void Particle_SpawnBlock()
    {
        int C = 0;

        for ( int X = 0; X < P.ParticleBlockSizeX; X++ )
        {
            for ( int Y = 0; Y < P.ParticleBlockSizeY; Y++ )
            {
                for ( int Z = 0; Z < P.ParticleBlockSizeZ; Z++ )
                {
                    Particle NewP = new Particle();

                    Vector3 SpawnOffset = new Vector3(X * P.ParticleBlockGap, Y * P.ParticleBlockGap, Z * P.ParticleBlockGap);
                    Vector3 SpawnPosition = P.ParticleSpawnPoint + SpawnOffset;

                    NewP.Position = SpawnPosition;

                    Particles[C++] = NewP;
                }     
            }
        }
    }

    public void Particle_SetNeighborsBruteforce(Particle P1)
    {
        P1.Neighbors.Clear();

        foreach ( Particle P2 in Particles )
        {
            if (P1 == P2) { continue; }

            float DistanceSqr = (P1.Position - P2.Position).sqrMagnitude;

            if (DistanceSqr < P.NeighborRadius)
            {
                P1.Neighbors.Add(P2);
            }
        }
    }

    public void Particle_SetDensityPressure(Particle P1)
    {
        #region Density
        P1.Density = 0;

        foreach ( Particle P2 in P1.Neighbors )
        {
            if (P1 == P2) { continue; }

            if (Util.DistanceSquared(P1, P2) <= P.SmoothingRadiusSqr )
            {
                P1.Density += P.ParticleMass * K.Poly6(P1, P2);
            }
        }

        P1.Density = Mathf.Max(P1.Density, P.RestDensity);
        #endregion

        #region Pressure
        P1.Pressure = P.Stiffness * (P1.Density - P.RestDensity);
        #endregion
    }

    public void Particle_SetNetForce(Particle P1)
    {
        P1.NetForce = Vector3.zero;

        Vector3 PressureGradient  = Vector3.zero;
        Vector3 ViscosityGradient = Vector3.zero;

        foreach ( Particle P2 in P1.Neighbors )
        {
            if (P1 == P2) { continue; }

            PressureGradient += Physics_ComputePressureForce(P1, P2) * P.PressureMultiplier;

            ViscosityGradient += Physics_ComputeViscosityForce(P1, P2);
        }

        P1.NetForce += PressureGradient + ViscosityGradient + P.Gravity;

        // Plus external forces
    }

    public void Particle_EnforceBoundary(Particle P1)
    {
        if ( P1.Position.x < P.BoundarySize.x / -2 )
        {
            P1.Position.x = (P.BoundarySize.x / -2) + 0.001f;
            P1.Velocity.x = -P1.Velocity.x * P.BoundaryElasticity;
            P1.NetForce.x = 0;
        }
        else if ( P1.Position.x > P.BoundarySize.x / 2 )
        {
            P1.Position.x = (P.BoundarySize.x / 2) - 0.001f;
            P1.Velocity.x = -P1.Velocity.x * P.BoundaryElasticity;
            P1.NetForce.x = 0;
        }

        if ( P1.Position.y < P.BoundarySize.y / -2 )
        {
            P1.Position.y = (P.BoundarySize.y / -2) + 0.001f;
            P1.Velocity.y = (-P1.Velocity.y * P.BoundaryElasticity) + 1;
            P1.NetForce.y = 0;

            P1.Velocity += Vector3.up * 1; //Pushes particles slighlty upwards so they don't compress at the bottom
        }
        else if ( P1.Position.y > P.BoundarySize.y / 2 )
        {
            P1.Position.y = (P.BoundarySize.y / 2) - 0.001f;
            P1.Velocity.y = -P1.Velocity.y * P.BoundaryElasticity;
            P1.NetForce.y = 0;
        }

        if ( P1.Position.z < P.BoundarySize.z / -2 )
        {
            P1.Position.z = (P.BoundarySize.z / -2) + 0.001f;
            P1.Velocity.z = (-P1.Velocity.z * P.BoundaryElasticity) + 1;
            P1.NetForce.z = 0;

            P1.Velocity += Vector3.up * 1; //Pushes particles slighlty upwards so they don't compress at the bottom
        }
        else if ( P1.Position.z > P.BoundarySize.z / 2 )
        {
            P1.Position.z = (P.BoundarySize.z / 2) - 0.001f;
            P1.Velocity.z = -P1.Velocity.z * P.BoundaryElasticity;
            P1.NetForce.z = 0;
        }
    }

    public void Particle_Integrate(Particle P1)
    {
        Particle_EnforceBoundary(P1);

        //Try Vector3 ParticleAcceleration = P1.Force * P.ParticleMass; Replace P1.NetForce with PA

        P1.Velocity += 0.5f * (P1.PastAcceleration + P1.NetForce) * P.Timestep;

        Vector3 DeltaPosition = P1.Velocity * P.Timestep + 0.5f * P1.NetForce * P.Timestep * P.Timestep;

        P1.Position += DeltaPosition;
        P1.PastAcceleration = P1.NetForce;

    }
    #endregion

    #region Physics
    public Vector3 Physics_ComputePressureForce(Particle P1, Particle P2)
    {
        float Dividend = P1.Pressure + P2.Pressure;
        float Divisor  = 2 * P1.Density * P2.Density;

        return -P.ParticleMass * (Dividend / Divisor) * K.SpikyGrad(P1, P2);
    }    

    public Vector3 Physics_ComputeViscosityForce(Particle P1, Particle P2)
    {
        Vector3 DeltaVelocity = P1.Velocity - P2.Velocity;

        return -P.ParticleViscosity * DeltaVelocity * (P.ParticleMass / P1.Density) * K.Laplacian(P1, P2);
    }
    #endregion

    #region Rendering 
    private void OnDrawGizmos()
    {
        if ( Application.isPlaying )
        {
            foreach ( Particle P1 in Particles )
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(P1.Position, P.DrawRadius);
            }
        }
    }
    #endregion

    private void Update()
    {
        for ( int I = 0; I < P.SimIterations; I++ )
        {
            Simulation_Step();
        }
    }

}
