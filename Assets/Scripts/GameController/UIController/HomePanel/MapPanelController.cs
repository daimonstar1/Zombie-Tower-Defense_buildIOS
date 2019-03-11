using UnityEngine;
using System.Collections;

public class MapPanelController : MonoBehaviour
{

    private GameObject levelButtons;
    GameObject levelSelectPanel;

    void Awake()
    {
        AssignObject();
    }

    void Start()
    {
        // InvokeRepeating("CheckAlertQuestComplete", 0, 2);
    }

    void Update()
    {
        if (levelSelectPanel.transform.localPosition.x > 0)
        {
            levelSelectPanel.transform.localPosition = new Vector3(0, 0, 0);
            levelSelectPanel.GetComponent<SpringPanel>().enabled = false;
            //levelSelectPanel.GetComponent<SpringPanel>().strength = 100;
            //levelSelectPanel.GetComponent<SpringPanel>().target = new Vector3(0, 0, 0);
            levelSelectPanel.GetComponent<UIPanel>().clipOffset = new Vector3(0, 0, 0);
        }
        if (levelSelectPanel.transform.localPosition.x < -2700)
        {
            levelSelectPanel.transform.localPosition = new Vector3(-2700, 0, 0);

            levelSelectPanel.GetComponent<SpringPanel>().enabled = false;
            //levelSelectPanel.GetComponent<SpringPanel>().strength = 100;
            //levelSelectPanel.GetComponent<SpringPanel>().target = new Vector3(-2323, 0, 0);
            levelSelectPanel.GetComponent<UIPanel>().clipOffset = new Vector3(2700, 0, 0);

        }
    }

    void AssignObject()
    {
        levelButtons = Master.GetChildByName(gameObject, "LevelButtons");
        levelSelectPanel = Master.GetChildByName(gameObject, "LevelSelect");
    }

    public void OnOpen()
    {
        SetLevelButton();
        Master.Tutorial.CheckAndContinueNextStepTutorial(TutorialController.TutorialsIndex.BuildUnitInGameplay);
    }

    public void SetLevelButton()
    {
        Vector3 posLastLevelButton = Vector3.zero;
        foreach (Transform level in levelButtons.transform)
        {
            if (level.childCount > 0)
            {
                Destroy(level.GetChild(0).gameObject);
            }
            int levelIndex = int.Parse(level.gameObject.name);
            GameObject pf_levelButton = Master.GetGameObjectInPrefabs("UI/LevelButton");
            GameObject levelButton = NGUITools.AddChild(level.gameObject, pf_levelButton);
            levelButton.name = "LevelSelect_" + levelIndex;
            levelButton.GetComponentInChildren<LevelButton>().SetAttribute(levelIndex);

            if (!Master.LevelData.isUnlockedHardLevel)
            {
                if (levelIndex == Master.LevelData.lastLevel + 1)
                {
                    posLastLevelButton = level.transform.localPosition;
                }
            }
            else
            {
                if (levelIndex == Master.LevelData.lastLevelHard + 1)
                {
                    posLastLevelButton = level.transform.localPosition;
                }
            }

        }
        //      Debug.Log(Master.LevelData.lastLevel + " | " + posLastLevelButton.x);
        //set screen center of last level
        if (!Master.LevelData.isUnlockedHardLevel)
        {
            if (Master.LevelData.lastLevel >= 12)
            {
                levelSelectPanel.GetComponent<SpringPanel>().enabled = true;
                levelSelectPanel.GetComponent<SpringPanel>().strength = 100;
                levelSelectPanel.GetComponent<SpringPanel>().target = new Vector3(-posLastLevelButton.x, 0, 0);
            }
        }
        else
        {
            if (Master.LevelData.lastLevelHard >= 12)
            {
                levelSelectPanel.GetComponent<SpringPanel>().enabled = true;
                levelSelectPanel.GetComponent<SpringPanel>().strength = 100;
                levelSelectPanel.GetComponent<SpringPanel>().target = new Vector3(-posLastLevelButton.x, 0, 0);
            }
        }

    }



}
