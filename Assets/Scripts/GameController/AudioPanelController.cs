using UnityEngine;
using System.Collections;

public class AudioPanelController :DialogController
{

	public UITexture soundTexture;
	public UITexture musicTexture;
	public Texture _on;
	public Texture _off;

	void Start ()
	{
	
	}

	void OnEnable ()
	{
		SetSettings ();
	}
	// Update is called once per frame
	void Update ()
	{
	
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

	public void SetSettings ()
	{
		if (Master.Audio.isSoundOn) {
			
			Master.GetChildByName (soundTexture.gameObject, "X").SetActive (false);
			Master.GetChildByName (soundTexture.gameObject, "on").SetActive (true);
			soundTexture.mainTexture = _on;
		} else {
			Master.GetChildByName (soundTexture.gameObject, "X").SetActive (true);
			Master.GetChildByName (soundTexture.gameObject, "on").SetActive (false);
			soundTexture.mainTexture = _off;
		}

		if (Master.Audio.isBackgroundMusicOn) {
			Master.GetChildByName (musicTexture.gameObject, "X").SetActive (false);
			Master.GetChildByName (musicTexture.gameObject, "on").SetActive (true);
			musicTexture.mainTexture = _on;
		} else {
			Master.GetChildByName (musicTexture.gameObject, "X").SetActive (true);
			Master.GetChildByName (musicTexture.gameObject, "on").SetActive (false);
			musicTexture.mainTexture = _off;
		}

	}
}
