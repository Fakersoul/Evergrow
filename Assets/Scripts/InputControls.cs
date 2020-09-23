using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InputControls : MonoBehaviour
{
    private float time = 0.0f;

    [SerializeField]
    private TreeGeneration tree = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.touches[i].position);
            Debug.DrawLine(Vector3.zero, touchPosition, Color.red);

            float pushPercentageWidth = Camera.main.ScreenToViewportPoint(Input.touches[i].position).x;
            tree.ChangeGrowingDirection((pushPercentageWidth - 0.5f) * 2); //Normally bottom left of the screen we map it to [-1, 1]

            time += Time.deltaTime;
            if (time >= 2.0f)
            {
                time = 0.0f;
                //Debug.Log(touchPosition);
            }
        }
    }
}
