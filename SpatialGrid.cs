using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatialGrid : MonoBehaviour
{
    public bool ShowScaling;
    public bool ShowHighlight;
    public bool ShowConvertedHighlight;
    public bool ShowKernel;

    public int BX;
    public int BY;
    public int BZ;

    public Vector3 Boundary;
    public Vector3 HalfBoundary;
    Vector3 BS;

    public Vector3 Index;

    private void OnDrawGizmos()
    {
        //
        HalfBoundary = Boundary / 2;
        //

        BS.x = Boundary.x / BX;
        BS.y = Boundary.y / BY;
        BS.z = Boundary.z / BZ;

        float S = 0;
        float SF = 1;

        if (ShowHighlight)
        {
            HighlightSelect();
        }
        
        if (ShowConvertedHighlight)
        {
            ConvertedHighlighSelect();
        }

        if (ShowKernel)
        {
            DrawKernel();
        }

        for ( int X = 0; X < BX; X++ )
        {
            for ( int Y = 0; Y < BY; Y++ )
            {
                for ( int Z = 0; Z < BZ; Z++ )
                {
                    float R = (1 / (float)BX) * X;
                    float G = (1 / (float)BY) * Y;
                    float B = (1 / (float)BZ) * Z;
                    Color C = new Color(R, G, B, 1f);

                    Gizmos.color = C;

                    Vector3 P = Vector3.zero;
                    P.x = X * BS.x;
                    P.y = Y * BS.y;
                    P.z = Z * BS.z;

                    P -= Boundary / 2;
                    P += BS / 2;

                    if (X == 0 && Y == 0 && Z == 0)
                    {
                        Gizmos.DrawCube(P, BS * SF);
                    }
                    else
                    {
                        Gizmos.DrawWireCube(P, BS * SF);
                    }

                    S++;
                    if (ShowScaling)
                    {
                        SF = 1 - (S / (BX * BY * BZ));
                    }
                    else
                    {
                        SF = 1;
                    }
                }
            }
        }
    }

    public void HighlightSelect()
    {
        int XI = Mathf.FloorToInt(Index.x);
        int YI = Mathf.FloorToInt(Index.y);
        int ZI = Mathf.FloorToInt(Index.z);

        Vector3 P = Vector3.zero;
        P.x = XI * BS.x;
        P.y = YI * BS.y;
        P.z = ZI * BS.z;
        P -= Boundary / 2;
        P += BS / 2;

        Gizmos.color = Color.green;
        Gizmos.DrawCube(P, BS);

        Gizmos.DrawWireSphere(Index, 0.1f);
    }

    public void ConvertedHighlighSelect()
    {
        #region X
        float CX = Index.x + (Boundary.x * 0.5f); //Converted X
        float DSX = CX / Boundary.x; //Divisor
        float DEX = 1f / BX; //Dividend
        int IX = Mathf.FloorToInt(DSX / DEX);
        #endregion

        #region Y
        float CY = Index.y + (Boundary.y * 0.5f); //Converted Y
        float DSY = CY / Boundary.y; //Divisor
        float DEY = 1f / BY; //Dividend
        int IY = Mathf.FloorToInt(DSY / DEY);
        #endregion

        #region Z
        float CZ = Index.z + (Boundary.z * 0.5f); //Converted Z
        float DSZ = CZ / Boundary.z; //Divisor
        float DEZ = 1f / BZ; //Dividend
        int IZ = Mathf.FloorToInt(DSZ / DEZ);
        #endregion

        Vector3 P = Vector3.zero;
        P.x = IX * BS.x;
        P.y = IY * BS.y;
        P.z = IZ * BS.z;
        P -= Boundary / 2;
        P += BS / 2;

        Gizmos.color = Color.green;
        Gizmos.DrawCube(P, BS);

        Gizmos.DrawWireSphere(Index, 0.1f);
    }

    public int PositionIndex(Vector3 Position)
    {
        float ConvertedX = Position.x + HalfBoundary.x;
        float DivisorX = ConvertedX / Boundary.x;
        float DividendX = 1f / BX;

        float ConvertedY = Position.y + HalfBoundary.y;
        float DivisorY = ConvertedY / Boundary.y;
        float DividendY = 1f / BY;

        float ConvertedZ = Position.z + HalfBoundary.z;
        float DivisorZ = ConvertedZ / Boundary.z;
        float DividendZ = 1f / BZ;

        int XIndex = Mathf.FloorToInt(DivisorX / DividendX);
        int YIndex = Mathf.FloorToInt(DivisorY / DividendY);
        int ZIndex = Mathf.FloorToInt(DivisorZ / DividendZ);

        return XIndex + BY * (YIndex + BZ * ZIndex);
    }

    public void DrawKernel()
    {
        int XI = Mathf.FloorToInt(Index.x);
        int YI = Mathf.FloorToInt(Index.y);
        int ZI = Mathf.FloorToInt(Index.z);

        for ( int X = XI-1; X <= XI+1; X++ )
        {
            for ( int Y = YI - 1; Y <= YI + 1; Y++ )
            {
                for ( int Z = ZI - 1; Z <= ZI + 1; Z++ )
                {
                    Vector3 P = Vector3.zero;
                    P.x = X * BS.x;
                    P.y = Y * BS.y;
                    P.z = Z * BS.z;
                    P -= Boundary / 2;
                    P += BS / 2;

                    Gizmos.color = new Color(1, 0.92f, 0.016f, 0.33f);
                    Gizmos.DrawCube(P, BS);
                }
            }
        }
    }
}
