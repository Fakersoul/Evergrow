using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

[RequireComponent(typeof(GrowingSpline))]
public class BranchGeneration : MonoBehaviour
{
    [SerializeField]
    GameObject branch = null;
    GrowingSpline tree = null;

    [Header("Branch Settings")]
    [SerializeField]
    [Range(0.0f, 100.0f)]
    float probabilityNewBranch = 10.0f;
    [SerializeField]
    float timerNewBranch = 5.0f;
    [SerializeField]
    int includingNodes = 2;


    float elapsedNewBranch = 0.0f;
    float addedprobability = 0.0f;

    //[SerializeField]
    //[Tooltip("The diameter of the branch crown")]
    //float diameter = 1;
    //[SerializeField]
    //[Range(0.0f, 1.0f)]
    //[Tooltip("The position of the diameter (left=0.0, right=1.0)")]
    //float diameterPosition = 0.75f;

    [Header("Space Colonization")]
    [SerializeField]
    int amountAttractors = 20;
    [SerializeField]
    float width = 6.0f;
    [SerializeField]
    float height = 6.0f;

    // Start is called before the first frame update
    void Start()
    {
        tree = GetComponent<GrowingSpline>();

        if (!branch.GetComponentInChildren<BranchController>())
            Debug.LogError("Branch object not correctly setup");

        if (!branch)
        {
            Debug.LogError("No branch prototype for the tree");
        }
    }

    // Update is called once per frame
    void Update()
    {
        elapsedNewBranch += Time.deltaTime;
        if (elapsedNewBranch > timerNewBranch) 
        {
            //Check if new branch
            float chance = Random.Range(0.0f, 100.0f);
            if (chance < (addedprobability += probabilityNewBranch)) 
            {
                int nodeIndex = Random.Range(tree.SplineCount - includingNodes, tree.SplineCount);

                Vector2 p0, p1, p2, p3;
                tree.GetCubicBezierCurvePoints(nodeIndex, out p0, out p1, out p2, out p3);

                float percentageValue = Random.Range(0.0f, 1.0f);

                bool isLeft = (Random.value > 0.5f);

                Vector3 branchPosition = tree.GetPoint(p0, p1, p2, p3, percentageValue);
                branchPosition.z = 0.01f; //Needs to be behind the tree
                //branchSpline.GrowthDirection = tree.GetPointNormal(p0, p1, p2, p3, percentageValue, false); //TODO get the angle
                GameObject newBranch = Instantiate(branch, branchPosition + transform.position, Quaternion.Euler(0.0f, 0.0f, isLeft ? 90.0f : -90.0f), gameObject.transform);

                BranchColonization colonization = newBranch.GetComponent<BranchColonization>();
                colonization.GenerateAttractors(amountAttractors, width, height);
                
                GrowingSpline branchSpline = newBranch.GetComponentInChildren<GrowingSpline>();
                branchSpline.GrowthDirection = new Vector2(0.0f, 1.0f);
                branchSpline.SpriteShape = tree.SpriteShape;

                //Tangents are set in the branch growth

                addedprobability = 0.0f;
            }
            //Reset timer
            elapsedNewBranch -= timerNewBranch;
        }
    }
}
