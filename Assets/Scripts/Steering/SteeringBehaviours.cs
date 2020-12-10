using System;
using UnityEngine;

[Serializable]
public struct SteeringParameters
{
    public Vector2 position;
    public Vector2 direction;

    public Vector2 linearVelocity; //should have been normalized
    public float angularVelocity;


    public float Orientation
    {
        get
        {
            float angleX = Mathf.Acos(direction.x);
            float angleY = Mathf.Acos(direction.y);

            if (angleY >= 0.0f)
                return angleX;
            else
                return angleX + Mathf.PI;
        }
        set
        {
            direction = new Vector2(Mathf.Cos(value), Mathf.Sin(value));
        }
    }
    //public Vector2 LinearVelocity { set { linearVelocity = value;  } }
}

public struct SteeringOutput 
{
    public Vector2 linearVelocity;
    public Vector2 angularVelocity;
}

public abstract class SteeringBehaviour
{
    public abstract SteeringOutput CalculateSteering(float deltaTime, SteeringParameters parameters);
    public void SetTarget(Vector2 target) { targetPosition = target; }

    protected Vector2 targetPosition = Vector2.zero;
}

public class Seek : SteeringBehaviour
{
    public override SteeringOutput CalculateSteering(float deltaTime, SteeringParameters parameters)
    {
        SteeringOutput steering = new SteeringOutput();

        steering.linearVelocity = targetPosition - parameters.position;
        steering.linearVelocity.Normalize();
        //velocity *= parameters.maxLinearSpeed;

        return steering;
    }
}

[Serializable]
public class Wander : SteeringBehaviour
{
    public override SteeringOutput CalculateSteering(float deltaTime, SteeringParameters parameters)
    {
        SteeringOutput steering = new SteeringOutput();

        Vector2 circleCenter = parameters.direction * distance;
        Vector2 displacement = new Vector2(Mathf.Cos(wanderAngle), Mathf.Sin(wanderAngle)) * radius;

        wanderAngle += UnityEngine.Random.value * angleChange - angleChange / 2.0f;

        steering.linearVelocity = circleCenter + displacement;

        return steering;
    }

    public float Distance { get { return distance; } }
    public float Radius { get { return radius; } }
    public float AngleChange { get { return angleChange; } }
    public float WanderAngle { set { wanderAngle = value; } }

    [SerializeField]
    [Min(0.0f)]
    float distance = 10.0f ;
    [SerializeField]
    [Min(0.0f)]
    float radius = 1.0f;
    [SerializeField]
    [Min(0.0f)]
    float angleChange = 0.05f;

    float wanderAngle = 0.0f;
}

public class Flee : Seek
{
    public override SteeringOutput CalculateSteering(float deltaTime, SteeringParameters parameters)
    {
        SteeringOutput steering = base.CalculateSteering(deltaTime, parameters);
        steering.linearVelocity *= -1;
        return steering;
    }
}

public class Evade : Flee
{
    public override SteeringOutput CalculateSteering(float deltaTime, SteeringParameters parameters)
    {
        SteeringOutput steering = new SteeringOutput();

        float squaredDist = (targetPosition - parameters.position).SqrMagnitude();

        if (squaredDist > (evadeRadius * evadeRadius))
            return steering;

        targetPosition = targetPosition + (parameters.linearVelocity * (squaredDist / (strenght * strenght)));
        
        return base.CalculateSteering(deltaTime, parameters);
    }

    float evadeRadius = 2.0f;
    float strenght = 1.0f;
}

public class Avoidance : SteeringBehaviour
{
    public override SteeringOutput CalculateSteering(float deltaTime, SteeringParameters parameters)
    {
        SteeringOutput steering = new SteeringOutput();

        Vector2 ahead = parameters.position + parameters.direction * maxSeeAhead;
        Vector2 ahead2 = parameters.position + parameters.direction * maxSeeAhead * 0.5f;
        Vector2 self = parameters.position;

        // the property "center" of the obstacle is a Vector3D.
        float evasionRadius = radius * radius;
        if ((targetPosition - ahead).SqrMagnitude() <= evasionRadius || (targetPosition - ahead2).SqrMagnitude() <= evasionRadius || (targetPosition - self).SqrMagnitude() <= evasionRadius)
        {
            steering.linearVelocity = (ahead - targetPosition).normalized;
        }
        return steering;
    }

    float radius = 5.0f;
    float maxSeeAhead = 2.0f;
}