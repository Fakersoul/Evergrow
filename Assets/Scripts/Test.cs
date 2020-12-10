using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(gameObject.transform.position, new Vector2(1.0f, -0.1f), float.MaxValue);
        Debug.Log(hits.Length);
        foreach (RaycastHit2D hit in hits) 
        {
            Debug.DrawLine(gameObject.transform.position, hit.point, Color.green);
        }

        //Vector2 temp = transform.position;
        //temp += new Vector2(1.0f, -0.1f) * 10;
        //Debug.DrawLine(gameObject.transform.position, temp, Color.white, 0.0f);
        
    }
}
