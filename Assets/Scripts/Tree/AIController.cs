using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GrowingSpline))]
[DisallowMultipleComponent]
public class AIController : TreeController
{
    [SerializeField]
    Wander wander = new Wander(10.0f, 1.0f, 0.05f, Mathf.PI / 2.0f);
    [SerializeField]
    float weightWander = 1.2f;
    [SerializeField]
    Seek seek = new Seek();
    [SerializeField]
    GrowingSpline player = null;
    [SerializeField]
    float weightSeek = 0.2f;

    PrioritySteering behaviour = new PrioritySteering();

    [SerializeField]
    float rayOffset = 2.0f;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        if (!player)
            Debug.LogError("No player attached");

        behaviour.AddBehaviour(new BlendedSteering(new List<BlendedSteering.WeightedBehaviour>()
                {
                    new BlendedSteering.WeightedBehaviour(wander, weightWander)
                    //,
                    //new BlendedSteering.WeightedBehaviour(seek, weightSeek)
                }));
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (Avoided)
            wander.WanderAngle = spline.Orientation;

        SteeringParameters parameters = new SteeringParameters();
        parameters.position = spline.TopNodeWorld;
        parameters.linearVelocity = spline.LinearVelocity;
        parameters.Orientation = spline.Orientation; //Also sets direction

        seek.SetTarget(player.TopNodeWorld);

        SteeringOutput newDirection = behaviour.CalculateSteering(Time.deltaTime, parameters);
        spline.GrowthDirection = newDirection.linearVelocity;
    }

    protected override void OnDrawGizmosSelected() 
    {
        base.OnDrawGizmosSelected();

        if (!spline)
            return;

        SteeringParameters parameters = new SteeringParameters();
        parameters.position = spline.TopNodeWorld;
        parameters.linearVelocity = spline.LinearVelocity;
        parameters.Direction = spline.GrowthDirection;

        behaviour.Draw(parameters);
    }
}
