using UnityEngine;

public class Cloud : MonoBehaviour
{
    private float _speed = 2;
    private float _endPositionX;

    public void StartFloating(float speed, float endPositionX)
    {
        _speed = speed;
        _endPositionX = endPositionX;
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