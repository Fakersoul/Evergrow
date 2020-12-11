using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(GrowingSpline))]
[DisallowMultipleComponent]
public class PlayerController : TreeController
{
    [SerializeField]
    float sensitivity = 0.05f;

    // growDirection is a percentage value where -1 is left and 1 is right
    public void ChangeGrowthDirection(float force)
    {
        spline.Orientation = spline.Orientation + (sensitivity * force);
    }
}
