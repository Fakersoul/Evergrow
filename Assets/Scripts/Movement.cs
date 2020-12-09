using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField]
    GrowingSpline tree = null;

    [SerializeField]
    [Tooltip("Time it takes until the camera snapping ")]
    float snappingTime = 2.0f;
    [SerializeField]
    [Tooltip("Distance to start the snapping")]
    float distancce = 5.0f;
    [SerializeField]
    [Tooltip("Offset vector of the final camera position")]
    Vector2 offsetVector = Vector2.zero;

    bool startSnapping = false;
    float elapsedTimeCamera = 0.0f;
    Vector2 oldCameraPosition = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        if (!tree)
            Debug.LogError("Camera Movement has no tree");

        oldCameraPosition = transform.position;
        distancce *= distancce;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 topOfTree = tree.TopNode;

        if (!startSnapping && topOfTree.y > transform.position.y)
            if ((topOfTree - (Vector2)transform.position).sqrMagnitude > distancce)
                startSnapping = true;

        if(startSnapping)
            if (elapsedTimeCamera > snappingTime)
            {
                transform.position = topOfTree + offsetVector;
            }
            else //Smooth transition on start
            {
                elapsedTimeCamera += Time.deltaTime;

                float newXpos = Mathf.Lerp(oldCameraPosition.x, topOfTree.x + offsetVector.x, elapsedTimeCamera / snappingTime);
                float newYpos = Mathf.Lerp(oldCameraPosition.y, topOfTree.y + offsetVector.y, elapsedTimeCamera / snappingTime);

                transform.position = new Vector2(newXpos, newYpos);
            }
    }
}
