using UnityEngine;
using System.Collections;

public class UnitLevelExp : MonoBehaviour
{
  bool isUnitUnlocked = false;

  // Use this for initialization
  private string unitID;
  UI2DSprite unitIcon;
  UILabel levelLabel;
  UISlider expSlider;
  UnitDataController.UnitData unitData;

  float expGot;
  float maxExp;
  float currentExp;
  int currentLevel;
  public bool isStartIncreaseExp;
  float stepPerIncreaseExp = 0.008f;

  int levelIncrease = 0;
  int levelIncreased = 0;
  float expRemaining = 0;
  float expSliderValueUpTo = 0;
  UITexture forceground;

  void Awake ()
  {
    //AssignObjects();
  }

  void AssignObjects ()
  {
    if (Master.UnitData.CheckUnitUnlocked (unitID)) {
      isUnitUnlocked = true;
    }
    unitIcon = Master.GetChildByName (gameObject, "UnitIcon").GetComponent<UI2DSprite> ();
    levelLabel = Master.GetChildByName (gameObject, "LevelLabel").GetComponent<UILabel> ();
    expSlider = Master.GetChildByName (gameObject, "ExpSlider").GetComponent<UISlider> ();
    forceground = Master.GetChildByName (gameObject, "Forceground").GetComponent<UITexture> ();
    forceground.mainTexture = Resources.Load<Texture2D> ("Textures/UI/slider_forceground1");

  }

  // Update is called once per frame
  void Update ()
  {
    //this is very complexity to explain =))
    if (isStartIncreaseExp && isUnitUnlocked) {
      forceground.mainTexture = Resources.Load<Texture2D> ("Textures/UI/slider_forceground_increasing");
      expSlider.value += stepPerIncreaseExp;

      if (levelIncreased < levelIncrease) {
        if (expSlider.value >= 1) {
          Master.Audio.PlaySound ("snd_levelUp");
          GameObject levelUpEffect = NGUITools.AddChild (gameObject, Resources.Load<GameObject> ("Prefabs/Effects/Effect_LevelUp"));
          levelUpEffect.transform.position = transform.position;
          Destroy (levelUpEffect, 1f);
          levelIncreased++;
          expSlider.value = 0;
        }
      }

      if (levelIncreased > levelIncrease) {
        levelIncreased = levelIncrease;
      }

      if (levelIncreased == levelIncrease && expSlider.value >= expSliderValueUpTo) {
        expSlider.value = expSliderValueUpTo;
        isStartIncreaseExp = false;
        FindObjectOfType<LevelCompleteDialog> ().unitExpFinishIncreasement [unitID] = true;
        forceground.mainTexture = Resources.Load<Texture2D> ("Textures/UI/slider_forceground1");
      }
    }

    levelLabel.text = (unitData.Level + levelIncreased).ToString ();
  }

  public void SetAttribute (string unitID, float expGot)
  {

    this.unitID = unitID;
    this.expGot = expGot;
    AssignObjects ();
    unitData = Master.UnitData.GetUnitDataWithUpgradeByID (unitID);
    SetUnitIcon ();
    SetExpLevel ();
  }

  public void StartIncreaseExp ()
  {
    int[] resultOfIncreaseExp = Master.UnitData.IncreaseExp (expGot, unitID);
    levelIncrease = resultOfIncreaseExp [0];
    expRemaining = resultOfIncreaseExp [1];
    float finalMaxExp = Master.UnitData.GetMaxExpAtLevel (unitData.Level + levelIncrease);
    Debug.Log (finalMaxExp);
    if (levelIncrease > 0) {
      expSliderValueUpTo = (float)(expRemaining / finalMaxExp);
    } else {
      expSliderValueUpTo = (float)((currentExp + expGot) / maxExp);
    }


    isStartIncreaseExp = true;

  }

  void SetUnitIcon ()
  {
    if (isUnitUnlocked) {
      unitIcon.sprite2D = Resources.Load<Sprite> ("Textures/Characters/Units/Unit_" + unitID + "/Icon");
    } else {
      unitIcon.sprite2D = Resources.Load<Sprite> ("Textures/Characters/Units/unit_lock");
    }
  }

  void SetExpLevel ()
  {
    maxExp = Master.UnitData.GetMaxExpAtLevel (unitData.Level);
    currentExp = unitData.Exp;
    expSlider.value = currentExp / maxExp;
    if (!isUnitUnlocked) {
      expSlider.value = 0;
      levelLabel.text = "0";
    }
  }

}
