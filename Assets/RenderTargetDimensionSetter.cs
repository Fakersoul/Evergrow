using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
public class RenderTargetDimensionSetter : MonoBehaviour
{
    [SerializeField]
    RenderTexture renderTarget = null;
    [SerializeField]
    Camera mainCamera = null;

    private void OnValidate()
    {
        if (renderTarget && mainCamera) 
        {
            RenderTexture newRenderTexture = new RenderTexture(renderTarget);
            newRenderTexture.width = mainCamera.pixelWidth;
            newRenderTexture.height = mainCamera.pixelHeight;
            renderTarget.Release();
            renderTarget = newRenderTexture;
        }
    }

    void Start()
    {
        if(Application.isPlaying)
            Destroy(this);
    }
}
