using UnityEngine;
using System.Collections;

public class PauseGameDialog : DialogController
{
  UILabel statusSoundLabel;
  UILabel statusMusicLabel;

  public override void AssignObjects ()
  {
    statusSoundLabel = Master.GetChildByName (gameObject, "StatusSoundLabel").GetComponent<UILabel> ();
    statusMusicLabel = Master.GetChildByName (gameObject, "StatusMusicLabel").GetComponent<UILabel> ();
  }

  public override void OnStart ()
  {
    SetSettings ();
  }

  void SetSettings ()
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

  // Use this for initialization
  public void ResumeButton_OnClick ()
  {
    Master.PlaySoundButtonClick ();

    Close (() => {
      Master.Gameplay.ResumeGame ();
    });
  }

  public void MenuButton_OnClick ()
  {
    Master.PlaySoundButtonClick ();

    Close (() => {
      Master.Gameplay.GoToMenu ();
    });
  }

  public void ReplayButton_OnClick ()
  {
    Master.PlaySoundButtonClick ();

    Close (() => {
      Master.Gameplay.ReplayGame ();
    });
  }

  public UITexture soundTexture;
  public UITexture musicTexture;
  public Texture _on;
  public Texture _off;

  public void ChangeStatusAudioButton_OnClick (GameObject go)
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


}
