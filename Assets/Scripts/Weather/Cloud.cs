using UnityEngine;

public class Cloud : MonoBehaviour
{
    [SerializeField]
    float _speed = 2;

    private float _endPositionX;
    public float EndPositionX { set { _endPositionX = value; } }

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