using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GrowingSpline))]
public class TreeController : MonoBehaviour
{
    [Header("Avoidance with rays")]
    [SerializeField]
    float maxAhead = 2.0f;
    [SerializeField]
    int amountRays = 5;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    float steeringSensitivity = 0.3f;


    protected bool Avoided { get; private set; }

    Ray2D[] rays = new Ray2D[0];

    protected GrowingSpline spline = null;
    protected EdgeCollider2D splineCollider = null;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        spline = GetComponent<GrowingSpline>();
        splineCollider = GetComponent<EdgeCollider2D>();

        rays = new Ray2D[amountRays];
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        //Update rays
        for (int rayIndex = 0; rayIndex < rays.Length; rayIndex++)
        {
            rays[rayIndex].origin = spline.TopNodeWorld;
            float angleRay = spline.Orientation + (Mathf.PI / 2.0f) - (rayIndex * (Mathf.PI / (amountRays - 1)));
            rays[rayIndex].direction = new Vector2(Mathf.Cos(angleRay), Mathf.Sin(angleRay));
        }


        //Detect collision
        splineCollider.enabled = false;
        Avoided = false;
        Vector2 direction = Vector2.zero;
        foreach (Ray2D ray in rays)
        {
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, maxAhead);
            if (hit) 
            {
                float steeringMultiplier = Mathf.Lerp(steeringSensitivity, 1.0f, 1.0f - ((hit.point - ray.origin).sqrMagnitude / (maxAhead * maxAhead)));
                float angle = Vector2.SignedAngle(ray.direction, spline.GrowthDirection);
                if (angle < 0.0f) //left
                {
                    direction += Vector2.right * steeringMultiplier;
                }
                else if (angle > 0.0f) //right
                {
                    direction += Vector2.left * steeringMultiplier;
                }
                else //middle 
                {
                    if (hit.point.x >= spline.TopNodeWorld.x)
                        direction += Vector2.right * steeringMultiplier;
                    else
                        direction += Vector2.left * steeringMultiplier;
                }
                Avoided = true;
            }
        }
        spline.GrowthDirection += direction;
        splineCollider.enabled = true;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (spline)
            Gizmos.DrawWireSphere(spline.TopNodeWorld, maxAhead);
        if (rays.Length > 0)
            foreach (Ray2D ray in rays)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(ray.origin, ray.origin + (ray.direction * maxAhead));
            }
    }
}
