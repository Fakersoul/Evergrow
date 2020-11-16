using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(SpriteRenderer))]
public class Cloud : MonoBehaviour
{
    [SerializeField]
    float _speed = 2;

    [SerializeField]
    Sprite[] sprites;

    private float _endPositionX;
    public float EndPositionX { set { _endPositionX = value; } }

    private void Start()
    {
        int randomIndex = Random.Range(0, sprites.Length);
        GetComponent<SpriteRenderer>().sprite = sprites[randomIndex];
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * (Time.deltaTime * _speed));

        if (transform.position.x > _endPositionX)
        {
            Destroy(gameObject);
        }
    }
}