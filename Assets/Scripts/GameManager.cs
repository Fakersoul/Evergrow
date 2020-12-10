using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameObject leftTree;
    [SerializeField]
    GameObject playerTree;
    [SerializeField]
    GameObject rightTree;

#if UNITY_EDITOR
    [SerializeField]
    float timeScale = 1.0f;
#endif
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        Time.timeScale = timeScale;
    }
#endif
}
