using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    GrowingSpline tree = null;

    [SerializeField]
    float snappingTime = 2.0f;

    float elapsedTimeCamera = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        if (!tree)
            Debug.LogError("Camera Movement has no tree");
    }

    // Update is called once per frame
    void Update()
    {
        if (!tree)
            return;


        Vector2 topOfTree = tree.TopNode;
        if (elapsedTimeCamera > snappingTime) 
        {
            transform.position = topOfTree;
        }
        else //Smooth transition on start
        {
            if (topOfTree.y >= transform.position.y)
            {
                elapsedTimeCamera += Time.deltaTime;

                float newXpos = Mathf.Lerp(transform.position.x, topOfTree.x, elapsedTimeCamera / snappingTime);
                float newYpos = Mathf.Lerp(transform.position.y, topOfTree.y, elapsedTimeCamera / snappingTime);

                transform.position = new Vector2(newXpos, newYpos);
            }
        }
    }
}
