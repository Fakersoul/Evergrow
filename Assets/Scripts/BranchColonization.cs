using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchColonization : MonoBehaviour
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

    [SerializeField]
    BranchGrowthController branchGrowthController;

    [SerializeField]
    GameObject branch;

    public class Attractor
    {
        public Attractor(Vector2 position, int index)
        {
            this.Position = position;
            this.Index = index;
        }

        public Vector2 Position { get; }
        public int Index { get; }

        public Node AttractingNode { get; set; } = null;
        public Node PreviousAttractingNode { get; set; } = null;
    }

    public class Node
    {
        public Node(BranchGrowthController branch) 
        {
            Branch = branch;
        }

        public Node(Node other) 
        {
            this.Position = other.Position;
            this.Branch = other.Branch;
        }

        public Vector2 Position { get ; set ; }
        public BranchGrowthController Branch { get; } = null;
        public List<Attractor> InfluencingAttractors { get; } = new List<Attractor>();
    }

    //All the attractors
    List<Attractor> attractors = new List<Attractor>();
    [ReadOnly]
    List<Node> nodes = new List<Node>();

    public void GenerateAttractors(int amount, float width, float height)
    {
        if (attractors == null)
            attractors = new List<Attractor>();
        else if (attractors.Count != 0)
            attractors.Clear();

        for (int attractorIndex = 0; attractorIndex < amount; attractorIndex++)
        {
            Vector2 position = new Vector2(Random.Range(-width / 2.0f, width / 2.0f), Random.Range(offSetAttractors, offSetAttractors + height)); //TODO not good
            attractors.Add(new Attractor(position, attractorIndex));
        }
    }

    void Start()
    {
        if (!branchGrowthController)
            Debug.LogError("Insert the branch gameobject with the controller");

        if (!branch)
            Debug.LogError("Insert the branch gameobject");
           

        nodes.Add(branchGrowthController.BranchTopNode);
        
        //Square values 
        attractionRadius *= attractionRadius;
        attractorKillDistance *= attractorKillDistance;
        maxDistance *= maxDistance;

        //GameObject newBranch = Instantiate(gameObject);
        //newBranch.GetComponent<BranchGrowth>().attractors = attractors;
    }

    void Update()
    {
        //Clear influecing nodes!
        foreach (Node node in nodes) 
        {
            node.InfluencingAttractors.Clear();
        }

        List<int> attractorRemovalIndexes = new List<int>();
        List<Node> addedNodes = new List<Node>(); //All new nodes
        
        //Debug.Log("Amount of attractors [" + attractors.Count + "], Anmount of nodes [" + nodes.Count + "]", gameObject);

        //Check every attractor on nearby nodes
        for (int attractorIndex = 0; attractorIndex < attractors.Count; attractorIndex++)
        {
            Attractor attractor = attractors[attractorIndex];
            attractor.AttractingNode = null;
 
            //Check if there was an previous distance
            float shortestDistanceSquared = float.MaxValue;
            if (attractor.PreviousAttractingNode != null)
            {
                shortestDistanceSquared = (attractor.Position - attractor.PreviousAttractingNode.Position).sqrMagnitude;
                //Debug.Log("Previous node found! Tested distance squared= " + shortestDistanceSquared, gameObject);
            }

            bool removed = false;

            //Get closest node -> Influencing nodes           
            for (int nodeIndex = 0; nodeIndex < nodes.Count; nodeIndex++)
            {
                Node node = nodes[nodeIndex];
                float newDistanceSquared = (attractor.Position - node.Position).SqrMagnitude();
                //Debug.Log("New distance towards attractor" + newDistanceSquared, gameObject);

                //Check if attractor needs to be removed
                if (newDistanceSquared <= attractorKillDistance) 
                {
                    //Debug.Log("Attractor will be removed", gameObject);
                    attractorRemovalIndexes.Add(attractorIndex);
                    removed = true;
                    break;
                }

                //Check if attractor is in attraction radius
                if (newDistanceSquared <= attractionRadius)
                {
                    if (shortestDistanceSquared >= newDistanceSquared)
                    {
                        //Debug.Log("Attractor will attract node", gameObject);
                        shortestDistanceSquared = newDistanceSquared;

                        if (attractor.AttractingNode != null)
                            attractor.AttractingNode.InfluencingAttractors.Remove(attractor);

                        attractor.AttractingNode = node;
                        node.InfluencingAttractors.Add(attractor);
                    }
                }
            }

            if (!removed)
            {
                //If there was no new node found (could be because of a smaller distance) but there was a previous valid node
                if (attractor.AttractingNode == null && attractor.PreviousAttractingNode != null)
                {
                    Debug.Log("New branch", gameObject);
                    GameObject newBranch = Instantiate(branch, gameObject.transform.position + (Vector3)attractor.PreviousAttractingNode.Position, new Quaternion(), attractor.PreviousAttractingNode.Branch.transform);


                    GrowingSpline branchSpline = newBranch.GetComponent<GrowingSpline>();
                    
                    branchSpline.GrowthDirection = (attractor.Position - attractor.PreviousAttractingNode.Position).normalized;
                    branchSpline.SpriteShape = branchGrowthController.gameObject.GetComponent<GrowingSpline>().SpriteShape;


                    BranchGrowthController newBranchController = newBranch.GetComponent<BranchGrowthController>();

                    addedNodes.Add(newBranchController.BranchTopNode);
                    newBranchController.NodeOffset = attractor.PreviousAttractingNode.Position;
                }
            }
            else 
            {
                attractor.AttractingNode = null; // node got deleted! Needs to be here otherwise next frame a new branch would appear
            }

            
            //Set previous node for next frame
            if (attractor.AttractingNode != null)
            {
                attractor.PreviousAttractingNode = new Node(attractor.AttractingNode);
            }
            else
            {
                attractor.PreviousAttractingNode = null;
            }
        }

        //Remove the attractors that need to be removed
        foreach (int removalIndex in attractorRemovalIndexes)
        {
            attractors.RemoveAt(removalIndex);
        }

        //Add all new nodes
        foreach (Node node in addedNodes)
        {
            nodes.Add(node);
        }
    }

    private void OnDrawGizmosSelected()
    {
        foreach (Attractor attractor in attractors)
        {
            Gizmos.color = Color.white;
            Vector3 position = transform.localToWorldMatrix.MultiplyPoint(attractor.Position);

            Gizmos.DrawSphere(position, 0.05f);
        }
    }
}

//Comments



//Vector2 direction = Vector2.zero;
//foreach (Attractor attractor in influencingAttractors)
//{
//    direction += (attractor.Position - spline.TopNode);
//}
//direction.Normalize();
//direction = new Vector2(Mathf.Lerp(spline.GrowthDirection.x, direction.x, sensitivity), Mathf.Lerp(spline.GrowthDirection.y, direction.y, sensitivity));
//spline.GrowthDirection = direction;