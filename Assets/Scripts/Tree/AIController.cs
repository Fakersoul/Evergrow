using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GrowingSpline))]
[DisallowMultipleComponent]
public class AIController : MonoBehaviour
{
    [SerializeField]
    Wander wanderBehaviour = new Wander();

    GrowingSpline spline = null;

    // Start is called before the first frame update
    void Start()
    {
        spline = GetComponent<GrowingSpline>();
        wanderBehaviour.WanderAngle = Mathf.PI / 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        SteeringParameters parameters = new SteeringParameters();
        parameters.position = spline.TopNodeWorld;
        parameters.linearVelocity = spline.LinearVelocity;

        SteeringOutput newDirection = wanderBehaviour.CalculateSteering(Time.deltaTime, parameters);
        //newDirection.x = Mathf.Clamp(newDirection.x, -Mathf.Cos(maxAngleWidth), Mathf.Cos(maxAngleWidth));
        spline.GrowthDirection = newDirection.linearVelocity;
    }

    void OnDrawGizmosSelected() 
    {
        if (spline)
        {
            Vector2 worldSplineTopNode = spline.TopNodeWorld;
            Vector2 circleCenter = worldSplineTopNode + (spline.GrowthDirection * wanderBehaviour.Distance);
            Gizmos.DrawLine(worldSplineTopNode, circleCenter);
            Gizmos.DrawWireSphere(circleCenter, wanderBehaviour.Radius);
        }
    }
}
