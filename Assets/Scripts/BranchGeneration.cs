using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchGeneration : MonoBehaviour
{
    [SerializeField]
    GameObject branch;

    [Header("Branch Settings")]
    [SerializeField]
    [Tooltip("The diameter of the branch crown")]
    float diameter = 1;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    [Tooltip("The position of the diameter (left=0.0, right=1.0)")]
    float diameterPosition = 0.75f;

    [Space]
    [SerializeField]
    float attractionRadius = 1.0f;
    [SerializeField]
    int amountAttractors = 20;
    [SerializeField]
    float attractorKillDistance = 0.5f;


    List<Vector2> attractors = new List<Vector2>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
