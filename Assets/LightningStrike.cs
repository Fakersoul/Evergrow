using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningStrike : MonoBehaviour
{
    [SerializeField]
    GameManager manager = null;

    [SerializeField]
    float timeToStrike = 30.0f;

    void Strike() 
    {
        
    }
    
    // Start is called before the first frame update
    void Start()
    {
        if (!manager)
            Debug.LogError("No manager");

        //InvokeRepeating("Strike", timeToStrike, Random.value * timeToStrike);    
    }
}
