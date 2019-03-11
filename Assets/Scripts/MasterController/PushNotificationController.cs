using UnityEngine;
using System.Collections;

public class PushNotificationController : MonoBehaviour
{

	// Use this for initialization
	public class NotificationID
	{
		public static int FullEnergy = 0;
		public static int GetFreeRewardWatchVideo = 1;
		public static int GetFreeRewardWatchVideo_OF = 4;
		public static int GetFreeRewardShareFacebook = 2;
		public static int LongTimeNoLogin = 3;
	}

	string title = "Zombie Rising";

	void Awake ()
	{
		if (Master.PushNotification == null) {
			Master.PushNotification = this;
		}
	}


	void Start ()
	{
		SetAllNotification ();
	}

	void OnApplicationPause (bool pauseStatus)
	{
		if (pauseStatus) {
			SetAllNotification ();
		}
	}

	public void SetAllNotification ()
	{
		SetEnergyNotification ();
		SetWatchVideoRewardNotification ();
		SetShareFacebookRewardNotification ();
		SetLongTimeNoLogin ();
	}

	public void CancelAllNotification ()
	{
//        AndroidNotificationManager.Instance.CancelAllLocalNotifications();
	}

	public void SetEnergyNotification ()
	{
		string content = "Your Energies are fully charged. Let kill all the zombies!";
		int secondDelay = 0;
		int remainEnergy = Master.Stats.MaxEnergy - Master.Stats.Energy;
		if (remainEnergy > 0) {
			secondDelay = (Master.Stats.minuteFillPerEnergy * (remainEnergy - 1)) * 60;
			secondDelay += Master.Stats.timeRemainingCountdownEnergy;
		} else {
			secondDelay = (Master.Stats.minuteFillPerEnergy * Master.Stats.MaxEnergy) * 60;
		}
		SetNotification (NotificationID.FullEnergy, secondDelay, title, content, "snd_fullenergynotification");
	}

	public void SetWatchVideoRewardNotification ()
	{
		if ((Master.LevelData.lastLevel + 1) < FreeRewardController.levelCanGetFreeReward)
			return;

		string content = "Hurry up! Your free rewards are ready. Let get its now!";

		int secondDelay = 0;
		if (FreeRewardController.TimeRemainingWatchVideo () > 0) {
			secondDelay = FreeRewardController.TimeRemainingWatchVideo ();
		} else {
			secondDelay = 5 * 60 * 60;
		}

		SetNotification (NotificationID.GetFreeRewardWatchVideo, secondDelay, title, content, "snd_getrewardnotification");
	}

	public void SetWatchVideoRewardNotification_Offerwall ()
	{
		if ((Master.LevelData.lastLevel + 1) < FreeRewardController.levelCanGetFreeReward)
			return;

		string content = "Hurry up! Your free rewards are ready. Let get its now!";

		int secondDelay = 0;
		if (FreeRewardController.TimeRemainingWatchVideo_Offerwall () > 0) {
			secondDelay = FreeRewardController.TimeRemainingWatchVideo_Offerwall ();
		} else {
			secondDelay = 5 * 60 * 60;
		}

		SetNotification (NotificationID.GetFreeRewardWatchVideo_OF, secondDelay, title, content, "snd_getrewardnotification");
	}

	public void SetShareFacebookRewardNotification ()
	{
		if ((Master.LevelData.lastLevel + 1) < FreeRewardController.levelCanGetFreeReward)
			return;

		string content = "Hurry up! Let's share Facebook to get big rewards!";

		int secondDelay = 0;
		if (FreeRewardController.TimeRemainingShareFacebook () > 0) {
			secondDelay = FreeRewardController.TimeRemainingShareFacebook ();
		} else {
			secondDelay = 10 * 60 * 60;
		}

		SetNotification (NotificationID.GetFreeRewardShareFacebook, secondDelay, title, content, "snd_getrewardnotification");
	}

	public void SetLongTimeNoLogin ()
	{
		int secondDelay = 48 * 60 * 60;
		string content = "The Zombies are destroying the City, let defeat them now!";

		SetNotification (NotificationID.LongTimeNoLogin, secondDelay, title, content, "snd_fullenergynotification");
	}

	void SetNotification (int id, int secondDelay, string title, string content, string soundName, string icon = "notificationsmallicon", string largeIcon = "notificationicon")
	{

#if UNITY_EDITOR
		return;
#endif

#if UNITY_ANDROID
//        AndroidNotificationManager.Instance.CancelLocalNotification(id);
//        AndroidNotificationBuilder builder = new AndroidNotificationBuilder(id, title, content, secondDelay);
//        AndroidNotificationBuilder.NotificationColor color = new AndroidNotificationBuilder.NotificationColor(new Color((float)2 / 255, (float)199 / 255, 0, 1));
//        builder.SetColor(color);
//        builder.SetSoundName(soundName);
//        builder.SetIconName(icon);
//        builder.SetLargeIcon(largeIcon);
//        builder.SetVibration(true);
//        builder.ShowIfAppIsForeground(false);
//        AndroidNotificationManager.Instance.ScheduleLocalNotification(builder);


#elif UNITY_IOS
        var notif = new UnityEngine.iOS.LocalNotification();
        notif.fireDate = System.DateTime.Now.AddSeconds(secondDelay);
        notif.alertAction = title;
        notif.alertBody = content;
        UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(notif);
#endif
	}

}
