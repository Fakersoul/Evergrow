using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SunBeam : MonoBehaviour
{
    [SerializeField]
    RectTransform rectTransform = null;
    [SerializeField]
    BoxCollider2D boxCollider = null;

    private void OnValidate()
    {
        if (rectTransform)
            if (boxCollider)
                boxCollider.size = rectTransform.sizeDelta;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
