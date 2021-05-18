using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parameters : MonoBehaviour
{
    #region Spawn Block
    public Vector3 ParticleSpawnPoint = new Vector3(0, 0, 0);
    public int ParticleBlockSizeX = 1;
    public int ParticleBlockSizeY = 1;
    public int ParticleBlockSizeZ = 1;
    public int ParticleBlockSize = 0;
    public float ParticleBlockGap = 1;
    #endregion

    #region Simulation Parameters
    public float Timestep = 1;
    public float Stiffness = 50; //?
    public float BoundaryElasticity = 0.5f;
    public int SolverIterations = 1;
    public int SimIterations = 1;
    public Vector3 BoundarySize = new Vector3(0, 0, 0);
    public Vector3 Gravity = new Vector3(0, -9.81f, 0);
    #endregion

    #region Particle Parameters
    public float ParticleMass = 20f;
    public float ParticleViscosity = 0.6f;
    #endregion

    #region Pressure Parameters
    public float PressureMultiplier = 1f;
    public float SmoothingRadius = 0.7f;
    public float SmoothingRadiusSqr = 0f;
    public float NeighborRadius = 1.4f;
    public float RestDensity = 82f;
    #endregion

    #region Rendering Parameters
    public float DrawRadius = 0.1f;
    #endregion

    #region Tree Parameters
    public int MaxPointsPerLeaf = 32;
    //Higher = Faster Construction, Slower Query
    //Lower  = Slower Construction, Faster Query
    #endregion

    private void Start()
    {
        SmoothingRadiusSqr = SmoothingRadius * SmoothingRadius;

        ParticleBlockSize = ParticleBlockSizeX * ParticleBlockSizeY * ParticleBlockSizeZ;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(Vector3.zero, BoundarySize);

        if ( !Application.isPlaying )
        {
            for ( int X = 0; X < ParticleBlockSizeX; X++ )
            {
                for ( int Y = 0; Y < ParticleBlockSizeY; Y++ )
                {
                    for ( int Z = 0; Z < ParticleBlockSizeZ; Z++ )
                    {
                        Vector3 SpawnOffset = new Vector3(X * ParticleBlockGap, Y * ParticleBlockGap, Z * ParticleBlockGap);
                        Vector3 SpawnPosition = ParticleSpawnPoint + SpawnOffset;

                        Gizmos.color = Color.white;
                        Gizmos.DrawWireSphere(SpawnPosition, DrawRadius);
                    }
                }
            }
        }
    }
}
