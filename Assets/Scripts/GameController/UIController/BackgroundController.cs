using UnityEngine;
using System.Collections;

public class BackgroundController : MonoBehaviour
{

    // Use this for initialization
    [HideInInspector]
    public GameObject backgroudRoot;
    [HideInInspector]
    public Camera camera;
    UITexture backgroundTexture;


    void Awake()
    {
        if (Master.Background == null)
        {
            Master.Background = this;
        }

        backgroudRoot = GameObject.Find("Background Root");
        camera = Master.GetChildByName(backgroudRoot, "Camera").GetComponent<Camera>();
        backgroundTexture = Master.GetChildByName(backgroudRoot, "BG").GetComponent<UITexture>();
    }

    void Start()
    {
        backgroundTexture.mainTexture = Resources.Load<Texture2D>("Textures/Background/bg_stage_" + GetBackgroundIndex(Master.LevelData.currentLevel));
    }

    public static int GetBackgroundIndex(int levelIndex)
    {
        int bgIndex = 1;
        if (levelIndex > 15)
        {
            bgIndex = 2;
        }
        if (levelIndex >= 30)
        {
            bgIndex = 3;
        }
        return bgIndex;
    }
}
