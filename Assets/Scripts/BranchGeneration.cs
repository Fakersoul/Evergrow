using UnityEngine;

namespace BranchColonization
{
    struct Attractor
    {
        public Attractor(Vector2 position, uint index)
        {
            this.position = position;
            this.index = index;
        }

        public readonly Vector2 position;
        public readonly uint index;
    }
}

[RequireComponent(typeof(TreeGrowth))]
public class BranchGeneration : MonoBehaviour
{
    [SerializeField]
    GameObject branch;
    TreeGrowth tree;

    [Header("Branch Settings")]
    [SerializeField]
    [Range(0.0f, 100.0f)]
    float probabilityNewBranch = 10.0f;
    [SerializeField]
    readonly float timerNewBranch = 5.0f;

    float elapsedNewBranch = 0.0f;
    float addedprobability = 0.0f;

    //[SerializeField]
    //[Tooltip("The diameter of the branch crown")]
    //float diameter = 1;
    //[SerializeField]
    //[Range(0.0f, 1.0f)]
    //[Tooltip("The position of the diameter (left=0.0, right=1.0)")]
    //float diameterPosition = 0.75f;

    [SerializeField]
    int amountAttractors = 20;

    // Start is called before the first frame update
    void Start()
    {
        tree = GetComponent<TreeGrowth>();
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
                int nodeIndex = Random.Range(1, tree.SplineCount);

                Vector2 p0, p1, p2, p3;
                tree.GetCubicBezierCurvePoints(nodeIndex, out p0, out p1, out p2, out p3);

                float percentageValue = Random.Range(0.0f, 1.0f);


                GameObject newBranch = Instantiate(branch, tree.GetPoint(p0, p1, p2, p3, percentageValue), Quaternion.identity, gameObject.transform);
                BranchGrowth branchGrowthComponent = newBranch.GetComponent<BranchGrowth>();

                //Tangents are set in the branch growth

                bool boolean = (Random.value > 0.5f);
                branchGrowthComponent.Direction = tree.GetPointNormal(p0, p1, p2, p3, percentageValue, boolean);
                branchGrowthComponent.GenerateAttractors(amountAttractors, 5.0f, 5.0f);
                addedprobability = 0.0f;
            }
            //Reset timer
            elapsedNewBranch -= timerNewBranch;
        }
    }
}
