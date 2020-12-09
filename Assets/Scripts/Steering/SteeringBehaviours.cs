using System;
using UnityEngine;

public struct SteeringParameters
{
    public Vector2 position;
    public Vector2 linearVelocity; //should have been normalized

    //public float orientation;
    //public float currentLinearSpeed;

    //float angularVelocity;
    //float maxAngularSpeed;    
}
    //public float maxLinearSpeed; -> Is currently used in growing spline should go out of it

public abstract class SteeringBehaviour
{
    public abstract Vector2 CalculateSteering(float deltaTime, SteeringParameters parameters);
    public void SetTarget(Vector2 target) { targetPosition = target; }

    protected Vector2 targetPosition = Vector2.zero;
}

public class Seek : SteeringBehaviour
{
    public override Vector2 CalculateSteering(float deltaTime, SteeringParameters parameters)
    {
        Vector2 velocity;

        velocity = targetPosition - parameters.position;
        velocity.Normalize();
        //velocity *= parameters.maxLinearSpeed;

        return velocity;
    }
}

[Serializable]
public class Wander : Seek
{
    public override Vector2 CalculateSteering(float deltaTime, SteeringParameters parameters)
    {
        float halfJitter = maxJitterOffset / 2.0f;
        Vector2 randomOffset = new Vector2(UnityEngine.Random.Range(-halfJitter, halfJitter), UnityEngine.Random.Range(-halfJitter, halfJitter));

        wanderTarget += randomOffset;
        wanderTarget.Normalize();
        wanderTarget *= radius;

        Vector2 addOffset = parameters.linearVelocity;
        addOffset *= distance;

        targetPosition = parameters.position + addOffset + wanderTarget;

        return base.CalculateSteering(deltaTime, parameters);
    }

    public float Distance { get { return distance; } set { distance = value; } } 
    public float Radius { get { return radius; } set { radius = value; } } 
    public float MaxJitterOffset { get { return maxJitterOffset; } set { maxJitterOffset = value; } } 

    [SerializeField]
    protected float distance = 6.0f ;
    [SerializeField]
    protected float radius = 1.0f;
    [SerializeField]
    protected float maxJitterOffset = 0.1f;
    protected Vector2 wanderTarget = new Vector2(0, 1);
}