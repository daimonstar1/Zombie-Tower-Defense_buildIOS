using UnityEngine;
using System.Collections;

public class LevelSelectDialog : DialogController
{

  private UILabel levelTitleLabel;
  private int levelIndex;
  UITexture bgLevel;
  bool isUnlockHardMode;
  bool isUnlockHardModePreLevel;
  bool isCanClick = false;

  //for boost exp
  GameObject timeRemainingBoostingEXP;
  UILabel timeCountdownBoostingEXP;
  UITexture expBoostIcon;
  GameObject boostEXPButton;

  public override void AssignObjects ()
  {

    levelTitleLabel = Master.GetChildByName (gameObject, "Title").GetComponent<UILabel> ();
    bgLevel = Master.GetChildByName (gameObject, "BGLevel").GetComponent<UITexture> ();
    //for boost exp
    timeRemainingBoostingEXP = Master.GetChildByName (gameObject, "TimeRemainingBoostingEXP");
    timeCountdownBoostingEXP = Master.GetChildByName (gameObject, "TimeCountdownBoostingEXP").GetComponent<UILabel> ();
    expBoostIcon = Master.GetChildByName (gameObject, "EXPBoostIcon").GetComponent<UITexture> ();
    boostEXPButton = Master.GetChildByName (gameObject, "BoostEXPButton");
  }

  public override void OnOpen (string[] agruments = null, System.Action onCloseComplete = null)
  {
    InvokeRepeating ("SetBoostEXP", 0, 1);

    levelIndex = int.Parse (agruments [0]);
    levelTitleLabel.text = "Level " + levelIndex;
    bgLevel.mainTexture = Resources.Load<Texture2D> ("Textures/UI/Dialog/LevelSelect/" + BackgroundController.GetBackgroundIndex (levelIndex));
    if (Master.LevelData.lastLevel >= Master.LevelData.totalLevel) {
      isUnlockHardMode = true;
      if ((Master.LevelData.lastLevelHard + 1) >= levelIndex) {
        isUnlockHardModePreLevel = true;
      }
    }
    SetListUnits ();
    SetListEnemies ();
    SetStars ();

    Master.GetChildByName (gameObject, "HardLockIcon").SetActive (!(isUnlockHardMode && isUnlockHardModePreLevel));

  }

  void OnDestroy ()
  {
    CancelInvoke ();
  }

  public override void OnShowComplete ()
  {
    GetComponent<Animation> ().Play ();
    Master.WaitAndDo (0.5f, () => {
      isCanClick = true;
      if (Master.Tutorial.CheckAndContinueNextStepTutorial (TutorialController.TutorialsIndex.BuildUnitInGameplay)) {
        GameObject buttonSelectNormal = Master.GetChildByName (gameObject, "ButtonSelectGameMode_Normal");
        GameObject arrow = Master.GetChildByName (Master.Tutorial.currentStepGO, "Arrows");
        arrow.transform.position = buttonSelectNormal.transform.position - new Vector3 (0, 0.2f, 0);
      }
    }, this);
  }

  void SetListUnits ()
  {
    UnitDataController.UnitData[] listUnits = Master.UnitData.GetUnlockUnitsAtLevel (levelIndex);
    GameObject pf_unitIcon = Resources.Load<GameObject> ("Prefabs/Characters/Units/Unit_Icon");
    GameObject unitRoot = Master.GetChildByName (gameObject, "ListUnits");
    foreach (UnitDataController.UnitData unitData in listUnits) {
      GameObject unit = NGUITools.AddChild (unitRoot, pf_unitIcon);
      unit.GetComponentInChildren<UITexture> ().mainTexture = Resources.Load<Texture2D> ("Textures/Characters/Units/Unit_" + unitData.UnitID + "/Icon");
      unit.GetComponentInChildren<UITexture> ().depth = 93;
    }
    unitRoot.GetComponent<UIGrid> ().Reposition ();
  }

  void SetListEnemies ()
  {
    EnemyDataController.EnemyData[] listData = Master.EnemyData.GetEnemiesAtLevel (levelIndex);
    GameObject pf_enemyIcon = Resources.Load<GameObject> ("Prefabs/Characters/Enemies/Enemy_Icon");
    GameObject enemiesRoot = Master.GetChildByName (gameObject, "ListEnemies");
    foreach (EnemyDataController.EnemyData enemyData in listData) {
      GameObject enemy = NGUITools.AddChild (enemiesRoot, pf_enemyIcon);
      enemy.GetComponentInChildren<UITexture> ().mainTexture = Resources.Load<Texture2D> ("Textures/Characters/Enemies/Enemy_" + enemyData.EnemyID + "/Icon");
      enemy.GetComponentInChildren<UITexture> ().depth = 93;

      if (listData.Length >= 9) {
        enemy.GetComponentInChildren<UITexture> ().width = 76;
        enemy.GetComponentInChildren<UITexture> ().height = 88;
      }
    }

    if (listData.Length >= 9) {
      enemiesRoot.GetComponent<UIGrid> ().cellWidth = 70;
      enemiesRoot.GetComponent<UIGrid> ().maxPerLine = 5;
    }
    enemiesRoot.GetComponent<UIGrid> ().Reposition ();
  }


  void SetStars ()
  {
    GameObject[] stars = new GameObject[3];
    int starAtLevel = Master.LevelData.GetStarAtLevel (levelIndex);
    for (int i = 0; i < 3; i++) {
      stars [i] = Master.GetChildByName (gameObject, "Star_" + (i + 1));
      if (i < starAtLevel) {
        stars [i].SetActive (true);
      } else {
        stars [i].SetActive (false);
      }
    }
    //if()
  }


  void SetBoostEXP ()
  {
    if (BoostEXPController.Instance.IsBoosting) {
      timeRemainingBoostingEXP.SetActive (true);
      timeCountdownBoostingEXP.text = BoostEXPController.Instance.getTimeRemainingString ();
      expBoostIcon.mainTexture = Resources.Load<Texture2D> ("Textures/UI/x" + (BoostEXPController.Instance.BoostType + 1) + "EXP");
      boostEXPButton.GetComponent<UIButton> ().enabled = false;
      boostEXPButton.GetComponent<Animator> ().enabled = false;
    } else {
      timeRemainingBoostingEXP.SetActive (false);
      expBoostIcon.mainTexture = Resources.Load<Texture2D> ("Textures/UI/buy_exp");
      boostEXPButton.GetComponent<UIButton> ().enabled = true;

      if (!Master.Tutorial.isDoingTutorial) {
        boostEXPButton.GetComponent<Animator> ().enabled = true;
      } else {
        boostEXPButton.GetComponent<Animator> ().enabled = false;
      }
    }
  }

  public void BoostEXPButton_OnClick ()
  {
    Master.UI.ShowDialog ("BuyBoostEXPDialog");
  }


  public void GoButton_OnClick ()
  {
    if (!isCanClick)
      return;
    Master.PlaySoundButtonClick ();

    Master.UIMenu.CloseDialog (gameObject, 0.25f, () => {

      Master.LevelData.currentLevel = levelIndex;
      Master.UIMenu.Transition (() => {
        Application.LoadLevel ("Play");
      });
    });
  }

  public void GoToNormal ()
  {
    if (!isCanClick)
      return;
    GoToGameplay (GameplayController.GameDifficult.Normal);
  }

  public void GoToHard ()
  {
    if (!isCanClick)
      return;
    if (!isUnlockHardMode) {
      Master.UIMenu.ShowDialog ("HardModeAlertDialog", 0.4f);
    } else if (isUnlockHardMode && !isUnlockHardModePreLevel) {
      GameObject d = Master.UIMenu.ShowDialog ("HardModeAlertDialog_2", 0.4f);
      Master.GetChildByName (d, "LevelValue").GetComponent<UILabel> ().text = (levelIndex - 1).ToString ();
    } else {
      GoToGameplay (GameplayController.GameDifficult.Hard);
    }
  }

  void GoToGameplay (GameplayController.GameDifficult gameDifficult)
  {
    Master.PlaySoundButtonClick ();

    if (Master.Stats.Energy > 0) {
      if (Master.Tutorial.isDoingTutorial) {
        Master.Tutorial.currentStepGO.SetActive (false);
      }
      GameplayController.gameDifficult = gameDifficult;

      //Master.Tutorial.CheckAndContinueNextStepTutorial(TutorialController.TutorialsIndex.BuildUnitInGameplay, 1);
      Master.UIMenu.CloseDialog (gameObject, 0.4f, () => {
        Master.UI.Transition (() => {
          Master.LevelData.currentLevel = levelIndex;
          Application.LoadLevel ("Play");
        });

      });
    } else {
      Master.UIMenu.CloseDialog (gameObject, 0.25f, () => {
        Master.UIMenu.ShowDialog (UIController.Dialog.ListDialogs.FillEnergyDialog, 0.3f, new string[] {
          "GoToLevel",
          levelIndex.ToString ()
        });
      });
    }

  }



}
