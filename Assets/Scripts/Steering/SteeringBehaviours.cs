using System;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

[Serializable]
public struct SteeringParameters
{
    public Vector2 position;

    public Vector2 linearVelocity;
    public float angularVelocity;
    
    Vector2 direction;
    float orientation;

    public Vector2 Direction 
    {
        get 
        {
            return direction;
        }
        set 
        {
            direction = value;

            //Setting orientation
            float angleX = Mathf.Acos(direction.x);
            float angleY = Mathf.Acos(direction.y);

            if (angleY >= 0.0f)
                orientation = angleX;
            else
                orientation = angleX + Mathf.PI;
        }
    }

    public float Orientation
    {
        get
        {
            return orientation;
        }
        set
        {
            orientation = value;
            //Setting direction
            direction = new Vector2(Mathf.Cos(value), Mathf.Sin(value));
        }
    }
}

public struct SteeringOutput 
{
    public Vector2 linearVelocity;
    public float angularVelocity;

    public bool succesful;
}

[Serializable]
public abstract class SteeringBehaviour
{
    public abstract SteeringOutput CalculateSteering(float deltaTime, SteeringParameters parameters);
    public void SetTarget(Vector2 target) { targetPosition = target; }
    public virtual void Draw(SteeringParameters parameters) { }

    protected Vector2 targetPosition = Vector2.zero;
}

[Serializable]
public class Seek : SteeringBehaviour
{
    public override SteeringOutput CalculateSteering(float deltaTime, SteeringParameters parameters)
    {
        SteeringOutput steering = new SteeringOutput();

        steering.linearVelocity = targetPosition - parameters.position;
        steering.linearVelocity.Normalize();
        steering.succesful = true;
        //velocity *= parameters.maxLinearSpeed;

        return steering;
    }

    public override void Draw(SteeringParameters parameters)
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(parameters.position, 0.2f);
        Gizmos.DrawSphere(targetPosition, 0.2f);
        Gizmos.color = Color.white;
        Gizmos.DrawLine(parameters.position, targetPosition);
    }
}

[Serializable]
public class Wander : SteeringBehaviour
{
    public Wander(float distance, float radius, float angleChange, float wanderAngle) 
    {
        this.distance = distance;
        this.radius = radius;
        this.angleChange = angleChange;
        this.wanderAngle = wanderAngle;
    }

    public override SteeringOutput CalculateSteering(float deltaTime, SteeringParameters parameters)
    {
        SteeringOutput steering = new SteeringOutput();

        Vector2 circleCenter = parameters.Direction * distance;
        Vector2 displacement = new Vector2(Mathf.Cos(wanderAngle), Mathf.Sin(wanderAngle)) * radius;

        wanderAngle += UnityEngine.Random.value * angleChange - angleChange / 2.0f;

        steering.linearVelocity = circleCenter + displacement;
        steering.succesful = true;

        return steering;
    }

    public override void Draw(SteeringParameters parameters)
    {
        Vector2 circleCenter = parameters.Direction * distance + parameters.position;
        
        Gizmos.color = Color.white;
        Gizmos.DrawLine(parameters.position, circleCenter);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(circleCenter, radius);
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
    [SerializeField]
    [Min(0.0f)]
    float wanderAngle = 0.0f;
}

[Serializable]
public class Flee : Seek
{
    public override SteeringOutput CalculateSteering(float deltaTime, SteeringParameters parameters)
    {
        SteeringOutput steering = base.CalculateSteering(deltaTime, parameters);
        steering.linearVelocity *= -1;
        return steering;
    }
}

[Serializable]
public class Evade : Flee
{
    public Evade(float evadeRadius, float strenght) 
    {
        this.evadeRadius = evadeRadius;
        this.strenght = strenght;
    }

    public override SteeringOutput CalculateSteering(float deltaTime, SteeringParameters parameters)
    {
        SteeringOutput steering = new SteeringOutput();

        float squaredDist = (targetPosition - parameters.position).SqrMagnitude();

        if (squaredDist > (evadeRadius * evadeRadius))
            return steering;

        targetPosition = targetPosition + (parameters.linearVelocity * (squaredDist / (strenght * strenght)));
        
        return base.CalculateSteering(deltaTime, parameters);
    }

    [SerializeField]
    [Min(0.0f)]
    float evadeRadius = 2.0f;
    [SerializeField]
    float strenght = 1.0f;
}

[Serializable]
public class Avoidance : SteeringBehaviour
{
    public Avoidance(float radius, float maxSeeAhead, float angleOffset) 
    {
        this.radius = radius;
        this.maxSeeAhead = maxSeeAhead;
        this.angleOffset = angleOffset;
    }

    public override SteeringOutput CalculateSteering(float deltaTime, SteeringParameters parameters)
    {
        SteeringOutput steering = new SteeringOutput();

        float angleRadians = angleOffset * Mathf.Deg2Rad;
        Vector2 newDirection = parameters.Direction.x * new Vector2(Mathf.Cos(angleRadians), Mathf.Sin(angleRadians)) + parameters.Direction.y * new Vector2(-Mathf.Sin(angleRadians), Mathf.Cos(angleRadians));

        Vector2 ahead = parameters.position + (newDirection * maxSeeAhead);
        Vector2 ahead2 = parameters.position + (newDirection * maxSeeAhead * 0.5f);
        Vector2 self = parameters.position;

        // the property "center" of the obstacle is a Vector3D.
        float evasionRadius = radius * radius;
        if ((targetPosition - ahead).SqrMagnitude() <= evasionRadius || (targetPosition - ahead2).SqrMagnitude() <= evasionRadius || (targetPosition - self).SqrMagnitude() <= evasionRadius)
        {
            steering.linearVelocity = (ahead - targetPosition).normalized;
            steering.succesful = true;
        }
        return steering;
    }

    public override void Draw(SteeringParameters parameters)
    {
        float angleRadians = angleOffset * Mathf.Deg2Rad;
        Vector2 newDirection = parameters.Direction.x * new Vector2(Mathf.Cos(angleRadians), Mathf.Sin(angleRadians)) + parameters.Direction.y * new Vector2(-Mathf.Sin(angleRadians), Mathf.Cos(angleRadians));

        Vector2 ahead = parameters.position + (newDirection * maxSeeAhead);
        Vector2 ahead2 = parameters.position + (newDirection * maxSeeAhead * 0.5f);

        Gizmos.DrawWireSphere(targetPosition, radius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(ahead, 0.2f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(ahead2, 0.2f);
    }

    public float AngleOffset { get { return angleOffset; } set { angleOffset = value; } }

    [SerializeField]
    [Min(0.0f)]
    float radius = 5.0f;
    [SerializeField]
    [Min(0.0f)]
    float maxSeeAhead = 2.0f;
    [SerializeField]
    [Range(0.0f, 90.0f)]
    float angleOffset = 10.0f;
}