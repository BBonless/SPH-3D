using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle
{
    #region Position
    public Vector3      Position            = Vector3.zero;
    public Vector3      PastPosition        = Vector3.zero;
    #endregion

    #region Motion
    public Vector3      Velocity            = Vector3.zero;
    public Vector3      PastAcceleration    = Vector3.zero;
    public Vector3      NetForce            = Vector3.zero;
    #endregion

    #region Physical Properties
    public float        Pressure            = 0;
    public float        Density             = 0;
    #endregion

    public List<Particle> Neighbors = new List<Particle>();
}
