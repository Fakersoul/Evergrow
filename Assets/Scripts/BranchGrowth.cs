using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BranchColonization
{
    public struct Attractor
    {
        Vector2 position;
        uint index;
    }
}

public class BranchGrowth : MonoBehaviour
{
    List<BranchColonization.Attractor> attractors = new List<BranchColonization.Attractor>();

    [SerializeField]
    float speed = 1.0f;

    [SerializeField]
    float probablility = 1.0f;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
