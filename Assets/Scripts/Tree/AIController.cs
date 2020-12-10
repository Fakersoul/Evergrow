using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GrowingSpline))]
[DisallowMultipleComponent]
public class AIController : MonoBehaviour
{
    [SerializeField]
    Wander wanderBehaviour = new Wander();

    [SerializeField]
    [Range(0.0f, 90.0f)]
    float maxAngleWidth = 60;

    GrowingSpline spline = null;

    // Start is called before the first frame update
    void Start()
    {
        spline = GetComponent<GrowingSpline>();
    }

    // Update is called once per frame
    void Update()
    {
        SteeringParameters parameters = new SteeringParameters();
        parameters.position = spline.TopNode;
        parameters.linearVelocity = spline.GrowthDirection;

        Vector2 newDirection = wanderBehaviour.CalculateSteering(Time.deltaTime, parameters);
        newDirection.x = Mathf.Clamp(newDirection.x, -Mathf.Cos(maxAngleWidth), Mathf.Cos(maxAngleWidth));
        spline.GrowthDirection = newDirection;
    }

    void OnDrawGizmosSelected() 
    {
        if (spline)
        {
            Vector2 worldSplineTopNode = spline.TopNode + (Vector2)transform.position;
            Vector2 circleCenter = worldSplineTopNode + (spline.GrowthDirection * wanderBehaviour.Distance);
            Gizmos.DrawLine(worldSplineTopNode, circleCenter);
            Gizmos.DrawWireSphere(circleCenter, wanderBehaviour.Radius);
        
        }
    }
}
