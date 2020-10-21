using BranchColonization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(GrowingSpline))]
[DisallowMultipleComponent]
public class BranchGrowthController : MonoBehaviour
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

    GrowingSpline spline;

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
        spline = GetComponent<GrowingSpline>();

        //BranchSpline.SetRightTangent(0, growthDirection);
        //BranchSpline.SetLeftTangent(1, -growthDirection);

        //GameObject newBranch = Instantiate(gameObject);
        //newBranch.GetComponent<BranchGrowth>().attractors = attractors;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDrawGizmosSelected()
    {
        if (attractors != null)
            foreach (Attractor attractor in attractors) 
            {
                Gizmos.color = Color.white;
                Gizmos.DrawSphere(attractor.position, 0.05f);
            }
    }
}
