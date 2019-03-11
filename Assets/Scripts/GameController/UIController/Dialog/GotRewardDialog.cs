using UnityEngine;
using System.Collections;

public class GotRewardDialog : DialogController
{
    GameObject gemRoot;
    GameObject starRoot;
    UIGrid rewardRoot;
    private UILabel gemValueLabel;
    private UILabel starValueLabel;

    public override void AssignObjects()
    {
        isAutoPlaySound = false;
        rewardRoot = Master.GetChildByName(gameObject, "Reward").GetComponent<UIGrid>();
        gemRoot = Master.GetChildByName(gameObject, "Gem");
        starRoot = Master.GetChildByName(gameObject, "Star");

        gemValueLabel = Master.GetChildByName(gameObject, "GemValueLabel").GetComponent<UILabel>();
        starValueLabel = Master.GetChildByName(gameObject, "StarValueLabel").GetComponent<UILabel>();
    }

    public override void OnOpen(string[] agruments = null, System.Action onCloseComplete = null)
    {
        Master.Audio.PlaySound("snd_getReward");
        Master.Effect.CreateEffect("Effect_Star", gameObject.transform.localPosition);
        this.onCloseComplete = onCloseComplete;
        gemValueLabel.text = agruments[0];
        starValueLabel.text = agruments[1];

        if (int.Parse(agruments[0]) <= 0)
        {
            //Destroy(gemRoot);
            NGUITools.Destroy(gemRoot);

            //gemRoot.SetActive(false);
        }

        if (int.Parse(agruments[1]) <= 0)
        {
            NGUITools.Destroy(starRoot);
            // starRoot.SetActive(false);
        }

        rewardRoot.Reposition();
    }

    public void GotItButton_OnClick()
    {
        Master.UI.CloseDialog(gameObject, 0.4f, this.onCloseComplete, CloseDialogType.Center);
    }

}
