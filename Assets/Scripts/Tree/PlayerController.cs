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
        float currentAngle = spline.Orientation + (sensitivity * force);
        
        spline.GrowthDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * currentAngle), Mathf.Sin(Mathf.Deg2Rad * currentAngle));
    }

    //Resetting the tree
    void Start()
    {
        spline = GetComponent<GrowingSpline>();

        //spline.Clear();
        //spline.InsertNode(new Vector2(transform.position.x, transform.position.y));
        //spline.InsertNode(new Vector2(transform.position.x, transform.position.y + 1));
        //transform.position = Vector3.zero;
        //spline.SetRightTangentTREEGROWTH(0);
    }

    //void Start()
    //{
    //    //Clearing tree
    //    TreeSpline.Clear();

    //    InsertNode(new Vector2(transform.position.x, transform.position.y));
    //    InsertNode(new Vector2(transform.position.x, transform.position.y + 1));
    //    spriteController.transform.position = Vector3.zero;

    //    TreeSpline.SetRightTangent(0, growthDirection * curviness);
    //}



    //#region Gizmos
    //private void OnDrawGizmos()
    //{

    //}

    private void OnDrawGizmosSelected()
    {
        
    }
    //#endregion
}


