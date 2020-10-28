using UnityEngine;

public class CloudGeneratorScript : MonoBehaviour
{
    [SerializeField]
    GameObject[] clouds;

    [SerializeField]
    float spawnRate; //Spawnrate of cloud


    [SerializeField]
    GameObject CloudEndPoint; //Point that destroys clouds

    Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;

        Invoke("AttemptSpawn", spawnRate); //If does not work or is too slow, try coroutine!!!

    }

    void SpawnCloud()
    {
        int randomIndex = Random.Range(0, clouds.Length);
        GameObject cloud = Instantiate(clouds[randomIndex]);

        float startY = Random.Range(startPosition.y - 1f, startPosition.y + 1f);

        cloud.transform.position = new Vector3(startPosition.x, startY, startPosition.z);

        float scale = Random.Range(0.8f, 1.2f);
        cloud.transform.localScale = new Vector2(scale, scale);

        float speed = Random.Range(0.5f, 1.5f);
        cloud.GetComponent<Cloud>().StartFloating(speed, CloudEndPoint.transform.position.x);
    }

    void AttemptSpawn()
    {
        SpawnCloud();

        Invoke("AttemptSpawn", spawnRate); //See the loop? This is called again in Start Method...

    }


}
