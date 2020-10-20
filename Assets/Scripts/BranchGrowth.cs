using BranchColonization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(SpriteShapeController))]
[DisallowMultipleComponent]
public class BranchGrowth : MonoBehaviour
{ 
    List<Attractor> attractors;

    [SerializeField]
    float attractionRadius = 1.0f;
    [SerializeField]
    float attractorKillDistance = 0.5f;
    [SerializeField]
    float growthSpeed = 1.0f;
    [SerializeField]
    float probablility = 1.0f;

    SpriteShapeController spriteController;
    Vector2 growthDirection = Vector2.zero;

    public Vector2 Direction
    {
        set 
        {
            growthDirection = value;
        }
    }

    private Spline BranchSpline
    {
        get
        {
            return spriteController.spline;
        }
    }
    public int SplineCount
    {
        get
        {
            return BranchSpline.GetPointCount();
        }
    }

    public void GenerateAttractors(int amount, float width, float height) 
    {
        if (attractors == null)
            attractors = new List<Attractor>();
        else if (attractors.Count != 0) 
            attractors.Clear();

        Vector2 startingPos = gameObject.transform.position;
        for (uint attractor = 0; attractor < amount; attractor++)
        {
            Vector2 position = new Vector2(Random.Range(startingPos.x, startingPos.x + width), Random.Range(startingPos.y, startingPos.y + height));
            attractors.Add(new Attractor(position, attractor));
        }
    }

    void Start()
    {
        spriteController = GetComponent<SpriteShapeController>();

        BranchSpline.SetRightTangent(0, growthDirection);
        BranchSpline.SetLeftTangent(1, -growthDirection);

        //GameObject newBranch = Instantiate(gameObject);
        //newBranch.GetComponent<BranchGrowth>().attractors = attractors;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 newPosition = GetPoint(SplineCount - 1);
        newPosition += (growthSpeed * growthDirection);
        BranchSpline.SetPosition(SplineCount - 1, newPosition);
        BranchSpline.SetLeftTangent(SplineCount - 1, -growthDirection);
    }

    /// <summary>
    /// Get any node on the BranchSpline
    /// </summary>
    /// <param name="nodeIndex"></param>
    /// <returns></returns>
    public Vector2 GetPoint(int nodeIndex)
    {
        if (nodeIndex >= BranchSpline.GetPointCount())
        {
            Debug.Log("[TreeGrowth.GetPoint()] => Node index out of range");
            return Vector2.zero;
        }
        return BranchSpline.GetPosition(nodeIndex);
    }

    private void OnDrawGizmosSelected()
    {
        foreach (Attractor attractor in attractors) 
        {
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(attractor.position, 0.05f);
        }
    }
}
