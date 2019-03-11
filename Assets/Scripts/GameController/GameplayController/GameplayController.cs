using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using CodeStage.AntiCheat.ObscuredTypes;

public class GameplayController : MonoBehaviour
{
  [HideInInspector]
  public GameObject gameplayRoot;

  [HideInInspector]
  public Camera camera;

  // Use this for initialization
  [HideInInspector]
  public GameObject unitsRoot;
  [HideInInspector]
  public GameObject enemiesRoot;
  [HideInInspector]
  public GameObject skillsRoot;

  public Transform[] outOfScreenPos = new Transform[4];
  //0: left, 1:top, 2:right, 3:bottom

  public ObscuredInt gold;
  public ObscuredInt zombiesEscaped = 0;
  public ObscuredInt unitsDead = 0;
  public ObscuredBool isBlockUnitSelectByEnemy;

  public Dictionary<string, float> expOfUnits = new Dictionary<string, float> ();
  //<UnitID, Exp>

  public enum GameDifficult
  {
    Normal,
    Hard
  }

  public static GameDifficult gameDifficult = GameDifficult.Normal;

  [HideInInspector]
  public float currentTimeScale = 1;

  void Awake ()
  {
    if (Master.Gameplay == null) {
      Master.Gameplay = this;
    }

    gameplayRoot = GameObject.Find ("Gameplay Root");
    camera = Master.GetChildByName (gameplayRoot, "Camera").GetComponent<Camera> ();
    unitsRoot = Master.GetChildByName (gameplayRoot, "Units");
    enemiesRoot = Master.GetChildByName (gameplayRoot, "Enemies");
    skillsRoot = Master.GetChildByName (gameplayRoot, "Skills");

    Master.UnitData.LoadUnitData ();
    Master.SkillData.LoadSkillData ();
    Master.UnitData.LoadUnitAvaiable ();
    Master.SkillData.LoadSkillsAvaiable ();

  }

  void Start ()
  {
    Time.timeScale = currentTimeScale;
    //Master.Ad.Admob.HideBanner();
    SetUnitSelect ();
    SetSkillSelect ();
    gold = Master.Level.currentLevelData.InitialGold;
    Master.Stats.Energy--;
    Master.Stats.TimesPlay++;
    Master.isLevelComplete = false;
    Master.isGameStart = false;
    GetOutOfScreenPos ();

    if (FindObjectOfType<Transition> () != null) {
      FindObjectOfType<Transition> ().tempOnComplete = FirstLoadLevel;
    } else {
      FirstLoadLevel ();
    }

    InvokeRepeating ("GoldController", StatsController.timePerGold, StatsController.timePerGold);
    InvokeRepeating ("CheckLevelComplete", 1, 1);
    SetExpOfUnit ();


  }

  void OnDestroy ()
  {
    Debug.Log ("Destroy");
    Master.Touch.RemoveAllEvent ();
    Resources.UnloadUnusedAssets ();
    CancelInvoke ();
    StopAllCoroutines ();
  }

  // Update is called once per frame
  void Update ()
  {
    if (Input.GetKeyDown (KeyCode.Space)) {
      InstantWin ();
    }

    if (Input.GetKeyDown (KeyCode.L)) {
      InstantLost ();
    }
  }


  public void FirstLoadLevel ()
  {
    Master.UIGameplay.ShowLevelTitle (() => {
      if (Master.Tutorial.CheckAndContinueNextStepTutorial (TutorialController.TutorialsIndex.BuildUnitInGameplay)) {
        return;
      }

      if (Master.Level.currentLevelData.LevelIndex == 2) {
        if (Master.Tutorial.CheckAndStartTutorial (TutorialController.TutorialsIndex.UseSkill)) {
          return;
        }
      }

      Master.isLevelComplete = false;
      Master.isGameStart = true;
      Master.Level.StartInitEnenmy ();
    });
  }

  public void SetUnitSelect ()
  {
    GameObject unitSelectGrid = Master.GetChildByName (gameplayRoot, "UnitSelectGrid");

    //not show lock

    //foreach (UnitDataController.UnitData unitData in Master.UnitData.listUnitAvailable)
    //{
    //    GameObject pf_unitSelect = Master.GetGameObjectInPrefabs("Characters/Units/UnitSelect");
    //    GameObject obj_unitSelect = NGUITools.AddChild(unitSelectGrid, pf_unitSelect);
    //    obj_unitSelect.GetComponentInChildren<UnitSelect>().unitData = unitData;
    //    obj_unitSelect.GetComponentInChildren<UnitSelect>().SetInfo();
    //}

    //show lock

    foreach (UnitDataController.UnitData unitData in Master.UnitData.listUnitData) {
      GameObject pf_unitSelect = Master.GetGameObjectInPrefabs ("Characters/Units/UnitSelect");
      GameObject obj_unitSelect = NGUITools.AddChild (unitSelectGrid, pf_unitSelect);
      obj_unitSelect.GetComponentInChildren<UnitSelect> ().unitData = unitData;
      if (unitData.UnlockAtLevel <= Master.LevelData.currentLevel) {
        obj_unitSelect.GetComponentInChildren<UnitSelect> ().isLock = false;
      } else {
        obj_unitSelect.GetComponentInChildren<UnitSelect> ().isLock = true;
      }
      obj_unitSelect.GetComponentInChildren<UnitSelect> ().SetInfo ();
    }

    unitSelectGrid.GetComponent<UIGrid> ().Reposition ();
  }

  public void SetSkillSelect ()
  {
    GameObject skillSelectGrid = Master.GetChildByName (gameplayRoot, "SkillSelectGrid");

    //not show lock
    //foreach (SkillDataController.SkillData skillData in Master.SkillData.listSkillsAvaiable)
    //{
    //    GameObject pf_skillSelect = Master.GetGameObjectInPrefabs("Skills/SkillSelect");
    //    GameObject obj_skillSelect = NGUITools.AddChild(skillSelectGrid, pf_skillSelect);
    //    obj_skillSelect.GetComponentInChildren<SkillSelect>().skillData = skillData;
    //    obj_skillSelect.GetComponentInChildren<SkillSelect>().SetInfo();
    //}


    ////show lock
    foreach (SkillDataController.SkillData skillData in Master.SkillData.listSkillsData) {
      GameObject pf_skillSelect = Master.GetGameObjectInPrefabs ("Skills/SkillSelect");
      GameObject obj_skillSelect = NGUITools.AddChild (skillSelectGrid, pf_skillSelect);
      obj_skillSelect.GetComponentInChildren<SkillSelect> ().skillData = skillData;
      if (skillData.UnlockAtLevel <= Master.LevelData.lastLevel + 1) {
        obj_skillSelect.GetComponentInChildren<SkillSelect> ().isLock = false;
      } else {
        obj_skillSelect.GetComponentInChildren<SkillSelect> ().isLock = true;
      }
      obj_skillSelect.GetComponentInChildren<SkillSelect> ().SetInfo ();
    }

    skillSelectGrid.GetComponent<UIGrid> ().Reposition ();
  }

  void GetOutOfScreenPos ()
  {
    outOfScreenPos [0] = Master.GetChildByName (gameplayRoot, "OutOfScreenLeft").transform;
    outOfScreenPos [1] = Master.GetChildByName (gameplayRoot, "OutOfScreenTop").transform;
    outOfScreenPos [2] = Master.GetChildByName (gameplayRoot, "OutOfScreenRight").transform;
    outOfScreenPos [3] = Master.GetChildByName (gameplayRoot, "OutOfScreenBottom").transform;
  }

  public void CheckLevelComplete ()
  {
    if (!Master.isGameStart || Master.isLevelComplete)
      return;
    if ((Master.Level.totalSequenceIndex >= Master.Level.totalSequences && !Master.Lane.isExistCharacterByTagInAllLane ("Enemy"))
        || (zombiesEscaped > 0)) {
      Debug.LogError ("Master.Level.totalSequenceIndex=" + Master.Level.totalSequenceIndex);
      Debug.LogError ("Master.Level.totalSequences=" + Master.Level.totalSequences);
      Debug.LogError ("!Master.Lane.isExistCharacterByTagInAllLane(\"Enemy\")=" + !Master.Lane.isExistCharacterByTagInAllLane ("Enemy"));
      Debug.LogError ("zombiesEscaped=" + zombiesEscaped);
      Master.isLevelComplete = true;
      Master.isGameStart = false;
      Master.WaitAndDo (2f, () => {
        Master.UI.ShowDialog ("LevelCompleteDialog", 1f);
				
      }, this);
    }
  }

  public void ChangeGameSpeed ()
  {
    if (currentTimeScale == 1) {
      currentTimeScale = 2;
    } else {
      currentTimeScale = 1;
    }
    Time.timeScale = currentTimeScale;
  }

  public void SetExpOfUnit ()
  {
    expOfUnits.Clear ();
    foreach (UnitDataController.UnitData unitData in Master.UnitData.listUnitData) {
      expOfUnits.Add (unitData.UnitID, 0);
    }
  }

  public void UnitGetExp (string unitID, float exp)
  {
    if (BoostEXPController.Instance.IsBoosting) {
      exp = exp * (BoostEXPController.Instance.BoostType + 1);
    }
    expOfUnits [unitID] += exp;
  }

  public void InstantWin ()
  {
    Master.Level.totalSequenceIndex = Master.Level.totalSequences;
    GameObject[] gameObjects = GameObject.FindGameObjectsWithTag ("Enemy");
    foreach (GameObject go in gameObjects) {
      Master.Lane.RemoveCharacterAtLane (go.GetComponent<EnemyController> ().status.CurrentLane, go);
    }
  }

  public void InstantLost ()
  {
    zombiesEscaped++;
    Debug.LogError ("InstantLost--zombiesEscaped=" + zombiesEscaped);
  }



  void GoldController ()
  {
    if (Master.isGameStart) {
      gold += 1;
    }
  }

  public GameObject[] GetListUnitOnScene ()
  {
    return GameObject.FindGameObjectsWithTag ("Unit");
  }


  public void ResumeGame ()
  {
    Time.timeScale = currentTimeScale;
  }

  public void PauseGame ()
  {
    Time.timeScale = 0;
  }

  public void GoToMenu ()
  {
    Master.UI.Transition (() => {
      Time.timeScale = 1;
      Application.LoadLevel ("Menu");
    });
  }

  public void GoToNextLevel ()
  {
    Master.UI.Transition (() => {
      Master.LevelData.currentLevel++;
      Time.timeScale = 1;
      Application.LoadLevel ("Play");
    });


  }

  public void ReplayGame ()
  {
    if (Master.Stats.Energy > 0) {
      Master.UI.Transition (() => {
        Time.timeScale = 1;
        Application.LoadLevel (Application.loadedLevel);
      });

    } else {
      Master.UI.ShowDialog (UIController.Dialog.ListDialogs.FillEnergyDialog, 0.5f, new string[] { "ReplayScene" });
    }
  }

  public bool CheckEnergy ()
  {
    if (Master.Stats.Energy > 0) {
      return true;
    } else {
      Master.UI.ShowDialog (UIController.Dialog.ListDialogs.FillEnergyDialog, 0.5f, new string[] { "ReplayScene" });
      return false;
    }
  }


}

