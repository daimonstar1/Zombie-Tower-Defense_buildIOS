using UnityEngine;
using System.Collections;

public class BuyBoostEXPDialog : DialogController
{
    private UILabel buyX2EXPValue;
    private UILabel buyX3EXPValue;
    private UILabel buyX4EXPValue;

    public override void AssignObjects()
    {
        buyX2EXPValue = Master.GetChildByName(gameObject, "BuyX2EXPValue").GetComponent<UILabel>();
        buyX3EXPValue = Master.GetChildByName(gameObject, "BuyX3EXPValue").GetComponent<UILabel>();
        buyX4EXPValue = Master.GetChildByName(gameObject, "BuyX4EXPValue").GetComponent<UILabel>();

    }

    public override void OnOpen(string[] agruments = null, System.Action onCloseComplete = null)
    {
        buyX2EXPValue.text = "$" + ProductDataController.Instance.buyX2Price.ToString();
        buyX3EXPValue.text = "$" + ProductDataController.Instance.buyX3Price.ToString();
        buyX4EXPValue.text = "$" + ProductDataController.Instance.buyX4Price.ToString();

    }

    public void BuyX2Boost_OnClick()
    {
        Buy(1, 1);
    }

    public void BuyX3Boost_OnClick()
    {
        Buy(2, 1);
    }

    public void BuyX4Boost_OnClick()
    {
        Buy(3, 1);
    }

    void Buy(int boostType, int day = 1)
    {
        string productID = "";
        if (boostType == 1)
        {
            productID = ProductDataController.x2EXP;
        }
        else if (boostType == 2)
        {
            productID = ProductDataController.x3EXP;
        }
        else if (boostType == 3)
        {
            productID = ProductDataController.x4EXP;
        }

        IAPController.Instance.PurchaseProduct(productID, () =>
        {
            // Debug.Log("asdasdasd");
            Master.Audio.PlaySound("snd_buy");
            BoostEXPController.Instance.SetBoosting(boostType, day);
            Close();
        });

    }

}
