using UnityEngine;
using System.Collections;

public class LevelButton : MonoBehaviour
{

    // Use this for initialization
    public int levelIndex;
    public bool isActive;
    private UITexture bgTexture;
    //private UITexture starTexture;
    private UILabel levelIndexLabel;
    GameObject[] starsNormal = new GameObject[3];
    GameObject[] starsHard = new GameObject[3];
    bool isHardLevel;

    GameObject starsNormalMain;
    GameObject starsHardMain;
    void Start()
    {

        //SetAttribute();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void SetAttribute(int levelIndex)
    {
        this.levelIndex = levelIndex;

        bgTexture = Master.GetChildByName(gameObject, "BG").GetComponent<UITexture>();
        levelIndexLabel = Master.GetChildByName(gameObject, "LevelIndex").GetComponent<UILabel>();

        levelIndexLabel.text = levelIndex.ToString();
        starsNormalMain = Master.GetChildByName(gameObject, "StarsNormalMain");
        starsHardMain = Master.GetChildByName(gameObject, "StarsHardMain");

        for (int i = 0; i < 3; i++)
        {
            starsNormal[i] = Master.GetChildByName(gameObject, "Star_Normal_" + (i + 1));
            starsHard[i] = Master.GetChildByName(gameObject, "Star_Hard_" + (i + 1));
            starsNormal[i].SetActive(false);
            starsHard[i].SetActive(false);

        }

        if (levelIndex > (Master.LevelData.lastLevel + 1))
        {
            isActive = false;
        }
        else
        {
            isActive = true;
        }

        if ((Master.LevelData.lastLevel >= Master.LevelData.totalLevel) && (Master.LevelData.lastLevelHard + 1) >= levelIndex)
        {
            isHardLevel = true;
        }

        SetCollider();
        SetBackground();
        SetStar();

        if (!isActive)
        {
            gameObject.SetActive(false);
        }



    }

    void SetBackground()
    {
        if (isActive)
        {
            if ((Master.LevelData.lastLevel >= Master.LevelData.totalLevel) && (Master.LevelData.lastLevelHard + 1) >= levelIndex)
            {
                bgTexture.mainTexture = Resources.Load<Texture2D>("Textures/UI/Menu/Buttons/BG_LevelButton_hard");
            }
            else
            {
                bgTexture.mainTexture = Resources.Load<Texture2D>("Textures/UI/Menu/Buttons/BG_LevelButton");
            }
        }
    }

    void SetStar()
    {
        if (isActive)
        {
            int numberStarsNormal = Master.LevelData.GetStarAtLevel(levelIndex);
            int numberStarsHard = Master.LevelData.GetStarAtLevelHard(levelIndex);
            for (int i = 0; i < numberStarsNormal; i++)
            {
                starsNormal[i].SetActive(true);
            }
            for (int i = 0; i < numberStarsHard; i++)
            {
                starsHard[i].SetActive(true);
            }
            if (isHardLevel)
            {
                starsHardMain.SetActive(true);
            }
            else
            {
                starsHardMain.SetActive(false);
            }
        }
    }

    void SetCollider()
    {
        if (!isActive)
        {
            foreach (Collider2D item in GetComponentsInChildren<Collider2D>())
            {
                Destroy(item);
            }

            foreach (UIButton item in GetComponentsInChildren<UIButton>())
            {
                Destroy(item);
            }
        }
    }

    public void onClick()
    {
        Master.PlaySoundButtonClick();
        Master.UIMenu.ShowDialog("LevelSelected", 0.5f, new string[] { levelIndex.ToString() });

        if (Master.Tutorial.isDoingTutorial)
        {
            Master.Tutorial.currentStepGO.SetActive(false);
        }

    }

}
