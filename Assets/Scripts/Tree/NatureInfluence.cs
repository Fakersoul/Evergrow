using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(GrowingSpline))]
public class NatureInfluence : MonoBehaviour
{
    List<ParticleCollisionEvent> rainCollisions = new List<ParticleCollisionEvent>();

    GrowingSpline spline = null;

    // Start is called before the first frame update
    void Start()
    {
        spline = GetComponent<GrowingSpline>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnParticleCollision(GameObject other)
    {

        ParticleSystem part = other.GetComponent<ParticleSystem>();
        if (!part)
            Debug.LogError("No particle system found");

        int numCollisionEvents = part.GetCollisionEvents(gameObject, rainCollisions);

        Debug.Log(numCollisionEvents);

        for (int i = 0; i < numCollisionEvents; i++)
        {

        }
    }

    void OnDrawGizmosSelected()
    {

    }
}
