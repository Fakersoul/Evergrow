using UnityEngine;
using System.IO;

public class SunRays : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Only allowed to go to the right and down!")]
    Vector2Int direction = new Vector2Int(1, -1);

    [SerializeField]
    Camera renderCamera = null;
    RenderTexture renderTexture = null;

    [SerializeField]
    GameObject[] toRenderObjects = null;

#if UNITY_EDITOR
    [Space]
    [SerializeField]
    bool renderSunMask = false;
    [SerializeField]
    bool textureOutput = false;
#endif

    Texture2D sunMask = null;
    
    [SerializeField]
    Material sunRays = null;

    private void Start()
    {
        renderTexture = new RenderTexture(Camera.main.scaledPixelWidth, Camera.main.scaledPixelHeight, 0);
        renderCamera.targetTexture = renderTexture;
        
        sunMask = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);

        var colors = sunMask.GetRawTextureData<Color32>();
        for (int y = 0; y < sunMask.height; y++)
        {
            for (int x = 0; x < sunMask.width; x++)
            {
                colors[x + (y*sunMask.width)] = Color.clear;
            }
        }    
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (renderSunMask)
        {
#endif
            RenderTexture.active = renderTexture;
            sunMask.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            var colors = sunMask.GetRawTextureData<Color32>();
            bool[,] visitedPixels = new bool[sunMask.width, sunMask.height];

            
            //multithreading??
            foreach (GameObject renderObject in toRenderObjects)
            {
                Renderer[] rendererComponents = renderObject.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in rendererComponents)
                {
                    Vector2 screenCoordinateMin = Camera.main.WorldToScreenPoint(renderer.bounds.min);
                    Vector2 screenCoordinateMax = Camera.main.WorldToScreenPoint(renderer.bounds.max);

                    Vector2Int pixelCoordinateMin = new Vector2Int(Mathf.Clamp(Mathf.RoundToInt(screenCoordinateMin.x), 0, sunMask.width - 1), Mathf.Clamp(Mathf.RoundToInt(screenCoordinateMin.y), 0, sunMask.height - 1));
                    Vector2Int pixelCoordinateMax = new Vector2Int(Mathf.Clamp(Mathf.RoundToInt(screenCoordinateMax.x), 0, sunMask.width - 1), Mathf.Clamp(Mathf.RoundToInt(screenCoordinateMax.y), 0, sunMask.height - 1));

                    for (int y = pixelCoordinateMax.y; y >= pixelCoordinateMin.y; y--)
                    {
                        for (int x = pixelCoordinateMin.x; x < pixelCoordinateMax.x; x++)
                        {
                            if (visitedPixels[x, y])
                                continue;

                            if (colors[x + (y * sunMask.width)].a == 0)
                                visitedPixels[x, y] = true;

                            else //Color in line until bottom
                            {

                                Vector2Int startPixel = new Vector2Int(x, y);
                                Vector2Int endPixel = new Vector2Int(sunMask.width, 0);

                                //Calculation endPixel
                                {
                                    float slope = 0.0f;
                                    Vector2Int temp = startPixel - direction;
                                    slope = (temp.y - startPixel.y) / (temp.x - startPixel.x);

                                    //Try to find intersection bottom
                                    int endX = Mathf.RoundToInt((-startPixel.y + (slope * startPixel.x)) / slope);
                                    if (endX >= sunMask.width)
                                        endPixel.y = Mathf.RoundToInt((slope * (sunMask.width - startPixel.x)) + startPixel.y);
                                    else
                                        endPixel.x = endX;
                                }

                                ////Plotting line : https://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm
                                int dx = Mathf.Abs(endPixel.x - startPixel.x);
                                int sx = startPixel.x < endPixel.x ? 1 : -1;
                                int dy = -Mathf.Abs(endPixel.y - startPixel.y);
                                int sy = startPixel.y < endPixel.y ? 1 : -1;
                                int err = dx + dy;  /* error value e_xy */
                                while (true)   /* loop */
                                {
                                    if (startPixel.x >= sunMask.width || startPixel.x < 0)
                                        break;
                                    if (startPixel.y >= sunMask.height || startPixel.y < 0)
                                        break;

                                    visitedPixels[startPixel.x, startPixel.y] = true;
                                    colors[startPixel.x + (startPixel.y * sunMask.width)] = Color.white;

                                    if (startPixel.x == endPixel.x && startPixel.y == endPixel.y)
                                        break;
                                    int e2 = 2 * err;
                                    if (e2 >= dy) /* e_xy+e_x > 0 */
                                    {
                                        err += dy;
                                        startPixel.x += sx;
                                    }
                                    if (e2 <= dx) /* e_xy+e_y < 0 */
                                    {
                                        err += dx;
                                        startPixel.y += sy;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //colors[x + (y * sunMask.width)] = Color.white;
            if (sunRays) 
            {
                sunMask.Apply();
                sunRays.mainTexture = sunMask;
            }

#if UNITY_EDITOR
            if (textureOutput)
            {

                byte[] bytes = sunMask.EncodeToPNG();

                // For testing purposes, also write to a file in the project folder
                File.WriteAllBytes(Application.dataPath + "/../SavedScreen.png", bytes);
            }
#endif

#if UNITY_EDITOR
        }    
#endif
    }

    private void OnDrawGizmosSelected()
    {
        
    }
}
