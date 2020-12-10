using System.Collections.Generic;
using UnityEngine;

public class WeatherGenerator : MonoBehaviour
{
    [SerializeField]
    GameObject cloudPrefab = null;

    [Header("Spawning")]
    [SerializeField]
    Vector2 endPosition = new Vector2(-5.0f, 0.0f); //Point that destroys clouds
    [SerializeField]
    Vector2 startPosition = new Vector2(5.0f, 0.0f); //Point that spawns clouds

    [Header("Generic weather settings")]
    [SerializeField]
    [Min(0.0f)]
    Vector2 rangeSecondsUntilStorm = new Vector2(10, 20);
    [SerializeField]
    [Min(0.0f)]
    Vector2 rangeSecondsUntilClear = new Vector2(10, 20);
    [SerializeField]
    float transitionTime = 0.0f;

    [Header("Normal weather")]
    [SerializeField]
    float spawnRateNormal = 5.0f; //Spawnrate of cloud
    [SerializeField]
    Color cloudColorNormal = Color.white;

    [Header("Storm weather")]
    [SerializeField]
    float spawnRateStorm = 5.0f; //Spawnrate of cloud
    [SerializeField]
    Color cloudColorStorm = new Color(0.1f, 0.1f, 0.1f);

    [Header("Offset")]
    [SerializeField]
    float jitterExtend = 1.0f;
    [SerializeField]
    float minScale = 1.0f;
    [SerializeField]
    float maxScale = 1.0f;

    public enum WeatherTypes
    {
        Clear,
        Storm
    }

    List<Cloud> spawnedClouds = new List<Cloud>();
    float spawnRate = 5.0f; //Spawnrate of cloud
    Color cloudColor = Color.white;
    WeatherTypes currentWeatherCondition = WeatherTypes.Clear;

    float timePassedSpawning = 0.0f;
    float timePassedTransition = 0.0f;
    float timePassedWeatherCondition = 0.0f;

    float timeUntilClear = 0.0f;
    float timeUntilStorm = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        if (minScale > maxScale)
            minScale = maxScale;

        if (!cloudPrefab)
            Debug.LogError("No cloud prefab");


        switch (currentWeatherCondition)
        {
            case WeatherTypes.Clear:
                spawnRate = spawnRateNormal;
                cloudColor = cloudColorNormal;
                break;
            case WeatherTypes.Storm:
                spawnRate = spawnRateStorm;
                cloudColor = cloudColorStorm;
                break;
            default:
                break;
        }

        timePassedTransition = transitionTime;

        timeUntilStorm = Random.Range(rangeSecondsUntilStorm.x, rangeSecondsUntilStorm.y);
        timeUntilClear = Random.Range(rangeSecondsUntilClear.x, rangeSecondsUntilClear.y);
    }

    void Update() 
    {
        timePassedSpawning += Time.deltaTime;

        if (timePassedTransition < transitionTime) //There is a transition
        {
            switch (currentWeatherCondition)
            {
                case WeatherTypes.Clear:
                    spawnRate = Mathf.Lerp(spawnRateStorm, spawnRateNormal, timePassedTransition / transitionTime);
                    cloudColor = Color.Lerp(cloudColorStorm, cloudColorNormal, timePassedTransition / transitionTime);
                    break;
                case WeatherTypes.Storm:
                    spawnRate = Mathf.Lerp(spawnRateNormal, spawnRateStorm, timePassedTransition / transitionTime);
                    cloudColor = Color.Lerp(cloudColorNormal, cloudColorStorm, timePassedTransition / transitionTime);
                    break;
                default:
                    break;
            }

            foreach (Cloud cloud in spawnedClouds)
            {
                cloud.SetColor(cloudColor);
            }

            timePassedTransition += Time.deltaTime;

            if (timePassedTransition > transitionTime) //Transition ended
            {
                foreach (Cloud cloud in spawnedClouds)
                {
                    switch (currentWeatherCondition)
                    {
                        case WeatherTypes.Clear:
                            cloud.SetRain(false);
                            break;
                        case WeatherTypes.Storm:
                            cloud.SetRain(true);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        else //No transition
        {
            timePassedWeatherCondition += Time.deltaTime;


            switch (currentWeatherCondition)
            {
                case WeatherTypes.Clear:
                    if (timePassedWeatherCondition > timeUntilStorm)
                    {
                        timePassedWeatherCondition = 0.0f;
                        timePassedTransition = 0.0f;
                        currentWeatherCondition = WeatherTypes.Storm;
                        timeUntilStorm = Random.Range(rangeSecondsUntilStorm.x, rangeSecondsUntilStorm.y);
                    } 
                    break;
                case WeatherTypes.Storm:
                    if (timePassedWeatherCondition > timeUntilClear)
                    {
                        timePassedWeatherCondition = 0.0f;
                        timePassedTransition = 0.0f;
                        currentWeatherCondition = WeatherTypes.Clear;
                        timeUntilClear = Random.Range(rangeSecondsUntilClear.x, rangeSecondsUntilClear.y);
                    }
                    break;
                default:
                    break;
            }
        }

        if (timePassedSpawning > spawnRate) 
        {
            SpawnCloud();
            timePassedSpawning -= spawnRate;
        }
    }

    //TODO GET RID OF MAGIC NUMBERS
    void SpawnCloud()
    {
        float startY = Random.Range(startPosition.y - jitterExtend + transform.position.y, startPosition.y + jitterExtend + transform.position.y);
        GameObject cloud = Instantiate(cloudPrefab, new Vector3(startPosition.x, startY), new Quaternion(), transform); //Otherwise bug on screen while playing!

        float scale = Random.Range(minScale, maxScale);
        cloud.transform.localScale = new Vector2(scale, scale);

        Cloud cloudComponent = cloud.GetComponent<Cloud>();
        cloudComponent.EndPositionX = endPosition.x;

        bool inTransition = timePassedTransition >= transitionTime ? false : true;
            
        switch (currentWeatherCondition)
        {
            case WeatherTypes.Clear:
                cloudComponent.SetRain(inTransition);
                break;
            case WeatherTypes.Storm:
                cloudComponent.SetRain(!inTransition);
                break;
            default:
                break;
        }

        cloud.GetComponent<SpriteRenderer>().color = cloudColor;
        spawnedClouds.Add(cloudComponent);
    }

    public void RemoveCloud(Cloud cloud) 
    {
        spawnedClouds.Remove(cloud);
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
