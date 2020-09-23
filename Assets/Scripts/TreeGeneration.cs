using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.U2D.Path;


public class TreeGeneration : MonoBehaviour
{
    Vector2 growthDirection = new Vector2(0.0f, 1.0f);

    [SerializeField] [Range(0.0f, 1.0f)]
    float sensitivity = 0.5f;

    [SerializeField]
    [Tooltip("Specify the maximum angle of the tree direction. Note this property will be changed in the maximum distance of the [growthDirection.x] .")]
    [Rename("Maximum Angle")]
    //Range attribute does not work here
    float maxDirectionX = 70.0f;

    // Start is called before the first frame update
    void Start()
    {
        //Calculate the maximum distance of the growthDirection.x
        maxDirectionX = Mathf.Clamp(maxDirectionX, 0.0f, 90.0f);
        if (maxDirectionX == 0.0f)
            return;
        else if (maxDirectionX == 90.0f)
            maxDirectionX = float.MaxValue;
        else
        {
            float cosAngle = Mathf.Cos(maxDirectionX);
            maxDirectionX = Mathf.Sqrt(Mathf.Pow((1 - cosAngle) / cosAngle, 2.0f) - 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // growDirection is a percentage value where -1 is left and 1 is right
    public void ChangeGrowingDirection(float amount) 
    {
        float changeInDirection = sensitivity * amount * maxDirectionX;
        growthDirection = new Vector2(changeInDirection, growthDirection.y);
    }

    private void OnDrawGizmos()
    {
        Vector3 temp = growthDirection;
        Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + temp, Color.green);
    }
}
