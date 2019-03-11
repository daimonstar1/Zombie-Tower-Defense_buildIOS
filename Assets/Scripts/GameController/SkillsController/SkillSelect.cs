using UnityEngine;
using System.Collections;
using DG.Tweening;

public class SkillSelect : MonoBehaviour
{

	// Use this for initialization
	public SkillDataController.SkillData skillData;
	private SpriteRenderer icon;
	private Transform countdown;
	private bool isCountingdown;
	private bool isSelected;
	private bool isCanSelect;
	public bool isLock;
	private GameObject pf_skill;
	private GameObject skill;

	void Start ()
	{
		isCanSelect = true;
		pf_skill = Master.GetSkillPrefabByID (skillData.SkillID);


	}

	// Update is called once per frame
	void Update ()
	{
      
	}

	public void SetInfo ()
	{
		icon = Master.GetChildByName (gameObject, "Icon").GetComponent<SpriteRenderer> ();
		countdown = Master.GetChildByName (gameObject, "Countdown").GetComponent<Transform> ();
		if (!isLock) {
			icon.sprite = Resources.Load<Sprite> ("Textures/Skills/Skill_" + skillData.SkillID + "/Skill_" + skillData.SkillID + "_Icon");
		} else {
			icon.sprite = Resources.Load<Sprite> ("Textures/Skills/skill_locked");
			icon.color = new Color (1, 1, 1, 0.7f);
			Destroy (icon.gameObject.GetComponent<UIButton> ());
		}
		countdown.gameObject.SetActive (false);
		AddEvent ();
		if (isLock)
			icon.transform.localScale = Vector3.one * 58;
		else
			icon.transform.localScale = Vector3.one * 70;
	}

	public void AddEvent ()
	{
		if (isLock)
			return;

		Master.Touch.AddTouchEvent (TouchController.TouchType.Touching, () => {
			if (isSelected) {
				skill.GetComponentInChildren<SkillController> ().OnChoosingPosition ();
			}
		});

		Master.Touch.AddTouchEvent (TouchController.TouchType.TouchUp, () => {
			bool isSet = false;
			if (isSelected) {
				if (skill.GetComponentInChildren<SkillController> ().Set ()) {
					StartCountdown ();
					if (Master.Tutorial.CheckAndFinishTutorial (TutorialController.TutorialsIndex.UseSkill)) {
						Master.isLevelComplete = false;
						Master.isGameStart = true;
						Master.Level.StartInitEnenmy ();
					}
					isSet = true;
				}
				if (!isSet) {
					Destroy (skill);
					if (Master.Tutorial.isDoingTutorial && Master.Tutorial.currentTutorialIndex == TutorialController.TutorialsIndex.UseSkill) {
						Master.Tutorial.GoToPreviousStep (0);
					}
				}
				isSelected = false;
			}
		});
	}

	void StartCountdown ()
	{
		isCountingdown = true;
		countdown.gameObject.SetActive (true);
		Master.Effect.ScaleCountDownSkill (countdown, skillData.TimeCountdown, 60, 0, () => {
			isCountingdown = false;
			countdown.gameObject.SetActive (false);
		});
	}

	public void OnTouchIn ()
	{

		if ((!isCanSelect || isCountingdown || isLock || !Master.isGameStart) && (!Master.Tutorial.isDoingTutorial && Master.Tutorial.currentTutorialIndex != TutorialController.TutorialsIndex.UseSkill))
			return;

		if (Master.Tutorial.currentTutorialIndex == TutorialController.TutorialsIndex.UseSkill) {
			Master.Tutorial.GoToNextStep ();
			Master.WaitAndDo (0.7f, () => {
				if (Master.Touch.isTouching) {
					Master.Tutorial.GoToNextStep ();
				}
			}, this);
		}

		isSelected = true;
		Master.Audio.PlaySound ("snd_click");

		if (skillData.SkillID == "01") {
			Master.Lane.ShowUnitPositionsAvailable ();
		}

		skill = NGUITools.AddChild (Master.Gameplay.skillsRoot, pf_skill);
		skill.transform.position = Master.Gameplay.camera.ScreenToWorldPoint (Input.mousePosition);


	}

}
