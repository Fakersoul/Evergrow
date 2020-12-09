using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormCloudGenerator : MonoBehaviour
{
    [SerializeField]
    GameObject[] stormClouds = null;

    [SerializeField]
    float spawnrate = 2.0f;

    [SerializeField]
    GameObject stormCloudEndpoint;

    Vector3 stormStartPosition;

    // Start is called before the first frame update
    void Start()
    {
        stormStartPosition = transform.position;

        Invoke("AttemptStormSpawn", spawnrate); //if too slow, try coroutine!
        
    }

    void SpawnStormCloud()
    {
        int randomIndex = UnityEngine.Random.Range(0, stormClouds.Length);
        GameObject StormCloud = Instantiate(stormClouds[randomIndex]);

        StormCloud.transform.position = stormStartPosition;
    }

    void AttemptStormSpawn()
    {
        SpawnStormCloud();

        Invoke("AttemptStormSpawn", spawnrate);

    }
}
