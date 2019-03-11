using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class FreeRewardDialog : DialogController
{

	public class WatchVideo
	{
		public GameObject root;
		public UILabel title;
		public GameObject valuesRoot;
		public UILabel gemLabel;
		public UILabel starLabel;
		public UIButton actionButton;
		public UILabel actionButtonLabel;
	}

	public WatchVideo watchVideo = new WatchVideo ();


	public class ShareFacebook
	{
		public GameObject root;
		public UILabel title;
		public GameObject valuesRoot;
		public UILabel gemLabel;
		public UILabel starLabel;
		public UIButton actionButton;
		public UILabel actionButtonLabel;
	}

	public ShareFacebook shareFacebook = new ShareFacebook ();

	public WatchVideo watchVideo_Offerwall = new WatchVideo ();



	//UILabel shareFacebook.title ;

	int[] currentRandomReward = new int[2];
	int[] currentShareFacebookReward = new int[2];

	public override void AssignObjects ()
	{
		//PlayerPrefs.DeleteAll();
		watchVideo.root = Master.GetChildByName (gameObject, "WatchVideo");
		watchVideo.title = Master.GetChildByName (watchVideo.root, "Title").GetComponent<UILabel> ();
		watchVideo.valuesRoot = Master.GetChildByName (watchVideo.root, "Values");
		watchVideo.gemLabel = Master.GetChildByName (watchVideo.root, "GemValueLabel").GetComponent<UILabel> ();
		watchVideo.starLabel = Master.GetChildByName (watchVideo.root, "StarValueLabel").GetComponent<UILabel> ();
		watchVideo.actionButton = Master.GetChildByName (watchVideo.root, "WatchVideoButton").GetComponent<UIButton> ();
		watchVideo.actionButtonLabel = Master.GetChildByName (gameObject, "WatchVideoButtonLabel").GetComponent<UILabel> ();

		shareFacebook.root = Master.GetChildByName (gameObject, "ShareFacebook");
		shareFacebook.title = Master.GetChildByName (shareFacebook.root, "Title").GetComponent<UILabel> ();
		shareFacebook.valuesRoot = Master.GetChildByName (shareFacebook.root, "Values");
		shareFacebook.gemLabel = Master.GetChildByName (shareFacebook.root, "GemValueLabel").GetComponent<UILabel> ();
		shareFacebook.starLabel = Master.GetChildByName (shareFacebook.root, "StarValueLabel").GetComponent<UILabel> ();
		shareFacebook.actionButton = Master.GetChildByName (gameObject, "ShareFacebookButton").GetComponent<UIButton> ();
		shareFacebook.actionButtonLabel = Master.GetChildByName (gameObject, "ShareFacebookButtonLabel").GetComponent<UILabel> ();


		watchVideo_Offerwall.root = Master.GetChildByName (gameObject, "Offerwall");
		watchVideo_Offerwall.title = Master.GetChildByName (watchVideo_Offerwall.root, "Title").GetComponent<UILabel> ();
		watchVideo_Offerwall.actionButton = Master.GetChildByName (watchVideo_Offerwall.root, "Button_Go").GetComponent<UIButton> ();
		watchVideo_Offerwall.actionButtonLabel = Master.GetChildByName (watchVideo_Offerwall.actionButton.gameObject, "Text").GetComponent<UILabel> ();
	}

	public override void OnStart ()
	{
		SetRandomReward ();
		SetShareFacebookReward ();
		InvokeRepeating ("CheckTime", 0, 1);
	}

	void OnDestroy ()
	{
		CancelInvoke ();
	}

	public override void OnShowComplete ()
	{
//		if (Master.Tutorial.CheckAndContinueNextStepTutorial (TutorialController.TutorialsIndex.GetFreeReward)) {
//			Master.GetChildByName (Master.Tutorial.currentStepGO, "Arrows").transform.position = Master.GetChildByName (gameObject, "Button_GetRandomReward").transform.position + new Vector3 (0.2f, 0, 0);
//		}
	}

	void SetRandomReward ()
	{
		currentRandomReward = FreeRewardController.GetReward (0);
		watchVideo.gemLabel.text = currentRandomReward [0].ToString ();
		watchVideo.starLabel.text = currentRandomReward [1].ToString ();
	}

	void SetShareFacebookReward ()
	{
		//if (!FreeRewardController.IsSharedFacebook())
		//{
		currentShareFacebookReward = FreeRewardController.GetReward (1);
		shareFacebook.gemLabel.text = currentShareFacebookReward [0].ToString ();
		shareFacebook.starLabel.text = currentShareFacebookReward [1].ToString ();
		// }
	}

	void CheckTime ()
	{
		if (FreeRewardController.TimeRemainingWatchVideo_Offerwall () <= 0) {

			watchVideo_Offerwall.actionButton.gameObject.GetComponent<BoxCollider2D> ().enabled = true;
			watchVideo_Offerwall.actionButton.SetState (UIButtonColor.State.Normal, true);
			watchVideo_Offerwall.actionButtonLabel.text = "Go!";
		} else {
			watchVideo_Offerwall.actionButton.gameObject.GetComponent<BoxCollider2D> ().enabled = false;
			watchVideo_Offerwall.actionButton.SetState (UIButtonColor.State.Disabled, true);

			int secondRemaining = FreeRewardController.TimeRemainingWatchVideo_Offerwall ();
			TimeSpan t = TimeSpan.FromSeconds (secondRemaining);

			string timeString = string.Format ("{1:D2}:{2:D2}",
				                    t.Hours,
				                    t.Minutes,
				                    t.Seconds);
			watchVideo_Offerwall.actionButtonLabel.text = timeString;
		}

		//for time get watch video reward
		watchVideo.root.SetActive (true);
		if (FreeRewardController.TimeRemainingWatchVideo () <= 0) {

			watchVideo.actionButton.gameObject.GetComponent<BoxCollider2D> ().enabled = true;
			watchVideo.actionButton.SetState (UIButtonColor.State.Normal, true);
			watchVideo.actionButtonLabel.text = "Watch";
		} else {
			watchVideo.actionButton.gameObject.GetComponent<BoxCollider2D> ().enabled = false;
			watchVideo.actionButton.SetState (UIButtonColor.State.Disabled, true);

			int secondRemaining = FreeRewardController.TimeRemainingWatchVideo ();
			TimeSpan t = TimeSpan.FromSeconds (secondRemaining);

			string timeString = string.Format ("{1:D2}:{2:D2}",
				                    t.Hours,
				                    t.Minutes,
				                    t.Seconds);
			watchVideo.actionButtonLabel.text = timeString;
		}

		//for time get sharefacebook reward
		shareFacebook.root.SetActive (true);
		if (FreeRewardController.TimeRemainingShareFacebook () <= 0) {

			shareFacebook.actionButton.gameObject.GetComponent<BoxCollider2D> ().enabled = true;
			shareFacebook.actionButton.SetState (UIButtonColor.State.Normal, true);
			shareFacebook.actionButtonLabel.text = "Share";
		} else {
			shareFacebook.actionButton.gameObject.GetComponent<BoxCollider2D> ().enabled = false;
			shareFacebook.actionButton.SetState (UIButtonColor.State.Disabled, true);

			int secondRemaining = FreeRewardController.TimeRemainingShareFacebook ();
			TimeSpan t = TimeSpan.FromSeconds (secondRemaining);

			string timeString = string.Format ("{0:D2}:{1:D2}:{2:D2}",
				                    t.Hours,
				                    t.Minutes,
				                    t.Seconds);

			//int hour = secondRemaining / 3600;
			//int minute = secondRemaining / 60;
			//int second = secondRemaining % 60;
			shareFacebook.actionButtonLabel.text = timeString;
		}
	}

	public void GetWatchVideoRewardButton_OnClick ()
	{
		if (isGotRewaredDialogShowed ())
			return;

		Master.PlaySoundButtonClick ();

		if (Master.instance.CheckInternetConnection ()) {
			Master.Tutorial.CheckAndFinishTutorial (TutorialController.TutorialsIndex.GetFreeReward);
			Debug.LogError ("GetWatchVideoRewardButton_OnClick");
			AdmobControl.Instance.showAdmobVideo (this, 0);

		}
	}

	private bool isShowdialog = false;

	public void showVideoCompleted (bool success)
	{
		if (isShowdialog)
			return;
		Debug.LogError ("showVideoCompleted=" + success.ToString ());
		if (success) {
			Master.Stats.Gem += currentRandomReward [0];
			Master.Stats.Star += currentRandomReward [1];
			FreeRewardController.SetDateTimeGetRewardWatchVideo ();
			Master.QuestData.IncreaseProgressValue ("07");
			Master.PushNotification.SetWatchVideoRewardNotification ();
			isShowdialog = true;
			Invoke ("showReward", 0.2f);

		} else {
			GameObject alert = Master.UIMenu.ShowDialog ("Error");
			Master.GetChildByName (alert, "Content").GetComponent<UILabel> ().text = "Can not get the reward, please try again!";
		}
	}

	private void showReward ()
	{
		Master.UIMenu.ShowDialog ("GotRewardDialog", 0.3f, new string[] {
			currentRandomReward [0].ToString (),
			currentRandomReward [1].ToString ()
		}, null, () => {
			isShowdialog = false;
		}, ShowDialogType.Center);
		SetRandomReward ();
	}

	public void GetFacebookShareRewardButton_OnClick ()
	{
		if (isGotRewaredDialogShowed ())
			return;

		Master.PlaySoundButtonClick ();
		if (Master.instance.CheckInternetConnection ()) {

			FacebookController.Instance.ShareLink ("", FacebookController.Instance.titleShareLink, FacebookController.Instance.descriptionShareLink, FacebookController.Instance.linkImageShareLevelComplete, () => {
				Master.Stats.Gem += currentShareFacebookReward [0];
				Master.Stats.Star += currentShareFacebookReward [1];
				Master.QuestData.IncreaseProgressValue ("07");
				FreeRewardController.SetDatTimeGetShareFacebookReward ();
				Master.PushNotification.SetShareFacebookRewardNotification ();
				Master.UIMenu.ShowDialog ("GotRewardDialog", 0.3f, new string[] {
					currentShareFacebookReward [0].ToString (),
					currentShareFacebookReward [1].ToString ()
				}, null, null, ShowDialogType.Center);
				SetShareFacebookReward ();
				// Close();
			}, () => {
				GameObject alert = Master.UIMenu.ShowDialog ("Error");
				Master.GetChildByName (alert, "Content").GetComponent<UILabel> ().text = "Can not get the reward, please try again!";
			});
		}

	}

	public void OfferwallButton_Onclick ()
	{
		if (isGotRewaredDialogShowed ())
			return;

		Master.PlaySoundButtonClick ();

		if (Master.instance.CheckInternetConnection ()) {
			Debug.LogError ("OfferwallButton_Onclick");
			AdmobControl.Instance.showAdmobVideo (this, 1);
		} else {
			GameObject alert = Master.UIMenu.ShowDialog ("AlertDialog");
			Master.GetChildByName (alert, "Content").GetComponent<UILabel> ().text = "List to do is not ready, please try again after a few seconds";
		}
	}

	public void callback1 (bool success)
	{
		Debug.LogError ("callback1");
		if (isShowdialog)
			return;
		isShowdialog = true;
		if (success) {
			Master.Stats.Gem += currentRandomReward [0];
			Master.Stats.Star += currentRandomReward [1];
			FreeRewardController.SetDateTimeGetRewardWatchVideo_Offerwall ();
			Master.QuestData.IncreaseProgressValue ("07");
			Master.PushNotification.SetWatchVideoRewardNotification_Offerwall ();
			isShowdialog = true;
			Invoke ("showReward", 0.2f);
		
		} else {
			GameObject alert = Master.UIMenu.ShowDialog ("Error");
			Master.GetChildByName (alert, "Content").GetComponent<UILabel> ().text = "Can not get the reward, please try again!";
		}
	}

	bool isGotRewaredDialogShowed ()
	{
		if (FindObjectOfType<GotRewardDialog> () != null || GameObject.Find ("Error(Clone)") != null) {
			return true;
		} else {
			return false;
		}
	}

}
