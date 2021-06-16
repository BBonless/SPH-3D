using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrueSpatialGrid
{
    public int BucketsX;
    public int BucketsY;
    public int BucketsZ;
    public int BucketsCount;

    private Vector3 BucketSize;

    private Vector3 Boundary;
    private Vector3 HalfBoundary;

    private float NeighborRadiusSqr;

    public List<Particle>[] Buckets;

    public TrueSpatialGrid(Vector3 BoundaryIn, int BXIn, int BYIn, int BZIn, float NeighborRadiusIn)
    {
        BucketsX = BXIn;
        BucketsY = BYIn;
        BucketsZ = BZIn;
        BucketsCount = BucketsX * BucketsY * BucketsZ;

        BucketSize.x = Boundary.x / BucketsX;
        BucketSize.y = Boundary.y / BucketsY;
        BucketSize.z = Boundary.z / BucketsZ;

        Boundary = BoundaryIn;
        HalfBoundary = BoundaryIn * 0.5f;

        NeighborRadiusSqr = NeighborRadiusIn * NeighborRadiusIn;

        Buckets = new List<Particle>[BucketsCount];

        for ( int Bucket = 0; Bucket < BucketsCount; Bucket++ )
        {
            Buckets[Bucket] = new List<Particle>();
        }
    }

    public void DrawSpatialGrid()
    {
        for ( int X = 0; X < BucketsX; X++ )
        {
            for ( int Y = 0; Y < BucketsY; Y++ )
            {
                for ( int Z = 0; Z < BucketsZ; Z++ )
                {
                    float R = (1 / (float)BucketsX) * X;
                    float G = (1 / (float)BucketsY) * Y;
                    float B = (1 / (float)BucketsZ) * Z;
                    Color C = new Color(R, G, B, 0.2f);

                    Gizmos.color = C;

                    Vector3 CellPosition = Vector3.zero;
                    CellPosition.x = X * BucketSize.x;
                    CellPosition.y = Y * BucketSize.y;
                    CellPosition.z = Z * BucketSize.z;

                    CellPosition -= Boundary / 2;
                    CellPosition += BucketSize / 2;

                    Gizmos.DrawWireCube(CellPosition, BucketSize);
                }
            }
        }
    }

    public int F3D(int X, int Y, int Z) //Flatten 3D Coordinates to 1D Index
    {
        return X + BucketsY * (Y + BucketsZ * Z);
    }

    public int PositionIndex1D( Vector3 Position )
    {
        Position.x = Mathf.Clamp(Position.x, -HalfBoundary.x + 0.01f, HalfBoundary.x - 0.01f);
        Position.y = Mathf.Clamp(Position.y, -HalfBoundary.y + 0.01f, HalfBoundary.y - 0.01f);
        Position.z = Mathf.Clamp(Position.z, -HalfBoundary.z + 0.01f, HalfBoundary.z - 0.01f);

        float ConvertedX = Position.x + HalfBoundary.x;
        float DivisorX = ConvertedX / Boundary.x;
        float DividendX = 1f / BucketsX;

        float ConvertedY = Position.y + HalfBoundary.y;
        float DivisorY = ConvertedY / Boundary.y;
        float DividendY = 1f / BucketsY;

        float ConvertedZ = Position.z + HalfBoundary.z;
        float DivisorZ = ConvertedZ / Boundary.z;
        float DividendZ = 1f / BucketsZ;

        int XIndex = Mathf.FloorToInt(DivisorX / DividendX);
        int YIndex = Mathf.FloorToInt(DivisorY / DividendY);
        int ZIndex = Mathf.FloorToInt(DivisorZ / DividendZ);

        return F3D(XIndex, YIndex, ZIndex);
    }

    public int[] PositionIndex3D( Vector3 Position )
    {
        Position.x = Mathf.Clamp(Position.x, -HalfBoundary.x + 0.01f, HalfBoundary.x - 0.01f);
        Position.y = Mathf.Clamp(Position.y, -HalfBoundary.y + 0.01f, HalfBoundary.y - 0.01f);
        Position.z = Mathf.Clamp(Position.z, -HalfBoundary.z + 0.01f, HalfBoundary.z - 0.01f);

        float ConvertedX = Position.x + HalfBoundary.x;
        float DivisorX = ConvertedX / Boundary.x;
        float DividendX = 1f / BucketsX;

        float ConvertedY = Position.y + HalfBoundary.y;
        float DivisorY = ConvertedY / Boundary.y;
        float DividendY = 1f / BucketsY;

        float ConvertedZ = Position.z + HalfBoundary.z;
        float DivisorZ = ConvertedZ / Boundary.z;
        float DividendZ = 1f / BucketsZ;

        int[] Result = new int[3];
        Result[0] = Mathf.FloorToInt(DivisorX / DividendX);
        Result[1] = Mathf.FloorToInt(DivisorY / DividendY);
        Result[2] = Mathf.FloorToInt(DivisorZ / DividendZ);

        return Result;
    }

    public void Construct(Particle[] Particles)
    {
        foreach(Particle P in Particles)
        {
            int Index = PositionIndex1D(P.Position);

            Buckets[Index].Add(P);
        }
    }

    public void Clear()
    {
        foreach(List<Particle> Bucket in Buckets)
        {
            Bucket.Clear();
        }
    }

    public void NeighborQuery(Particle Target)
    {
        Target.Neighbors.Clear();

        int[] Index = PositionIndex3D(Target.Position);

        for ( int X = Index[0] - 1; X <= Index[0] + 1; X++ )
        {
            if (X < 0 || X > BucketsX - 1) { continue; }
            for ( int Y = Index[1] - 1; Y <= Index[1] + 1; Y++ )
            {
                if ( Y < 0 || Y > BucketsY - 1 ) { continue; }
                for ( int Z = Index[2] - 1; Z <= Index[2] + 1; Z++ )
                {
                    if ( Z < 0 || Z > BucketsZ - 1 ) { continue; }

                    List<Particle> Bucket = Buckets[F3D(X, Y, Z)];

                    foreach (Particle P in Bucket)
                    {
                        if (Vector3.SqrMagnitude(P.Position - Target.Position) < NeighborRadiusSqr)
                        {
                            Target.Neighbors.Add(P);
                        }
                    }
                }
            }
        }
    }
}
