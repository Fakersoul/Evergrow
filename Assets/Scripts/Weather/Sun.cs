using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    // Update is called once per frame
    [SerializeField]
    [Min(1)]
    int amountRays = 1;

    [SerializeField]
    [Min(0.0f)]
    float length;

    [SerializeField]
    Vector2 direction = Vector2.down;

    Vector2 leftPoint;
    Vector2 rightPoint;

    //float GetYValue(float xValue) 
    //{
    //    return ((direction.x * xValue) / -direction.y) + transform.position.y;
    //}
    //Gizmos.DrawLine(new Vector2(-length + transform.position.x, GetYValue(-length)), new Vector2(length + transform.position.x, GetYValue(length)));

    private void Start()
    {



        //transform.position = Camera.main.ScreenToWorldPoint(new Vector2(0, 1));
        //length = Camera.main.ScreenToWorldPoint(new Vector2(1, 0)).x;


        leftPoint = new Vector3(-length, 0.0f);
        rightPoint = new Vector3(length, 0.0f);


    }

    void Update()
    {
        float amount = 0.0f;
        for (int i = 0; i < amountRays; i++)
        {
            amount += 1.0f / (amountRays + 1);
            Vector2 rayLocation = Vector2.Lerp(leftPoint, rightPoint, amount) + (Vector2)transform.position;

            RaycastHit2D hit = Physics2D.Raycast(rayLocation, direction);

            if (hit)
            { 
                NatureInfluence natureInfluece = hit.collider.gameObject.GetComponent<NatureInfluence>();
                if (natureInfluece)
                    natureInfluece.OnSunCollision();
            } 

        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(leftPoint, rightPoint);

        float amount = 0.0f;
        for (int i = 0; i < amountRays; i++)
        {
            amount += 1.0f / (amountRays + 1);
            Vector2 rayLocation = Vector2.Lerp(leftPoint, rightPoint, amount) + (Vector2)transform.position;

            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(rayLocation, rayLocation + direction);
        }
    }
}
