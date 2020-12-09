using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(SpriteRenderer))]
public class Cloud : MonoBehaviour
{
    [SerializeField]
    GameObject rainGenerator = null;

    [SerializeField]
    float _speed = 2;

    [SerializeField]
    Sprite[] sprites = null;

    private float _endPositionX;
    public float EndPositionX { set { _endPositionX = value; } }

    SpriteRenderer spriteRenderer = null;

    public void SetColor(Color color) 
    {
        spriteRenderer.color = color;
    }

    public void SetRain(bool value) 
    {
        rainGenerator.SetActive(value);
    }

    private void Start()
    {
        if (!rainGenerator)
            Debug.LogError("No rain generator on cloud");

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * (Time.deltaTime * _speed));

        if (transform.position.x > _endPositionX)
        {
            gameObject.GetComponentInParent<WeatherGenerator>().RemoveCloud(this);
            Destroy(gameObject);
        }
    }
}