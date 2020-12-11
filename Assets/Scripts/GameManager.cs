using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    GrowingSpline leftTree;
    [SerializeField]
    GrowingSpline playerTree;
    [SerializeField]
    GrowingSpline rightTree;

#if UNITY_EDITOR
    [SerializeField]
    float timeScale = 1.0f;
#endif

    public GrowingSpline GetHighestTree() 
    {
        GrowingSpline highestTree = playerTree;
        float height = playerTree.GetPointWorldPos(playerTree.TopNodeIndex).y;

        if (height < leftTree.GetPointWorldPos(playerTree.TopNodeIndex).y)
            highestTree = leftTree;

        if (height < rightTree.GetPointWorldPos(playerTree.TopNodeIndex).y)
            highestTree = rightTree;

        return highestTree;
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        Time.timeScale = timeScale;
    }
#endif
}
