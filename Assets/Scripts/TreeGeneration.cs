using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(SpriteShapeController))]
[DisallowMultipleComponent]
public class TreeGeneration : MonoBehaviour
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


    void Start()
    {
        //Ressetting the splineController
        spriteController = GetComponent<SpriteShapeController>();
        Spline tree = spriteController.spline;

        tree.Clear();
        tree.InsertPointAt(0, new Vector2(0, 0));
        tree.InsertPointAt(1, new Vector2(0, 1));
        tree.SetTangentMode(0, ShapeTangentMode.Continuous);
        tree.SetRightTangent(0, growthDirection * curviness);
        tree.SetTangentMode(1, ShapeTangentMode.Continuous);
        splineCount = tree.GetPointCount();
    }

    void Update()
    {
        Spline tree = spriteController.spline;

        Vector2 newPosition = GetTopofTree();
        newPosition += (growthSpeed * growthDirection);
        tree.SetPosition(splineCount - 1, newPosition);
        tree.SetLeftTangent(splineCount - 1, -growthDirection * curviness);

        //Checking if new node is necessary
        elapsedNewNodeTime += Time.deltaTime;
        if (nodeInterval <= elapsedNewNodeTime) 
        {
            elapsedNewNodeTime -= nodeInterval;
            tree.SetRightTangent(splineCount - 1, growthDirection * curviness);
            tree.InsertPointAt(splineCount, newPosition + new Vector2(0.0f, 0.1f));
            tree.SetTangentMode(splineCount, ShapeTangentMode.Continuous);
            splineCount++;
        }
    }

    // growDirection is a percentage value where -1 is left and 1 is right
    public void ChangeGrowthDirection(float force) 
    {
        currentAngle = currentAngle + (sensitivity * force);
        currentAngle = Mathf.Clamp(currentAngle, 90 - maxAngleWidth, 90 + maxAngleWidth);
        growthDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * currentAngle), Mathf.Sin(Mathf.Deg2Rad * currentAngle));
    }

    public Vector2 GetTopofTree() 
    {
        Vector2 topOfTree = spriteController.spline.GetPosition(splineCount - 1);
        return topOfTree;
    }

    #region Gizmos
    private void OnDrawGizmos()
    {
        Vector3 temp = growthDirection;
        Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + temp, Color.green);

        for (int node = 0; node < splineCount; node++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(spriteController.spline.GetPosition(node), 0.1f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        
    }
    #endregion
}
