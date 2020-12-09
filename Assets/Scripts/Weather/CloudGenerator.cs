using UnityEngine;

public class CloudGenerator : MonoBehaviour
{
    [Header("Spawning")]
    [SerializeField]
    GameObject cloudPrefab = null;
    [SerializeField]
    float spawnRate = 5.0f; //Spawnrate of cloud
    [SerializeField]
    Vector2 endPosition = new Vector2(-5.0f, 0.0f); //Point that destroys clouds
    [SerializeField]
    Vector2 startPosition = new Vector2(5.0f, 0.0f); //Point that spawns clouds

    [Header("Offset")]
    [SerializeField]
    float jitterExtend = 1.0f;
    [SerializeField]
    float minScale = 1.0f;
    [SerializeField]
    float maxScale = 1.0f;
    

    // Start is called before the first frame update
    void Start()
    {
        if (minScale > maxScale)
            minScale = maxScale;

        Invoke("AttemptSpawn", spawnRate); //If does not work or is too slow, try coroutine!!!
    }

    //TODO GET RID OF MAGIC NUMBERS
    void SpawnCloud()
    {
        float startY = Random.Range(startPosition.y - jitterExtend + transform.position.y, startPosition.y + jitterExtend + transform.position.y);
        GameObject Cloud = Instantiate(cloudPrefab, new Vector3(startPosition.x, startY), new Quaternion(), transform); //Otherwise bug on screen while playing!
        float scale = Random.Range(minScale, maxScale);
        Cloud.transform.localScale = new Vector2(scale, scale);
        Cloud.GetComponent<Cloud>().EndPositionX = endPosition.x;
    }

    void AttemptSpawn()
    {
        SpawnCloud();
        Invoke("AttemptSpawn", spawnRate); //See the loop? This is called again in Start Method...
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 drawStartPosition = startPosition + (Vector2)transform.position;
        Vector2 drawEndPosition = endPosition + (Vector2)transform.position;

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(drawStartPosition, 1.0f);   
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(drawEndPosition, 1.0f);   
        Gizmos.color = Color.white;

        Gizmos.DrawLine(drawStartPosition, drawEndPosition);
    }
}
