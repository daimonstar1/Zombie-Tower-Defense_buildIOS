using UnityEngine;
using System.Collections;

public class FillEnergyDialog : DialogController
{

    // private int gemPerEnergy = 10;
    private int gemRequireForFull = 0;
    private int energyNeedToFillFull;
    UILabel gemPerEnergyLabel;
    UILabel gemFullEnergyLabel;
    string[] agruments;

    public override void AssignObjects()
    {
        gemPerEnergyLabel = Master.GetChildByName(gameObject, "GemValuePerEnergy").GetComponent<UILabel>();
        gemFullEnergyLabel = Master.GetChildByName(gameObject, "GemValueFullEnergy").GetComponent<UILabel>();

    }

    public override void OnOpen(string[] agruments = null, System.Action onCloseComplete = null)
    {
        this.agruments = agruments;
        energyNeedToFillFull = Master.Stats.MaxEnergy - Master.Stats.Energy;
        gemRequireForFull = energyNeedToFillFull * Master.Stats.GemPerEnergy;
        if (energyNeedToFillFull >= 1)
        {
            gemRequireForFull = (int)(gemRequireForFull * 0.7f);
        }
        gemFullEnergyLabel.text = gemRequireForFull.ToString();
        gemPerEnergyLabel.text = Master.Stats.GemPerEnergy.ToString();
    }

    public void BuyFullEnergyButton_OnClick()
    {
        BuyEnergy(gemRequireForFull, energyNeedToFillFull);
    }

    public void BuyOneEnergy_OnClick()
    {
        BuyEnergy(Master.Stats.GemPerEnergy, 1);
    }

    public void InviteFriendButton_OnClick()
    {
        FacebookController.Instance.AppInvite(() =>
        {
            Master.Audio.PlaySound("snd_buy");
            if (Master.Stats.Energy < Master.Stats.MaxEnergy)
            {
                Master.Stats.Energy++;
            }
            Close();
        });
    }

    void BuyEnergy(int gemRequire, int energy)
    {
        if (gemRequire <= Master.Stats.Gem)
        {
            Master.Audio.PlaySound("snd_buy");
            Master.Stats.Energy += energy;
            Master.Stats.Gem -= gemRequire;
            Close(() =>
            {
                if (agruments != null)
                {
                    if (agruments[0] == "ReplayScene")
                    {
                        Master.UI.Transition(() =>
                        {
                            Time.timeScale = 1;
                            Application.LoadLevel(Application.loadedLevel);
                        });
                    }
                    else if (agruments[0] == "GoToLevel")
                    {
                        Master.UI.Transition(() =>
                        {
                            Time.timeScale = 1;
                            Master.LevelData.currentLevel = int.Parse(agruments[1]);
                            Application.LoadLevel("Play");
                        });
                    }
                }
            });
        }
        else
        {
            Master.PlaySoundButtonClick();
            Close(() =>
            {
                if (Master.UIMenu != null)
                {
                    Master.UIMenu.OpenPanel(4);
                }
                else
                {
                    Master.defaultPanelMenu = 4;
                    Master.Gameplay.GoToMenu();
                }
            });

        }
    }

    public void CloseButton_OnClick()
    {
        Close(() =>
        {
            if (Master.Gameplay != null)
            {
                Master.Gameplay.GoToMenu();
            }
        });
    }
}
