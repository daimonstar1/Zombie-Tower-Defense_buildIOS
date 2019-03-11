using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;

using System;

public class LevelCompleteDialog : DialogController
{

	// Use this for initialization
	private UITexture completeStatusTexture;
	private GameObject[] stars = new GameObject[3];
	private GameObject starReward;
	private UILabel starRewardLabel;
	private GameObject gemReward;
	private UILabel gemRewardLabel;
	private GameObject allButtons;
	private UIGrid buttonsGrid;
	private GameObject nextButton;
	private GameObject shareButtons;
	private UIGrid unitLevelExp;
	private GameObject pf_unitLevelExpItem;
	List<UnitLevelExp> listUnitLevelExp = new List<UnitLevelExp> ();
	bool isVictory = true;
	bool isPlayedThisLevel = false;
	bool isCanClick = false;

	int star = 1;
	int gemRewardValue = 0;
	int starRewardValue = 0;

	public Dictionary<string, bool> unitExpFinishIncreasement = new Dictionary<string, bool> ();

	//for boost exp
	GameObject timeRemainingBoostingEXP;
	UILabel timeCountdownBoostingEXP;
	UITexture expBoostIcon;
	GameObject boostEXPButton;

	GameObject hardModeIcon;


	public override void AssignObjects ()
	{
		isAutoPlaySound = false;
		completeStatusTexture = Master.GetChildByName (gameObject, "CompleteStatus").GetComponent<UITexture> ();
		for (int i = 0; i < 3; i++) {
			stars [i] = Master.GetChildByName (gameObject, "Star_" + (i + 1));
			stars [i].SetActive (false);
			if (GameplayController.gameDifficult == GameplayController.GameDifficult.Hard) {
				stars [i].GetComponent<UITexture> ().color = new Color (255 / 255, (float)69 / 255, 0, 1);
			}
		}

		starReward = Master.GetChildByName (gameObject, "StarReward");
		starRewardLabel = Master.GetChildByName (gameObject, "StarRewardLabel").GetComponent<UILabel> ();
		gemReward = Master.GetChildByName (gameObject, "GemReward");
		gemRewardLabel = Master.GetChildByName (gameObject, "GemRewardLabel").GetComponent<UILabel> ();
		allButtons = Master.GetChildByName (gameObject, "AllButtons");

		buttonsGrid = Master.GetChildByName (gameObject, "Buttons").GetComponent<UIGrid> ();
		nextButton = Master.GetChildByName (gameObject, "NextButton");
		shareButtons = Master.GetChildByName (gameObject, "ShareButtons");

		unitLevelExp = Master.GetChildByName (gameObject, "UnitLevelExp").GetComponent<UIGrid> ();
		pf_unitLevelExpItem = Master.GetGameObjectInPrefabs ("UI/UnitExpLevel_01");

		//for boost exp
		timeRemainingBoostingEXP = Master.GetChildByName (gameObject, "TimeRemainingBoostingEXP");
		timeCountdownBoostingEXP = Master.GetChildByName (gameObject, "TimeCountdownBoostingEXP").GetComponent<UILabel> ();
		expBoostIcon = Master.GetChildByName (gameObject, "EXPBoostIcon").GetComponent<UITexture> ();
		boostEXPButton = Master.GetChildByName (gameObject, "BoostEXPButton");
		hardModeIcon = Master.GetChildByName (gameObject, "HardModeIcon");
		ShowListUnit ();
	}

	public override void OnUpdate ()
	{
		if (isStartCheckExpIncreasementFinish) {
			CheckFinishIncreaseExp ();
		}
	}

	void OnDestroy ()
	{
		CancelInvoke ();
	}

	public override void OnOpen (string[] agruments = null, Action onCloseComplete = null)
	{
		SetBoostEXP ();

		Master.Tutorial.CheckAndFinishTutorial ();

		//stop game
		Time.timeScale = 0;
		starRewardLabel.text = "--";
		gemRewardLabel.text = "--";
		allButtons.transform.localPosition = new Vector3 (allButtons.transform.localPosition.x, -500, 0);
		allButtons.SetActive (false);
		//check is victory
		if (GameplayController.gameDifficult == GameplayController.GameDifficult.Normal) {
			hardModeIcon.SetActive (false);
		} else {
			hardModeIcon.SetActive (true);
		}
		if (Master.Gameplay.zombiesEscaped > 0) {
			isVictory = false;
		}

		if (Master.LevelData.currentLevel <= Master.LevelData.lastLevel) {
			isPlayedThisLevel = true;
		}

		Master.Audio.StopBackgroundMusic ();

		if (isVictory) {
			Master.Stats.TimesLevelComplete++;

			Master.Audio.PlaySound ("snd_victory_2", 0.8f);

			//calculate star
			float percentUnitDead = ((float)Master.Gameplay.unitsDead / Master.Level.currentLevelData.NumberOfUnitsAllowedDead) * 100;
			if (percentUnitDead < 20) {
				star = 3;
			} else if (percentUnitDead >= 20 && percentUnitDead < 45) {
				star = 2;
			}

			//set Status icon
			completeStatusTexture.mainTexture = Resources.Load<Texture2D> ("Textures/UI/Dialog/LevelComplete/victory");

			//save level data
			gemRewardValue = RewardController.GetGemReward (Master.LevelData.currentLevel, star);
			starRewardValue = RewardController.GetStarReward (Master.LevelData.currentLevel, star);
			if (GameplayController.gameDifficult == GameplayController.GameDifficult.Normal) {
				Master.LevelData.SetLastLevel (Master.LevelData.currentLevel);
				Master.LevelData.SetStarAtLevel (Master.LevelData.currentLevel, star);
			} else if (GameplayController.gameDifficult == GameplayController.GameDifficult.Hard) {
				Master.LevelData.lastLevelHard = Master.LevelData.currentLevel;
				Master.LevelData.SetStarAtLevelHard (Master.LevelData.currentLevel, star);
			}

			if (Master.LevelData.currentLevel >= Master.LevelData.totalLevel) {
				NGUITools.Destroy (nextButton);
				buttonsGrid.Reposition ();
			}

		} else {
			Master.Audio.PlaySound ("snd_defeat", 0.3f);

			star = 0;
			completeStatusTexture.mainTexture = Resources.Load<Texture2D> ("Textures/UI/Dialog/LevelComplete/defeat");
			starRewardLabel.text = "0";
			gemRewardLabel.text = "0";
			NGUITools.Destroy (nextButton);
			buttonsGrid.Reposition ();
			shareButtons.SetActive (false);
		}
	}

	void ShowListUnit ()
	{
		foreach (UnitDataController.UnitData unitData in Master.UnitData.listUnitData) {
			GameObject go_unitExpLevel = NGUITools.AddChild (unitLevelExp.gameObject, pf_unitLevelExpItem);
			go_unitExpLevel.GetComponent<UnitLevelExp> ().SetAttribute (unitData.UnitID, Master.Gameplay.expOfUnits [unitData.UnitID]);
			go_unitExpLevel.name = "UnitLevelExp_" + unitData.UnitID;
			listUnitLevelExp.Add (go_unitExpLevel.GetComponent<UnitLevelExp> ());
		}
	}

	public override void OnShowComplete ()
	{
		if (isVictory) {
			ShowStar (() => {
				Master.WaitAndDo (0.7f, () => {
					Master.Audio.PlaySound ("snd_getReward");
					starRewardLabel.text = starRewardValue.ToString ();
					gemRewardLabel.text = gemRewardValue.ToString ();

					Master.WaitAndDo (0.5f, () => {
						StartIncreaseUnitExp (() => {
							Master.WaitAndDo (0.5f, () => {
								if (isVictory) {
									if (!isPlayedThisLevel) {
										Master.UnitData.CheckUnitUnlock (() => {
											Master.SkillData.CheckSkillUnlock (() => {
												Master.instance.CheckShowRatingDialog (() => {
													ShowAllButtons ();
												});
											});
										});
									} else {
										Master.instance.CheckShowRatingDialog (() => {
											ShowAllButtons ();
										});
									}
								} else {
									ShowAllButtons ();
								}
							}, this, true);
						});


					}, this, true);


				}, this, true);


			});
		} else {
			ShowAllButtons ();
		}
	}

	void ShowStar (System.Action onComplete = null)
	{
		float timePerShowStar = 0.5f;
		if (star > 0) {
			Master.WaitAndDo (0.1f, () => {
				doShowStar (0);
				if (star > 1) {
					Master.WaitAndDo (timePerShowStar, () => {
						doShowStar (1);
						if (star > 2) {
							Master.WaitAndDo (timePerShowStar, () => {
								doShowStar (2);
								if (onComplete != null) {
									onComplete ();
								}
							}, this, true);
						} else {
							if (onComplete != null) {
								onComplete ();
							}
						}
					}, this, true);
				} else {
					if (onComplete != null) {
						onComplete ();
					}
				}
			}, this, true);
		}
	}



	void ShowAllButtons ()
	{
		PlayShowHideSound ();
		allButtons.SetActive (true);
		allButtons.transform.DOLocalMoveY (-227, 0.2f).SetUpdate (true).OnComplete (() => {
			isCanClick = true;
			CheckTutorial ();
		});

		if (Master.LevelData.currentLevel >= Master.LevelData.totalLevel) {
			//if (PlayerPrefs.GetInt("isShowedUnlockedHardLevel", 0) == 0)
			//{
			Master.UI.ShowDialog ("UnlockedHardLevelDialog", 0.3f, null, null, null, ShowDialogType.Center);
			PlayerPrefs.SetInt ("isShowedUnlockedHardLevel", 1);
			//}

		}
	}

	void doShowStar (int starIndex)
	{
		Master.Effect.CreateEffect ("Effect_Star", stars [starIndex].transform.localPosition);
		stars [starIndex].SetActive (true);
		stars [starIndex].transform.localScale = new Vector3 (0.1f, 0.1f, 0.1f);
		stars [starIndex].transform.DOScale (new Vector3 (1.4f, 1.4f, 1.4f), 0.1f).SetUpdate (true).OnComplete (() => {
			Master.Audio.PlaySound ("snd_showStar", 0.5f);
			stars [starIndex].transform.DOScale (new Vector3 (1, 1, 1), 0.07f).SetUpdate (true).OnComplete (() => {

			});
		});

		Master.Effect.ShakeCamera (4);
	}

	System.Action onIncreaseExpComplete;
	bool isStartCheckExpIncreasementFinish = false;

	void StartIncreaseUnitExp (System.Action onComplete)
	{
		unitExpFinishIncreasement.Clear ();

		onIncreaseExpComplete = onComplete;
		foreach (string unitID in Master.Gameplay.expOfUnits.Keys) {
			if (Master.Gameplay.expOfUnits [unitID] > 0) {
				unitExpFinishIncreasement.Add (unitID, false);
			}
		}
		foreach (UnitLevelExp unitExp in listUnitLevelExp) {
			unitExp.StartIncreaseExp ();
		}
		isStartCheckExpIncreasementFinish = true;
	}

	void CheckFinishIncreaseExp ()
	{
		bool check = true;
		foreach (bool isFinish in unitExpFinishIncreasement.Values) {
			if (!isFinish) {
				check = false;
				break;
			}
		}
		if (check) {
			//Master.WaitAndDo(0.5f, () =>
			//{
			onIncreaseExpComplete ();
			onIncreaseExpComplete = null;
			isStartCheckExpIncreasementFinish = false;
			//    }, this, true);
		}
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
			boostEXPButton.SetActive (false);
		}
		Master.WaitAndDo (1, () => {
			SetBoostEXP ();
		}, this, true);
	}

	public void CheckTutorial ()
	{
		bool returnToMenu = false;

		//upgrade unit
		if (Master.Stats.Star >= 150 && !Master.Tutorial.IsTutorialDone (TutorialController.TutorialsIndex.UpgradeStatsOfUnit)) {
			returnToMenu = true;
		}

		//upgrade skill
		if (Master.Stats.Gem >= 10 && !Master.Tutorial.IsTutorialDone (TutorialController.TutorialsIndex.UpgradeSkill)) {
			returnToMenu = true;
		}

		//get free reward
		if ((Master.LevelData.lastLevel + 1) >= FreeRewardController.levelCanGetFreeReward && !Master.Tutorial.IsTutorialDone (TutorialController.TutorialsIndex.GetFreeReward)) {
			returnToMenu = true;
		}

		//get quest reward
		if (Master.QuestData.isHaveQuestComplete () && !Master.Tutorial.IsTutorialDone (TutorialController.TutorialsIndex.GetRewardOfQuest)) {
			if (Master.QuestData.isHaveQuestComplete ()) {
				returnToMenu = true;
			}
		}

		if (returnToMenu) {
			Vector3 arrowPos = Master.GetChildByName (gameObject, "MenuButton").transform.position - new Vector3 (0.3f, 0, 0);
			Master.Tutorial.StartTutorial (TutorialController.TutorialsIndex.ReturnToMenu);
			Master.GetChildByName (Master.Tutorial.currentStepGO, "Arrows").transform.position = arrowPos;
		}

	}

	public void MenuButton_OnClick ()
	{
		if (!isCanClick)
			return;

		Master.PlaySoundButtonClick ();
		Master.Tutorial.CheckAndFinishTutorial (TutorialController.TutorialsIndex.ReturnToMenu);

		Close (() => {
			Master.Gameplay.GoToMenu ();
		});
	}

	public void NextButton_OnClick ()
	{
		if (!isCanClick)
			return;

		Master.PlaySoundButtonClick ();
		Master.Tutorial.CheckAndFinishTutorial (TutorialController.TutorialsIndex.ReturnToMenu);
		Close (() => {
			Master.Gameplay.GoToNextLevel ();
		});
	}

	public void ReplayButton_OnClick ()
	{
		if (!isCanClick)
			return;

		Master.PlaySoundButtonClick ();
		Master.Tutorial.CheckAndFinishTutorial (TutorialController.TutorialsIndex.ReturnToMenu);
		Close (() => {
			Master.Gameplay.ReplayGame ();
		});
	}

	public void ShareFacebookButton_OnClick ()
	{
		if (!isCanClick)
			return;

		Master.PlaySoundButtonClick ();
		FacebookController.Instance.ShareLink ("", FacebookController.Instance.titleShareLink, FacebookController.Instance.descriptionShareLink, FacebookController.Instance.linkImageShareLevelComplete);
	}

	public void ShareTwitterButton_OnClick ()
	{
		if (!isCanClick)
			return;

		Master.PlaySoundButtonClick ();

	}


}
