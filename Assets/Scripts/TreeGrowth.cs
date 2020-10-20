using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(SpriteShapeController))]
[DisallowMultipleComponent]
public class TreeGrowth : MonoBehaviour
{
    [Header("Manipulation settings")]
    [SerializeField]
    float sensitivity = 0.5f;
    [SerializeField]
    float growthSpeed = 0.0005f;
    [SerializeField] [Range(0.0f, 90.0f)]
    float maxAngleWidth = 60;

    float currentAngle = 90;
    Vector2 growthDirection = new Vector2(0.0f, 1.0f);

    [Header("Spline settings")]
    [SerializeField]
    float nodeInterval = 5.0f;
    [SerializeField]
    float curviness = 0.5f;

    float elapsedNewNodeTime = 0.0f;
    SpriteShapeController spriteController;

    public float NodeInterval 
    {
        get 
        {
            return nodeInterval;
        }
    }
    private Spline TreeSpline
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
            return TreeSpline.GetPointCount();
        }
    }

    // growDirection is a percentage value where -1 is left and 1 is right
    public void ChangeGrowthDirection(float force)
    {
        currentAngle = currentAngle + (sensitivity * force);
        currentAngle = Mathf.Clamp(currentAngle, 90 - maxAngleWidth, 90 + maxAngleWidth);
        growthDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * currentAngle), Mathf.Sin(Mathf.Deg2Rad * currentAngle));
    }

    //Resetting the tree
    void Start()
    {
        //Ressetting the splineController
        spriteController = GetComponent<SpriteShapeController>();


        //Clearing tree
        Spline tree = spriteController.spline;
        tree.Clear();

        InsertNode(new Vector2(transform.position.x, transform.position.y));
        InsertNode(new Vector2(transform.position.x, transform.position.y + 1));
        spriteController.transform.position = Vector3.zero;


        tree.SetRightTangent(0, growthDirection * curviness);
    }

    //Updating tree
    void Update()
    {
        int lastPointIndex = SplineCount - 1;

        //New position + tngent of last node
        Vector2 newPosition = GetPoint(SplineCount - 1);
        newPosition += (growthSpeed * growthDirection);
        TreeSpline.SetPosition(lastPointIndex, newPosition);
        TreeSpline.SetLeftTangent(lastPointIndex, -growthDirection * curviness);

        //Checking if new node is necessary
        elapsedNewNodeTime += Time.deltaTime;
        if (nodeInterval <= elapsedNewNodeTime) 
        {
            //Spawn new node
            //Split bezier curve in middle (end points)
            Vector2 p0, p1, p2, p3;
            GetCubicBezierCurvePoints(SplineCount - 1, out p0, out p1, out p2, out p3);

            Vector2 q0 = (p0 + p1) / 2.0f;
            Vector2 q1 = (p1 + p2) / 2.0f;
            Vector2 q2 = (p2 + p3) / 2.0f;

            Vector2 r0 = (q0 + q1) / 2.0f;
            Vector2 r1 = (q1 + q2) / 2.0f;

            Vector2 point = (r0 + r1) / 2.0f;

            //change tangent of p0 and p3
            TreeSpline.SetRightTangent(lastPointIndex - 1, q0 - p0);
            TreeSpline.SetLeftTangent(lastPointIndex, q2 - p3);

            InsertNode(lastPointIndex, point); //lastPointIndex is not last point anymore!
            TreeSpline.SetLeftTangent(lastPointIndex, r0 - point);
            TreeSpline.SetRightTangent(lastPointIndex, r1 - point);

            elapsedNewNodeTime -= nodeInterval;
        }
    }

    /// <summary>
    /// Get any node on the TreeSpline
    /// </summary>
    /// <param name="nodeIndex"></param>
    /// <returns></returns>
    public Vector2 GetPoint(int nodeIndex)
    {
        if (nodeIndex >= TreeSpline.GetPointCount())
        {
            Debug.Log("[TreeGrowth.GetPoint()] => Node index out of range");
            return Vector2.zero;
        }
        return TreeSpline.GetPosition(nodeIndex);
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
            endNodeIndex = SplineCount - 1;

        p0 = GetPoint(endNodeIndex - 1);
        p1 = (Vector2)TreeSpline.GetRightTangent(endNodeIndex - 1) + p0;
        p3 = GetPoint(endNodeIndex);
        p2 = (Vector2)TreeSpline.GetLeftTangent(endNodeIndex) + p3;
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

    void InsertNode(Vector2 position)
    {
        InsertNode(SplineCount, position);
    }
    void InsertNode(int index, Vector2 position) 
    {
        Spline tree = spriteController.spline;
        tree.InsertPointAt(index, position);
        tree.SetTangentMode(index, ShapeTangentMode.Continuous);
    }

    #region Gizmos
    private void OnDrawGizmos()
    {
        Vector3 temp = growthDirection;
        Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + temp, Color.green);

        if (spriteController) 
        {
            Vector2 p0, p1, p2, p3;
            GetCubicBezierCurvePoints(SplineCount - 1, out p0, out p1, out p2, out p3);

            Vector2 tangent = GetPointTangent(p0, p1, p2, p3, 1.0f);
            Vector2 normal = GetPointNormal(tangent);
            Vector2 point = GetPoint(p0, p1, p2, p3, 1.0f);
            Gizmos.DrawLine(point, point + tangent);
            Gizmos.DrawLine(point, point + normal);

            for (int node = 0; node < SplineCount; node++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(TreeSpline.GetPosition(node), 0.1f);
                Gizmos.color = Color.magenta;
                Vector3 nodePosition = TreeSpline.GetPosition(node);
                Gizmos.DrawLine(nodePosition, TreeSpline.GetLeftTangent(node) + nodePosition);
                Gizmos.DrawLine(nodePosition, TreeSpline.GetRightTangent(node) + nodePosition);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (spriteController)
        {
            Gizmos.color = Color.yellow;

            Vector2 p0, p1, p2, p3;
            GetCubicBezierCurvePoints(SplineCount - 1, out p0, out p1, out p2, out p3);

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
        }
    }
    #endregion
}


///Comments
///

//Vector2 GetPoint(Vector2 p0, Vector2 p1, float t) 
//{
//    return (1.0f - t) * p0 + t * p1; 
//}