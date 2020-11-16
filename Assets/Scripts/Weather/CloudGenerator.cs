using UnityEngine;

public class CloudGenerator : MonoBehaviour
{
    [SerializeField]
    GameObject cloudPrefab;

    [SerializeField]
    float spawnRate; //Spawnrate of cloud

    [SerializeField]
    Vector2 endPosition; //Point that destroys clouds

    [SerializeField]
    Vector2 startPosition; //Point that spawns clouds

    // Start is called before the first frame update
    void Start()
    {
        Invoke("AttemptSpawn", spawnRate); //If does not work or is too slow, try coroutine!!!

        //Transform positions to offset of cloud generator
        startPosition -= (Vector2)transform.position;
        endPosition -= (Vector2)transform.position;
    }

    //TODO GET RID OF MAGIC NUMBERS
    void SpawnCloud()
    {
        float startY = Random.Range(startPosition.y - 1f + transform.position.y, startPosition.y + 1f + transform.position.y);
        GameObject Cloud = Instantiate(cloudPrefab, new Vector3(startPosition.x, startY), new Quaternion(), transform); //Otherwise bug on screen while playing!
        float scale = Random.Range(0.8f, 1.2f);
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
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(startPosition, 1.0f);   
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(endPosition, 1.0f);   
        Gizmos.color = Color.white;

        Gizmos.DrawLine(startPosition, endPosition);
    }
}
