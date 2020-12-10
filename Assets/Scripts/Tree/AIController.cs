using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GrowingSpline))]
[DisallowMultipleComponent]
public class AIController : MonoBehaviour
{
    [SerializeField]
    Avoidance avoidance = new Avoidance(5.0f, 2.0f, 10.0f);
    [SerializeField]
    float weightAvoidance = 1.0f;
    [SerializeField]
    Wander wander = new Wander(10.0f, 1.0f, 0.05f, Mathf.PI / 2.0f);
    [SerializeField]
    float weightWanderWhileAvoiding = 0.2f;
    [SerializeField]
    float weightWanderWhileSeeking = 1.2f;
    [SerializeField]
    Seek seek = new Seek();
    [SerializeField]
    GrowingSpline player = null;
    [SerializeField]
    float weightSeek = 0.2f;


    PrioritySteering behaviour = new PrioritySteering();

    GrowingSpline spline = null;
    [SerializeField]
    float rayOffset = 2.0f;


    // Start is called before the first frame update
    void Start()
    {
        if (!player)
            Debug.LogError("No player attached");

        spline = GetComponent<GrowingSpline>();

        behaviour.AddBehaviour(avoidance);

        behaviour.AddBehaviour(new BlendedSteering(new List<BlendedSteering.WeightedBehaviour>()
                {
                    new BlendedSteering.WeightedBehaviour(wander, weightWanderWhileSeeking),
                    new BlendedSteering.WeightedBehaviour(seek, weightSeek)
                }));
    }

    // Update is called once per frame
    void Update()
    {
        SteeringParameters parameters = new SteeringParameters();
        parameters.position = spline.TopNodeWorld;
        parameters.linearVelocity = spline.LinearVelocity;

        //RaycastHit2D hit = Physics2D.Raycast(spline.TopNodeWorld + (spline.GrowthDirection * rayOffset), spline.GrowthDirection);
        //avoidance.SetTarget(hit.point);

        if (spline.Orientation > Mathf.PI / 2.0f)
            avoidance.AngleOffset = -Mathf.Abs(avoidance.AngleOffset);
        else
            avoidance.AngleOffset = Mathf.Abs(avoidance.AngleOffset);

        seek.SetTarget(player.TopNodeWorld);

        SteeringOutput newDirection = behaviour.CalculateSteering(Time.deltaTime, parameters);
        spline.GrowthDirection = newDirection.linearVelocity;
    }

    void OnDrawGizmosSelected() 
    {
        if (!spline)
            return;

        SteeringParameters parameters = new SteeringParameters();
        parameters.position = spline.TopNodeWorld;
        parameters.linearVelocity = spline.LinearVelocity;
        parameters.Direction = spline.GrowthDirection;

        behaviour.Draw(parameters);
    }
}
