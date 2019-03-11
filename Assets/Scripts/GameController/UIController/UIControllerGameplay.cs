using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using DG.Tweening;

public class UIControllerGameplay : UIController
{

    [HideInInspector]
    public UILabel totalGoldLabel;
    [HideInInspector]
    public UISlider zombieProgressSlider;
    [HideInInspector]
    public GameObject zombieIconProgress;
    // [HideInInspector]
    // public UILabel unitsDeadLabel;
    [HideInInspector]
    public UILabel levelIndexLabel;
    [HideInInspector]
    public GameObject newEnemy;
    private string newEnemyID;

    private UITexture changeSpeedButton;

    public enum Positions
    {
        Left, Top, Right, Bottom, Center
    }
    public Dictionary<Positions, Transform> positionsOnScreen = new Dictionary<Positions, Transform>();

    //for boost exp
    GameObject timeRemainingBoostingEXP;
    UILabel timeCountdownBoostingEXP;
    UITexture expBoostIcon;
    GameObject boostEXPButton;

    public override void OnAwake()
    {
        if (Master.UI == null)
        {
            Master.UI = this;
        }

        Master.UIGameplay = this;
        Master.UIMenu = null;


        totalGoldLabel = Master.GetChildByName(uiRoot, "TotalGoldValue").GetComponent<UILabel>();
        zombieProgressSlider = Master.GetChildByName(uiRoot, "ZombieProgressSlider").GetComponent<UISlider>();
        zombieIconProgress = Master.GetChildByName(uiRoot, "ZombieIcon");
        //    unitsDeadLabel = Master.GetChildByName(uiRoot, "UnitsDeadValue").GetComponent<UILabel>();
        levelIndexLabel = Master.GetChildByName(uiRoot, "LevelIndexLabel").GetComponent<UILabel>();
        newEnemy = Master.GetChildByName(uiRoot, "NewEnemy");
        newEnemy.SetActive(false);
        // unitsDeadLabel.GetComponent<TweenColor>().enabled = false;
        //unitsDeadLabel.GetComponent<TweenScale>().enabled = false;

        positionsOnScreen.Add(Positions.Left, Master.GetChildByName(uiRoot, "Pos_Left").transform);
        positionsOnScreen.Add(Positions.Top, Master.GetChildByName(uiRoot, "Pos_Top").transform);
        positionsOnScreen.Add(Positions.Right, Master.GetChildByName(uiRoot, "Pos_Right").transform);
        positionsOnScreen.Add(Positions.Bottom, Master.GetChildByName(uiRoot, "Pos_Bottom").transform);
        positionsOnScreen.Add(Positions.Center, Master.GetChildByName(uiRoot, "Pos_Center").transform);

        changeSpeedButton = Master.GetChildByName(uiRoot, "ChangeSpeedButton").GetComponent<UITexture>();

        //for boost exp
        timeRemainingBoostingEXP = Master.GetChildByName(uiRoot, "TimeRemainingBoostingEXP");
        timeCountdownBoostingEXP = Master.GetChildByName(uiRoot, "TimeCountdownBoostingEXP").GetComponent<UILabel>();
        expBoostIcon = Master.GetChildByName(uiRoot, "EXPBoostIcon").GetComponent<UITexture>();
        boostEXPButton = Master.GetChildByName(uiRoot, "BoostEXPButton");

        if (Master.LevelData.currentLevel >= 2)
        {
            Master.GetChildByName(uiRoot, "RemoveUnitButtonMain").SetActive(true);
        }
        else
        {
            Master.GetChildByName(uiRoot, "RemoveUnitButtonMain").SetActive(false);

        }
    }

    public override void OnUpdate()
    {
        SetUI();
    }

    public override void OnStart()
    {
        Master.Audio.PlayBackgroundMusic("bg_gameplay", 0.7f);
        SetBoostEXP();

        ///for hide remove icon unit
        ///
        if (!Master.Tutorial.IsTutorialDone(TutorialController.TutorialsIndex.RemoveUnit))
        {
            Master.Gameplay.gold += 40;
        }
        Master.Touch.AddTouchEvent(TouchController.TouchType.TouchIn, () =>
        {
            if (isShowingRemoveIconUnit)
            {
                bool isClickAtUnit = false;
                Master.Touch.GetGameObjectAtMousePosition();
                if (Master.Touch.listGameObjectsAtMousePosition.Count > 0)
                {
                    foreach (GameObject obj in Master.Touch.listGameObjectsAtMousePosition)
                    {
                        if (obj.tag == "Unit" || obj.name == "RemoveUnitButton")
                        {
                            isClickAtUnit = true;
                        }
                    }
                }
                else
                {
                    isClickAtUnit = false;
                }
                if (!isClickAtUnit)
                {
                    if (!Master.Tutorial.isDoingTutorial)
                    {
                        HideRemoveIconUnit();
                    }
                }
            }
        });
    }

    void SetUI()
    {
        totalGoldLabel.text = Master.Gameplay.gold + "";
        //   unitsDeadLabel.text = Master.Gameplay.unitsDead + "/" + Master.Level.currentLevelData.NumberOfUnitsAllowedDead;
        levelIndexLabel.text = "Level " + Master.LevelData.currentLevel;
        SetZombieProgress();
    }

    void SetZombieProgress()
    {
        float value = (float)Master.Level.totalSequenceIndex / Master.Level.totalSequences;
        if (zombieProgressSlider.value < value)
        {
            zombieProgressSlider.value += Time.deltaTime / 2;
        }
        //float xChangeForZombieIcon = 165 - (zombieProgressSlider.value * 330);

        //  zombieIconProgress.transform.localPosition = new Vector3(xChangeForZombieIcon, zombieIconProgress.transform.localPosition.y, zombieIconProgress.transform.localPosition.z);
    }

    public void ChangeGameSpeed()
    {
        if (Master.Stats.isBoughtChangeGameSpeed)
        {
            Master.Gameplay.ChangeGameSpeed();
            if (Master.Gameplay.currentTimeScale == 2)
            {
                changeSpeedButton.mainTexture = Resources.Load<Texture2D>("Textures/UI/x1Speed");
            }
            else
            {
                changeSpeedButton.mainTexture = Resources.Load<Texture2D>("Textures/UI/x2Speed");
            }
        }
        else
        {
            Master.Gameplay.PauseGame();
            Master.UIGameplay.ShowDialog("BuyChangeGameSpeedDialog", 0.4f);
        }
    }

    public void PauseButton_OnClick()
    {
        //SceneManager.LoadScene("scene");
        Master.PlaySoundButtonClick();
        Master.Gameplay.PauseGame();
        Master.UIGameplay.ShowDialog("PauseGame", 0.4f);
    }

    public void SetNewEnemy(string enemyID)
    {
        newEnemyID = enemyID;
        newEnemy.SetActive(true);
        Master.WaitAndDo(10, () =>
        {
            newEnemy.SetActive(false);
        }, this);

        Master.Tutorial.CheckAndStartTutorial(TutorialController.TutorialsIndex.ViewZombieInfo, Vector3.zero, true, 10);

    }

    public void NewEnemyButton_OnClick()
    {
        Master.PlaySoundButtonClick();
        Master.Gameplay.PauseGame();
        Master.UI.ShowDialog(UIController.Dialog.ListDialogs.EnemyInfoDialog, 0.4f, new string[] { newEnemyID, "Gameplay" });
        newEnemy.SetActive(false);
        Master.Tutorial.CheckAndFinishTutorial(TutorialController.TutorialsIndex.ViewZombieInfo);
    }


    public void ShowLevelTitle(System.Action onComplete = null)
    {
        Master.Audio.PlaySound("snd_showLevelTitle", 0.7f);
        GameObject levelTitle = NGUITools.AddChild(uiRoot, Master.GetGameObjectInPrefabs("UI/LevelTitle"));
        levelTitle.GetComponent<UILabel>().text = "Level " + Master.LevelData.currentLevel;
        levelTitle.transform.position = positionsOnScreen[Positions.Right].position;
        float xChange = 0.2f;
        levelTitle.transform.DOMoveX(positionsOnScreen[Positions.Center].position.x - xChange, 0.27f).OnComplete(() =>
        {
            Master.Audio.PlaySound("snd_ready");

            levelTitle.transform.DOMoveX(positionsOnScreen[Positions.Center].position.x, 0.15f).OnComplete(() =>
            {
                Master.WaitAndDo(0.8f, () =>
                {
                    Master.Audio.PlaySound("snd_go");
                    Master.WaitAndDo(0.1f, () =>
                    {
                        levelTitle.transform.DOMoveX(positionsOnScreen[Positions.Center].position.x + xChange, 0.15f).OnComplete(() =>
                        {
                            Master.Audio.PlaySound("snd_showLevelTitle", 0.7f);

                            levelTitle.transform.DOMoveX(positionsOnScreen[Positions.Left].position.x - 0.5f, 0.3f).OnComplete(() =>
                            {
                                if (onComplete != null)
                                {
                                    onComplete();
                                }
                                Destroy(levelTitle);
                            });
                        });
                    }, this);

                }, this);
            });
        });
    }

    void SetBoostEXP()
    {
        if (BoostEXPController.Instance.IsBoosting)
        {
            timeRemainingBoostingEXP.SetActive(true);
            timeCountdownBoostingEXP.text = BoostEXPController.Instance.getTimeRemainingString();
            expBoostIcon.mainTexture = Resources.Load<Texture2D>("Textures/UI/x" + (BoostEXPController.Instance.BoostType + 1) + "EXP");
            boostEXPButton.GetComponent<UIButton>().enabled = false;
            boostEXPButton.GetComponent<Animator>().enabled = false;
        }
        else
        {
            boostEXPButton.SetActive(false);
            return;
        }
        Master.WaitAndDo(1, () =>
        {
            SetBoostEXP();
        }, this, true);
    }

    [HideInInspector]
    public bool isShowingRemoveIconUnit;
    public void ShowRemoveIconUnit()
    {
        if (isShowingRemoveIconUnit) return;

        GameObject[] listUnitsOnScene = GameObject.FindGameObjectsWithTag("Unit");

        foreach (GameObject unit in listUnitsOnScene)
        {
            UnitController unitController = unit.GetComponent<UnitController>();
            if (unitController != null)
            {
                unitController.ShowRemoveIconUnit(true);
                isShowingRemoveIconUnit = true;
            }
        }

        if (Master.Tutorial.isDoingTutorial && Master.Tutorial.currentTutorialIndex == TutorialController.TutorialsIndex.RemoveUnit)
        {
            if (Master.Tutorial.CheckAndContinueNextStepTutorial(TutorialController.TutorialsIndex.RemoveUnit))
            {
                GameObject arrow = Master.Tutorial.GetArrow();
                arrow.transform.position = listUnitsOnScene[0].transform.position - new Vector3(0, 0.15f, 0);
            }
        }
    }

    public void HideRemoveIconUnit()
    {
        if (!isShowingRemoveIconUnit) return;

        GameObject[] listUnitsOnScene = GameObject.FindGameObjectsWithTag("Unit");

        foreach (GameObject unit in listUnitsOnScene)
        {
            UnitController unitController = unit.GetComponent<UnitController>();
            if (unitController != null)
            {
                unitController.ShowRemoveIconUnit(false);
                isShowingRemoveIconUnit = false;
            }

        }
    }

    public void ToggleShowRemoveIconUnit()
    {
        if (isShowingRemoveIconUnit)
        {
            if (!Master.Tutorial.isDoingTutorial)
                HideRemoveIconUnit();
        }
        else
        {
            ShowRemoveIconUnit();
        }
    }

    //public void HightlightUnitDeadLabel()
    //{
    //    unitsDeadLabel.GetComponent<TweenColor>().enabled = true;
    //    unitsDeadLabel.GetComponent<TweenScale>().enabled = true;

    //    Master.WaitAndDo(1f, () =>
    //    {
    //        unitsDeadLabel.color = Color.white;
    //        unitsDeadLabel.transform.localScale = new Vector3(1, 1, 1);
    //        unitsDeadLabel.GetComponent<TweenColor>().enabled = false;
    //        unitsDeadLabel.GetComponent<TweenScale>().enabled = false;

    //    }, this);

    //}


}
