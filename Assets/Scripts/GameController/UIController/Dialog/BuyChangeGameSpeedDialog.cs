using UnityEngine;
using System.Collections;
using System;
using CodeStage.AntiCheat.ObscuredTypes;

public class BuyChangeGameSpeedDialog : DialogController
{

    public override void OnStart()
    {
        base.OnStart();
        onCloseComplete = () =>
        {
            Master.Gameplay.ResumeGame();
        };
    }

    public override void OnOpen(string[] agruments = null, Action onCloseComplete = null)
    {
        this.onCloseComplete = onCloseComplete;
    }

    public void BuyButton_OnClick()
    {
        IAPController.Instance.PurchaseProduct(ProductDataController.changeGameSpeedID, () =>
        {
            Master.Audio.PlaySound("snd_buy");
            Master.Stats.SetDataInt(Master.Stats.changeGameSpeedName, 1);
            Master.Stats.isBoughtChangeGameSpeed = true;
            Master.UIGameplay.ChangeGameSpeed();
            Close();
        });
    }



}
