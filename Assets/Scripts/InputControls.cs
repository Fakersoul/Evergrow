using UnityEngine;


public class InputControls : MonoBehaviour
{
    [SerializeField]
    private TreeGrowthController treeController = null;

    [SerializeField]
    private GameObject MainMenu = null;

#if UNITY_EDITOR
    //Only in the editor it is possible to manipulate the tree with arrow keys
    [SerializeField]
    [ReadOnly] 
    bool isRemote; 
#endif

    // Start is called before the first frame update
    void Start()
    {
        if (!treeController)
            Debug.LogError("Input Controls has no tree");
        if (!MainMenu)
            Debug.LogError("Input Controls has no main menu");
    }

    // Update is called once per frame
    void Update()
    {
        if (!treeController || !MainMenu) 
            return;


#if UNITY_EDITOR
        isRemote = UnityEditor.EditorApplication.isRemoteConnected;
        if (!isRemote)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                treeController.ChangeGrowthDirection(-1);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                treeController.ChangeGrowthDirection(1);
            }
            if (Input.GetKey(KeyCode.Escape))
            {
                MainMenu.GetComponent<MainMenu>().Pause();
            }
        }
        else
#endif
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.touches[i].position);
                Debug.DrawLine(Vector3.zero, touchPosition, Color.red);

                float pushPercentageWidth = Camera.main.ScreenToViewportPoint(Input.touches[i].position).x;
                treeController.ChangeGrowthDirection((pushPercentageWidth - 0.5f) * 2); //Normally bottom left of the screen we map it to [-1, 1]
            }
        }
    }
}
