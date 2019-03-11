using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AntiIAPHacking : MonoBehaviour
{
	private static AntiIAPHacking _instance;

	public static AntiIAPHacking Instance {
		get {
			if (_instance == null) {
				_instance = Resources.FindObjectsOfTypeAll<AntiIAPHacking> () [0];
			}
			return _instance;
		}
	}

	public string urlToGetListPackagesBanned = "https://docs.google.com/document/d/1msh4s8cg1W-tTYazDvd9m77ENLBDJNgTL9fM8uPRAFw/";

	public List<string> listPackagesBanned = new List<string> ();

	System.Action onNonDetected;
	System.Action onDetected;

	void Awake ()
	{
		DontDestroyOnLoad (gameObject);
	}

	public void Start ()
	{
		GetListPackagesBanned ();
		InvokeRepeating ("DetectIAPHackingApp", 5, 10);
	}

	public void GetListPackagesBanned (System.Action onComplete = null, System.Action onFail = null)
	{
		StartCoroutine (getListPackagesBanned (onComplete, onFail));
	}

	private IEnumerator getListPackagesBanned (System.Action onComplete = null, System.Action onFail = null)
	{
		WWW w = new WWW (urlToGetListPackagesBanned);
		yield return w;
		if (w.error != null) {
			ReadListPackagesBanned ();
			Debug.Log ("Error .. " + w.error);
			if (onFail != null)
				onFail ();
		} else {
			string longStringFromFile = w.text;
			longStringFromFile = longStringFromFile.Split (new[] { "ListPackagesBanned" }, System.StringSplitOptions.None) [1];
			foreach (string x in longStringFromFile.Split(new string[] { "\r", "\n" }, System.StringSplitOptions.None)) {
				if (x.Trim ().Length > 0 && !listPackagesBanned.Contains (x.Trim ())) {
					listPackagesBanned.Add (x.Trim ());
				}
			}

			SaveListPackagesBanned ();

			if (onComplete != null)
				onComplete ();



		}
	}

	void ReadListPackagesBanned ()
	{
		string res = PlayerPrefs.GetString ("ListPackagesBanned", "");
		if (res != "") {
			foreach (string x in res.Split(' ')) {
				if (x.Trim ().Length > 0 && !listPackagesBanned.Contains (x.Trim ())) {
					listPackagesBanned.Add (x.Trim ());
				}
			}
		}
	}

	void SaveListPackagesBanned ()
	{

		string res = "";
		foreach (string package in listPackagesBanned) {
			res += package + " ";
		}
		PlayerPrefs.SetString ("ListPackagesBanned", res);
		PlayerPrefs.Save ();
	}



	public bool DetectIAPHackingApp ()
	{
		DebugStreamer.AddMessage ("Start Detecting IAP Hacking");
		foreach (string x in listPackagesBanned) {
			Debug.Log ("PackageBanned: " + x);
		}
		foreach (string package in listPackagesBanned) {
			if (IsPackageInstalled (package.Trim ())) {
				Master.WaitAndDo (3, () => {
					Application.Quit ();
				}, this, true);
				Master.UI.ShowDialog ("DetectedIAPHackingApp");
				return true;
			}
		}
		return false;

	}

	public bool IsPackageInstalled (string bundleID)
	{

#if UNITY_EDITOR
		return false;
#endif


#if UNITY_ANDROID
		AndroidJavaClass up = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		AndroidJavaObject ca = up.GetStatic<AndroidJavaObject> ("currentActivity");
		AndroidJavaObject packageManager = ca.Call<AndroidJavaObject> ("getPackageManager");
		AndroidJavaObject launchIntent = null;
		//if the app is installed, no errors. Else, doesn't get past next line
		try {
			launchIntent = packageManager.Call<AndroidJavaObject> ("getLaunchIntentForPackage", bundleID);
			//        
			//        ca.Call("startActivity",launchIntent);
		} catch {
			Debug.Log ("exception");
		}

		Debug.Log ("launchIntent for -" + bundleID + ": " + launchIntent);
		if (launchIntent == null)
			return false;
		return true;
#endif
		return false;

	}

	IEnumerator checkInternetConnection (System.Action ifConnected, System.Action ifNotConnected)
	{
		WWW www = new WWW ("http://google.com");
		yield return www;
		if (www.error != null) {
			ifNotConnected ();

		} else {
			ifConnected ();
		}
	}


	public void CheckInternetConnection (System.Action ifConnected, System.Action ifNotConnected)
	{
		StartCoroutine (checkInternetConnection (ifConnected, ifNotConnected));
	}

}