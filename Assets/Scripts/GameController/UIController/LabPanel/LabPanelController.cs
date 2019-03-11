using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LabPanelController : MonoBehaviour
{
	GameObject freeReward;
	GameObject freeRewardTitle;
	UITexture soundTexture;
	UITexture musicTexture;
	GameObject alertQuestCompleteIcon;

	//    GameObject zombie;

	GameObject[] unitInLab;
	List<Transform> listPositionsOfUnit = new List<Transform> ();

	//for boost exp
	GameObject timeRemainingBoostingEXP;
	UILabel timeCountdownBoostingEXP;
	UITexture expBoostIcon;
	GameObject boostEXPButton;

	void Awake ()
	{
		AssignObject ();
	}
	// Use this for initialization
	void Start ()
	{
		InvokeRepeating ("CheckFreeReward", 0, 2);
		InvokeRepeating ("SetBoostEXP", 0, 1);
		SetUnits ();
	}


	void OnDestroy ()
	{
		CancelInvoke ();
	}

	void AssignObject ()
	{
		freeReward = Master.GetChildByName (gameObject, "FreeReward");
		freeRewardTitle = Master.GetChildByName (gameObject, "FreeRewardTitle");
		soundTexture = Master.GetChildByName (gameObject, "Sound").GetComponent<UITexture> ();
		musicTexture = Master.GetChildByName (gameObject, "Music").GetComponent<UITexture> ();
		alertQuestCompleteIcon = Master.GetChildByName (gameObject, "AlertQuestCompleteIcon");
//        zombie = Master.GetChildByName(gameObject, "Zombie");
		unitInLab = new GameObject[8];
		for (int i = 0; i < 8; i++) {
			unitInLab [i] = Master.GetGameObjectInPrefabs ("Characters/Units/UnitInLab_" + (i + 1));
		}
		foreach (Transform t in Master.GetChildByName(gameObject, "UnitPositions").transform) {
			listPositionsOfUnit.Add (t);
		}

		//for boost exp
		timeRemainingBoostingEXP = Master.GetChildByName (gameObject, "TimeRemainingBoostingEXP");
		timeCountdownBoostingEXP = Master.GetChildByName (gameObject, "TimeCountdownBoostingEXP").GetComponent<UILabel> ();
		expBoostIcon = Master.GetChildByName (gameObject, "EXPBoostIcon").GetComponent<UITexture> ();
		boostEXPButton = Master.GetChildByName (gameObject, "BoostEXPButton");

	}

	public void OnOpen ()
	{
		SetSettings ();
		CheckAlertQuestComplete ();
//        zombie.GetComponent<FrameAnimation>().Play();
//		Master.GetChildByName (gameObject, "Scientist").GetComponent<FrameAnimation> ().Play ();
		SetUnits ();

		CheckTutorial ();

	}

	void CheckTutorial ()
	{
		//build unit
		Master.Tutorial.CheckAndStartTutorial (TutorialController.TutorialsIndex.BuildUnitInGameplay);

		//upgrade unit
		if (!Master.Tutorial.isDoingTutorial) {
			if (Master.Stats.Star >= 150) {
				Vector3 arrowPos = Master.GetChildByName (gameObject, "UnitInLab_01").transform.position - new Vector3 (0, 0.12f, 0);
				Master.Tutorial.CheckAndStartTutorial (TutorialController.TutorialsIndex.UpgradeStatsOfUnit, arrowPos);
				Master.GetChildByName (Master.Tutorial.currentStepGO, "Arrows").transform.position = Master.GetChildByName (Master.UIMenu.panels [6], "UnitInLab_01").transform.position - new Vector3 (0, 0.1f, 0);
			}
		}

		//upgrade skill
		if (!Master.Tutorial.isDoingTutorial) {
			if (Master.Stats.Gem >= 10 && (Master.LevelData.lastLevel + 1) >= Master.SkillData.skill_01_data.UnlockAtLevel) {
				Master.Tutorial.CheckAndStartTutorial (TutorialController.TutorialsIndex.UpgradeSkill);
			}
		}

		//get reward quest
		if (!Master.Tutorial.isDoingTutorial) {
			if (Master.QuestData.isHaveQuestComplete ()) {
				Master.Tutorial.CheckAndStartTutorial (TutorialController.TutorialsIndex.GetRewardOfQuest);
			}
		}

		//get free reward
		if (!Master.Tutorial.isDoingTutorial) {
			if ((Master.LevelData.lastLevel + 1) >= FreeRewardController.levelCanGetFreeReward) {
				Master.Tutorial.CheckAndStartTutorial (TutorialController.TutorialsIndex.GetFreeReward);
			}
		}




	}

	void SetUnits ()
	{

		//delete current unit
		foreach (Transform t in listPositionsOfUnit) {
			foreach (Transform child in t) {
				Destroy (child.gameObject);
			}
		}

		UnitDataController.UnitData[] unitsData = Master.UnitData.GetUnlockUnitsAtLevel (Master.LevelData.lastLevel + 1);
		Debug.LogError ("Master.LevelData.lastLevel=" + Master.LevelData.lastLevel + ",unitsData=" + unitsData.Length);
		for (int i = 0; i < unitsData.Length; i++) {
			GameObject unit = NGUITools.AddChild (listPositionsOfUnit [i].gameObject, unitInLab [i]);
			unit.GetComponent<UnitInLabController> ().Active (unitsData [i].UnitID);
			unit.transform.Find ("anim").GetComponent<MeshRenderer> ().sortingOrder = 100 - (int)unit.transform.parent.localPosition.y;
			unit.name = "UnitInLab_" + unitsData [i].UnitID;
		}
	}

	void CheckFreeReward ()
	{
		if ((Master.LevelData.lastLevel + 1) >= FreeRewardController.levelCanGetFreeReward) {
			freeReward.transform.Find ("BG").GetComponent<UITexture> ().color = Color.white;
			freeReward.transform.Find ("BG").GetComponent<BoxCollider2D> ().enabled = true;
			freeReward.SetActive (true);
		} else {
			freeReward.transform.Find ("BG").GetComponent<UITexture> ().color = Color.gray;
			freeReward.SetActive (true);
			freeReward.transform.Find ("BG").GetComponent<BoxCollider2D> ().enabled = false;
//			freeReward.SetActive (false);
		}

		if (FreeRewardController.IsCanGetFreeReward ()) {
			freeRewardTitle.GetComponent<MoveObject> ().enabled = true;
		} else {
			freeRewardTitle.GetComponent<MoveObject> ().enabled = false;
		}
	}

	public void FreeRewardButton_OnClick ()
	{
		if (Master.Tutorial.isDoingTutorial && Master.Tutorial.currentTutorialIndex == TutorialController.TutorialsIndex.GetFreeReward) {
			Master.Tutorial.currentStepGO.SetActive (false);
		}
		Master.PlaySoundButtonClick ();
		Master.UI.ShowDialog ("FreeRewardDialog", 0.5f);
	}

	void SetBoostEXP ()
	{
		//if (Master.LevelData.lastLevel >= BoostEXPController.levelCanBuyBoostEXP)
		//{
		if (BoostEXPController.Instance.IsBoosting) {
			timeRemainingBoostingEXP.SetActive (true);
			timeCountdownBoostingEXP.text = BoostEXPController.Instance.getTimeRemainingString ();
			expBoostIcon.mainTexture = Resources.Load<Texture2D> ("Textures/UI/x" + (BoostEXPController.Instance.BoostType + 1) + "EXP");
			boostEXPButton.GetComponent<UIButton> ().enabled = false;
			boostEXPButton.GetComponent<Animator> ().enabled = false;
		} else {
			timeRemainingBoostingEXP.SetActive (false);
			expBoostIcon.mainTexture = Resources.Load<Texture2D> ("Textures/UI/EXP_boost");
			boostEXPButton.GetComponent<UIButton> ().enabled = true;

			if (!Master.Tutorial.isDoingTutorial) {
//				boostEXPButton.GetComponent<Animator> ().enabled = true;
			} else {
//				boostEXPButton.GetComponent<Animator> ().enabled = false;
			}
		}
		//}
		//else
		//{
		//    boostEXPButton.SetActive(false);
		//}
	}

	public void BoostEXPButton_OnClick ()
	{
		Master.UI.ShowDialog ("BuyBoostEXPDialog");
	}

	public void SetSettings ()
	{
		if (Master.Audio.isSoundOn) {
			Master.GetChildByName (soundTexture.gameObject, "X").SetActive (false);
		} else {
			Master.GetChildByName (soundTexture.gameObject, "X").SetActive (true);
		}

		if (Master.Audio.isBackgroundMusicOn) {
			Master.GetChildByName (musicTexture.gameObject, "X").SetActive (false);
		} else {
			Master.GetChildByName (musicTexture.gameObject, "X").SetActive (true);
		}

	}

	public void ToggleAudioSettingButton_OnClick (GameObject go)
	{
		Master.PlaySoundButtonClick ();
		string goName = go.name;
		if (goName == "Sound") {
			Master.Audio.ToggleSound ();
		} else if (goName == "Music") {
			Master.Audio.ToggleBackgroundMusic ();
		}

		SetSettings ();
	}

	void CheckAlertQuestComplete ()
	{
		if (Master.QuestData.isHaveQuestComplete ()) {
			alertQuestCompleteIcon.SetActive (true);
			alertQuestCompleteIcon.transform.parent.Find ("icon").gameObject.SetActive (false);
		} else {
			alertQuestCompleteIcon.SetActive (false);
			alertQuestCompleteIcon.transform.parent.Find ("icon").gameObject.SetActive (true);
		}
	}

	public void OpenMap ()
	{
		if (Master.Tutorial.isDoingTutorial) {
			Master.Tutorial.currentStepGO.SetActive (false);
		}
		Master.UIMenu.OpenPanel (0);
	}

}
