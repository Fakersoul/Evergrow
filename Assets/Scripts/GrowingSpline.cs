﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(SpriteShapeController))]

public class GrowingSpline : MonoBehaviour
{
    [Header("Growth settings")]
    [SerializeField]
    float nodeInterval = 5.0f;
    [SerializeField]
    float growthSpeed = 0.5f;
    [SerializeField]
    float curviness = 0.5f;

    [Header("Thickness settings")]
    [Min(1)]
    [SerializeField]
    int affectiveNodes = 1;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    float minThickness = 0.5f;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    float maxThickness = 1.0f;

    float elapsedNewNodeTime = 0.0f;

    SpriteShapeController spriteController;
    Vector2 growthDirection = new Vector2(0.0f, 0.0f);

    #region Getters and Setters
    //Pure Getters
    private Spline Spline 
    {
        get
        {
            return spriteController.spline;
        }
    }
    public int SplineCount
    {
        get
        {
            return Spline.GetPointCount();
        }
    }
    public int TopNodeIndex
    {
        get
        {
            return SplineCount - 1;
        }
    }
    public Vector2 TopNode 
    {
        get
        {
            return GetPoint(TopNodeIndex);
        }
    }


    public Vector2 GrowthDirection 
    {
        get 
        {
            return growthDirection;
        }
        set 
        {
            growthDirection = value;
        }
    }
    public float GrowthSpeed 
    {
        get 
        {
            return growthSpeed;
        }
        set 
        {
            growthSpeed = value;
        }
    }
    #endregion Getters and Setters

    #region Spline Functions
    #region GetFunctions
    /// <summary>
    /// Get any node on the TreeSpline
    /// </summary>
    /// <param name="nodeIndex"></param>
    /// <returns></returns>
    public Vector2 GetPoint(int nodeIndex)
    {
        if (nodeIndex >= Spline.GetPointCount())
        {
            Debug.Log("[TreeGrowth.GetPoint()] => Node index out of range");
            return Vector2.zero;
        }
        return Spline.GetPosition(nodeIndex);
    }

    public Vector2 GetPointWorldPos(int nodeIndex) 
    {
        return transform.localToWorldMatrix.MultiplyPoint(GetPoint(nodeIndex));
    }

    /// <summary>
    /// Get the 4 points that define a cubic bezier curve. Part of the spline of the tree.
    /// </summary>
    /// <param name="endNodeIndex">End point index</param>
    /// <param name="p0">start point</param>
    /// <param name="p1">control point (start)</param>
    /// <param name="p2">control point (end)</param>
    /// <param name="p3">end point</param>
    public void GetCubicBezierCurvePoints(int endNodeIndex, out Vector2 p0, out Vector2 p1, out Vector2 p2, out Vector2 p3)
    {
        if (endNodeIndex <= 0)
            endNodeIndex = 1;
        else if (endNodeIndex >= SplineCount)
            endNodeIndex = TopNodeIndex;

        p0 = GetPoint(endNodeIndex - 1);
        p1 = (Vector2)Spline.GetRightTangent(endNodeIndex - 1) + p0;
        p3 = GetPoint(endNodeIndex);
        p2 = (Vector2)Spline.GetLeftTangent(endNodeIndex) + p3;
    }

    /// <summary>
    /// Get any point along the cubic curve
    /// </summary>
    /// <param name="p0">start point</param>
    /// <param name="p1">control point (start)</param>
    /// <param name="p2">control point (end)</param>
    /// <param name="p3">end point</param>
    /// <param name="t">percentage value</param>
    /// <returns>The point on the cubic curve</returns>
    public Vector2 GetPoint(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        return (Mathf.Pow(1.0f - t, 3.0f) * p0) + (3.0f * Mathf.Pow(1.0f - t, 2.0f) * t * p1) + (3.0f * (1 - t) * Mathf.Pow(t, 2.0f) * p2) + (Mathf.Pow(t, 3.0f) * p3);
    }

    Vector2 GetPointDeriative(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        return
            Mathf.Pow(1.0f - t, 2.0f) * 3 * (p1 - p0) +
            2 * (1.0f - t) * t * 3 * (p2 - p1) +
            Mathf.Pow(t, 2.0f) * 3 * (p3 - p2);
    }

    Vector2 GetPointTangent(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        return GetPointDeriative(p0, p1, p2, p3, t).normalized;
    }

    /// <summary>
    /// Get the normal of any point along the cubic curve
    /// </summary>
    /// <param name="p0">start point</param>
    /// <param name="p1">control point (start)</param>
    /// <param name="p2">control point (end)</param>
    /// <param name="p3">end point</param>
    /// <param name="t">percentage value</param>
    /// <param name="leftSide">Left or Right side of the spline</param>
    /// <returns>The normal of a point on the cubic curve</returns>
    public Vector2 GetPointNormal(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t, bool leftSide = true)
    {
        return GetPointNormal(GetPointTangent(p0, p1, p2, p3, t), leftSide);
    }
    Vector2 GetPointNormal(Vector2 tangent, bool leftSide = true)
    {
        return Quaternion.Euler(0, 0, ((leftSide ? 1.0f : -1.0f) * 90.0f)) * tangent;
    }
    #endregion GetFunctions

    #region InsertFunctions
    void InsertNode(Vector2 position)
    {
        InsertNode(SplineCount, position);
    }

    void InsertNode(int index, Vector2 position, Vector2 leftTangent, Vector2 rightTangent)
    {
        InsertNode(index, position);
        Spline.SetLeftTangent(index, leftTangent);
        Spline.SetRightTangent(index, rightTangent);
    }

    void InsertNode(int index, Vector2 position, Vector2 leftTangent, Vector2 rightTangent, float height)
    {
        InsertNode(index, position, leftTangent, rightTangent);
        Spline.SetHeight(index, height);
    }

    void InsertNode(int index, Vector2 position)
    {
        Spline.InsertPointAt(index, position);
        Spline.SetTangentMode(index, ShapeTangentMode.Continuous);
    }
    #endregion InsertFunctions
    #endregion Spline Functions

    // Start is called before the first frame update
    void Start()
    {
        spriteController = GetComponent<SpriteShapeController>();

        for (int node = 0; node < SplineCount; node++)
        {
            Spline.SetTangentMode(node, ShapeTangentMode.Continuous);
        }

        //Set min thickness for top of spline
        Spline.SetHeight(TopNodeIndex, minThickness);
    }

    // Update is called once per frame
    void Update()
    {
        //New position + tangent of last node
        Vector2 newPosition = TopNode + (growthSpeed * growthDirection * Time.deltaTime);
        Spline.SetPosition(TopNodeIndex, newPosition);
        Spline.SetLeftTangent(TopNodeIndex, -growthDirection * curviness);

        //New node 
        {
            elapsedNewNodeTime += Time.deltaTime;
            if (nodeInterval <= elapsedNewNodeTime)
            {
                //Spawn new node
                //Split bezier curve in middle (end points)
                GetCubicBezierCurvePoints(TopNodeIndex, out Vector2 p0, out Vector2 p1, out Vector2 p2, out Vector2 p3);

                Vector2 q0 = (p0 + p1) / 2.0f;
                Vector2 q1 = (p1 + p2) / 2.0f;
                Vector2 q2 = (p2 + p3) / 2.0f;

                Vector2 r0 = (q0 + q1) / 2.0f;
                Vector2 r1 = (q1 + q2) / 2.0f;

                Vector2 point = (r0 + r1) / 2.0f;

                //change tangent of p0 and p3
                Spline.SetRightTangent(TopNodeIndex - 1, q0 - p0);
                Spline.SetLeftTangent(TopNodeIndex, q2 - p3);

                //change the thickness of previous last node node to max
                Spline.SetHeight(TopNodeIndex - 1, maxThickness);

                InsertNode(TopNodeIndex, point, r0 - point, r1 - point, (maxThickness + minThickness) / 2.0f);

                elapsedNewNodeTime -= nodeInterval;
            }
        }

        //Thickness of the tree
        {
            float newHeight = Mathf.Lerp((maxThickness + minThickness) / 2.0f, maxThickness, elapsedNewNodeTime / nodeInterval);
            Spline.SetHeight(TopNodeIndex - 1, newHeight);
        }
    }

    private void OnDrawGizmosSelected() 
    {
        Vector3 temp = growthDirection;
        Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + temp, Color.green);

        if (spriteController) 
        {
            Gizmos.color = Color.yellow;
            GetCubicBezierCurvePoints(TopNodeIndex, out Vector2 p0, out Vector2 p1, out Vector2 p2, out Vector2 p3);

            p0 += (Vector2)transform.position;
            p1 += (Vector2)transform.position;
            p2 += (Vector2)transform.position;
            p3 += (Vector2)transform.position;

            Gizmos.DrawLine(p0, p1);
            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawLine(p2, p3);

            Gizmos.color = Color.blue;
            Vector2 q0 = (p0 + p1) / 2.0f;
            Vector2 q1 = (p1 + p2) / 2.0f;
            Vector2 q2 = (p2 + p3) / 2.0f;
            Gizmos.DrawLine(q0, q1);
            Gizmos.DrawLine(q1, q2);

            Gizmos.color = Color.green;
            Vector2 r0 = (q0 + q1) / 2.0f;
            Vector2 r1 = (q1 + q2) / 2.0f;
            Gizmos.DrawLine(r0, r1);

            for (int node = 0; node < SplineCount; node++)
            {
                Gizmos.color = Color.red;
                Vector3 nodePosition = transform.localToWorldMatrix.MultiplyPoint(Spline.GetPosition(node));


                Gizmos.DrawSphere(nodePosition, 0.025f);
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(nodePosition, Spline.GetLeftTangent(node) + nodePosition);
                Gizmos.DrawLine(nodePosition, Spline.GetRightTangent(node) + nodePosition);
            }
        }
    }
}


///Comments
///

//Vector2 GetPoint(Vector2 p0, Vector2 p1, float t) 
//{
//    return (1.0f - t) * p0 + t * p1; 
//}