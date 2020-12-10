using UnityEngine;

//Spline is being transformed later

[RequireComponent(typeof(GrowingSpline))]
[DisallowMultipleComponent]
public class BranchController : MonoBehaviour
{
    //[SerializeField]
    //float avoidanceDistance = 0.2f;

    [SerializeField]
    float maxDistance = 2.0f;

    //All the nodes

    public BranchColonization.Node BranchTopNode { get; private set; } = null;
    public Vector2 NodeOffset { get { return nodeOffset; } set { nodeOffset = value; } }

    GrowingSpline spline = null;
    Vector2 nodeOffset = Vector2.zero;

    private void Awake()
    {
        BranchTopNode = new BranchColonization.Node(this);
    }

    void Start()
    {
        spline = GetComponent<GrowingSpline>();
        //collider = GetComponent<EdgeCollider2D>(); //TODO not safe
    }

    // Update is called once per frame
    void Update()
    {
        BranchTopNode.Position = spline.TopNode + nodeOffset;

        if (BranchTopNode.Position.sqrMagnitude > maxDistance)
        {
            spline.enabled = false;
        }

        if (BranchTopNode.InfluencingAttractors.Count == 0)
            return;

        Vector2 direction = Vector2.zero;
        foreach (BranchColonization.Attractor attractor in BranchTopNode.InfluencingAttractors)
        {
            direction += (attractor.Position - BranchTopNode.Position);
        }
        direction.Normalize();
        //direction = new Vector2(Mathf.Lerp(spline.GrowthDirection.x, direction.x, sensitivity), Mathf.Lerp(spline.GrowthDirection.y, direction.y, sensitivity));
        spline.GrowthDirection = direction;
    }

    private void OnDrawGizmosSelected()
    {
        foreach (BranchColonization.Attractor attractor in BranchTopNode.InfluencingAttractors)
        {
            Gizmos.color = Color.blue;
            Vector3 position = transform.localToWorldMatrix.MultiplyPoint(attractor.Position);

            Gizmos.DrawSphere(position, 0.05f);
        }
    }
}

//COMMENTS

//RaycastHit2D[] hits = Physics2D.RaycastAll(spline.GetPointWorldPos(spline.TopNodeIndex), spline.GrowthDirection, avoidanceDistance);
//foreach (RaycastHit2D hit in hits) 
//{
//    if (hit.collider == collider)
//        continue;
//    else
//        if (hit)
//            Debug.Log("Hit");
//}




//public void GenerateAttractors(int amount, float width, float height)
//{
//    if (attractors == null)
//        attractors = new List<Attractor>();
//    else if (attractors.Count != 0)
//        attractors.Clear();

//    for (int attractorIndex = 0; attractorIndex < amount; attractorIndex++)
//    {
//        Vector2 position = new Vector2(Random.Range(offSetAttractors, offSetAttractors + width), Random.Range(-height / 4.0f, 3.0f * (height / 4.0f))); //TODO not good
//        attractors.Add(new Attractor(position));
//    }
//}

////Not going to work with branches on branches
//Attractor[] InfluencingAttractors() 
//{
//    List<Attractor> influencingAttractors = new List<Attractor>();
//    for (int index = 0; index < attractors.Count; index++)
//    {
//        Attractor attractor = attractors[index];
//        attractor.DistanceNearestNode = (attractor.Position - spline.TopNode).SqrMagnitude();

//        if (attractor.DistanceNearestNode <= attractionRadius) 
//        {
//            influencingAttractors.Add(attractor);
//        }
//    }
//    return influencingAttractors.ToArray();
//}

//void KillAttractors(Attractor[] influencingAttractors) 
//{
//    foreach (Attractor attractor in influencingAttractors)
//    {
//        if (attractor.DistanceNearestNode <= attractorKillDistance) 
//        {
//            attractors.Remove(attractor);
//        }
//    }
//}


//Check if max distance is achieved
//if ((spline.GetPointWorldPos(spline.TopNodeIndex) - (Vector2)transform.position).sqrMagnitude > maxDistance)
//{
//    spline.enabled = false;
//    this.enabled = false;
//    attractors.Clear();
//}  

//Check infuencing Attractors


//Attractor[] influencingAttractors = InfluencingAttractors();
//if (influencingAttractors.Length == 0)
//    return;


//Check if moving away from certain attractor?
//Yes, Create new Branch and delete influencingAttractor (add to new branch)
//No, Move to the attractors

//    Vector2 direction = Vector2.zero;
//    foreach (Attractor attractor in influencingAttractors) 
//    {
//        direction += (attractor.Position - spline.TopNode);
//    }
//    direction.Normalize();
//    direction = new Vector2(Mathf.Lerp(spline.GrowthDirection.x, direction.x, sensitivity), Mathf.Lerp(spline.GrowthDirection.y, direction.y, sensitivity));
//    spline.GrowthDirection = direction;

//    //Delete all attractors in killing range
//    KillAttractors(influencingAttractors);
//}