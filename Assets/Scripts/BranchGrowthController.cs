using BranchColonization;
using System.Collections.Generic;
using UnityEngine;

//Spline is being transformed later

[RequireComponent(typeof(GrowingSpline))]
[DisallowMultipleComponent]
public class BranchGrowthController : MonoBehaviour
{
    [SerializeField]
    float offSetAttractors = 1.0f;
    [SerializeField]
    float sensitivity = 0.5f;

    //Values become squared in start
    [SerializeField]
    float attractionRadius = 1.0f;
    [SerializeField]
    float attractorKillDistance = 0.3f;

    [SerializeField]
    float avoidanceDistance = 0.2f;

    [SerializeField]
    float maxDistance = 2.0f;


    List<Attractor> attractors;
    GrowingSpline spline;
    EdgeCollider2D collider;

    public void GenerateAttractors(int amount, float width, float height)
    {
        if (attractors == null)
            attractors = new List<Attractor>();
        else if (attractors.Count != 0)
            attractors.Clear();

        for (int attractorIndex = 0; attractorIndex < amount; attractorIndex++)
        {
            Vector2 position = new Vector2(Random.Range(offSetAttractors, offSetAttractors + width), Random.Range(-height / 4.0f, height / 2.0f)); //TODO not good
            attractors.Add(new Attractor(position, attractorIndex));
        }
    }

    //Not going to work with branches on branches
    Attractor[] InfluencingAttractors(Vector2 position) 
    {
        List<Attractor> influencingAttractors = new List<Attractor>();
        for (int index = 0; index < attractors.Count; index++)
        {
            Attractor attractor = attractors[index];
            attractor.Distance = (attractor.Position - position).SqrMagnitude();

            if (attractor.Distance <= attractionRadius) 
            {
                influencingAttractors.Add(attractor);
            }
        }
        return influencingAttractors.ToArray();
    }
    void KillAttractors(Attractor[] influencingAttractors) 
    {
        foreach (Attractor attractor in influencingAttractors)
        {
            if (attractor.Distance <= attractorKillDistance) 
            {
                attractors.Remove(attractor);
            }
        }
    }

    void Start()
    {
        spline = GetComponent<GrowingSpline>();
        collider = GetComponent<EdgeCollider2D>(); //TODO not safe

        //Square values 
        attractionRadius *= attractionRadius;
        attractorKillDistance *= attractorKillDistance;
        maxDistance *= maxDistance;

        //BranchSpline.SetRightTangent(0, growthDirection);
        //BranchSpline.SetLeftTangent(1, -growthDirection);

        //GameObject newBranch = Instantiate(gameObject);
        //newBranch.GetComponent<BranchGrowth>().attractors = attractors;
    }

    // Update is called once per frame
    void Update()
    {
        if ((spline.GetPointWorldPos(spline.TopNodeIndex) - (Vector2)transform.position).sqrMagnitude > maxDistance)
        {
            spline.enabled = false;
            this.enabled = false;
            attractors.Clear();
        }  


        //RaycastHit2D[] hits = Physics2D.RaycastAll(spline.GetPointWorldPos(spline.TopNodeIndex), spline.GrowthDirection, avoidanceDistance);
        //foreach (RaycastHit2D hit in hits) 
        //{
        //    if (hit.collider == collider)
        //        continue;
        //    else
        //        if (hit)
        //            Debug.Log("Hit");

        //}


        Attractor[] influencingAttractors = InfluencingAttractors(spline.TopNode);
        if (influencingAttractors.Length == 0)
            return;



        Vector2 averagePosition = Vector2.zero;
        foreach (Attractor attractor in influencingAttractors) 
        {
            averagePosition += attractor.Position;
        }
        averagePosition /= influencingAttractors.Length;
        Vector2 newDirection = averagePosition - spline.TopNode;
        newDirection.Normalize();
        newDirection = new Vector2(Mathf.Lerp(spline.GrowthDirection.x, newDirection.x, sensitivity), Mathf.Lerp(spline.GrowthDirection.y, newDirection.y, sensitivity));

        spline.GrowthDirection = newDirection;

        KillAttractors(influencingAttractors);
    }

    private void OnDrawGizmosSelected()
    {
        if (attractors != null)
            foreach (Attractor attractor in attractors) 
            {
                Gizmos.color = Color.white;
                Vector3 position = transform.localToWorldMatrix.MultiplyPoint(attractor.Position);
                
                Gizmos.DrawSphere(position, 0.05f);
            }

        Gizmos.DrawRay(spline.GetPointWorldPos(spline.TopNodeIndex), spline.GrowthDirection * avoidanceDistance);
        
    }
}
