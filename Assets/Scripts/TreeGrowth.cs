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
    int splineCount = 0;
    SpriteShapeController spriteController;

    public Vector2 GrowthDirection
    {
        get
        {
            return growthDirection; 
        }
    }
    public Vector2 TopOfTree
    {
        get 
        {
            return spriteController.spline.GetPosition(splineCount - 1);
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
        Spline tree = spriteController.spline;

        int lastPointIndex = splineCount - 1;

        //New position + tngent of last node
        Vector2 newPosition = TopOfTree;
        newPosition += (growthSpeed * growthDirection);
        tree.SetPosition(lastPointIndex, newPosition);
        tree.SetLeftTangent(lastPointIndex, -growthDirection * curviness);

        //Checking if new node is necessary
        elapsedNewNodeTime += Time.deltaTime;
        if (nodeInterval <= elapsedNewNodeTime) 
        {
            //Spawn new node
            //Split bezier curve in middle (end points)
            Vector2 p0 = tree.GetPosition(lastPointIndex - 1);
            Vector2 p1 = (Vector2) tree.GetRightTangent(lastPointIndex - 1) + p0;
            Vector2 p3 = tree.GetPosition(lastPointIndex);
            Vector2 p2 = (Vector2) tree.GetLeftTangent(lastPointIndex) + p3;

            Vector2 q0 = (p0 + p1) / 2.0f;
            Vector2 q1 = (p1 + p2) / 2.0f;
            Vector2 q2 = (p2 + p3) / 2.0f;

            Vector2 r0 = (q0 + q1) / 2.0f;
            Vector2 r1 = (q1 + q2) / 2.0f;

            Vector2 point = (r0 + r1) / 2.0f;

            //change tangent of p0 and p3
            tree.SetRightTangent(lastPointIndex - 1, q0 - p0);
            tree.SetLeftTangent(lastPointIndex, q2 - p3);

            InsertNode(lastPointIndex, point); //lastPointIndex is not last point anymore!
            tree.SetLeftTangent(lastPointIndex, r0 - point);
            tree.SetRightTangent(lastPointIndex, r1 - point);

            elapsedNewNodeTime -= nodeInterval;
        }
    }

    void InsertNode(Vector2 position)
    {
        InsertNode(splineCount, position);
    }
    void InsertNode(int index, Vector2 position) 
    {
        Spline tree = spriteController.spline;
        tree.InsertPointAt(index, position);
        tree.SetTangentMode(index, ShapeTangentMode.Continuous);
        splineCount++;
    }

    #region Gizmos
    private void OnDrawGizmos()
    {
        Vector3 temp = growthDirection;
        Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + temp, Color.green);

        if (spriteController) 
        {
            Spline tree = spriteController.spline;

            Gizmos.color = Color.yellow;
            Vector2 p0 = tree.GetPosition(splineCount - 1 - 1);
            Vector2 p1 = (Vector2)tree.GetRightTangent(splineCount - 1 - 1) + p0;
            Vector2 p3 = tree.GetPosition(splineCount - 1);
            Vector2 p2 = (Vector2)tree.GetLeftTangent(splineCount - 1) + p3;
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

            for (int node = 0; node < splineCount; node++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(tree.GetPosition(node), 0.1f);
                Gizmos.color = Color.magenta;
                Vector3 nodePosition = tree.GetPosition(node);
                Gizmos.DrawLine(nodePosition, tree.GetLeftTangent(node) + nodePosition);
                Gizmos.DrawLine(nodePosition, tree.GetRightTangent(node) + nodePosition);
            }


        }


    }

    private void OnDrawGizmosSelected()
    {
        
    }
    #endregion
}
