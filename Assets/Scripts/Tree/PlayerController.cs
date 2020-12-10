using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(GrowingSpline))]
[DisallowMultipleComponent]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float sensitivity = 0.5f;

    GrowingSpline spline;

    // growDirection is a percentage value where -1 is left and 1 is right
    public void ChangeGrowthDirection(float force)
    {
        spline.Orientation = spline.Orientation + (sensitivity * force);
    }

    void Start()
    {
        spline = GetComponent<GrowingSpline>();
    }

    private void OnDrawGizmosSelected()
    {
        
    }
}


