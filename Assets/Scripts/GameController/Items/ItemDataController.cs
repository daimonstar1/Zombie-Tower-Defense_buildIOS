using UnityEngine;
using System.Collections;
using CodeStage.AntiCheat.ObscuredTypes;

public class ItemDataController : MonoBehaviour
{

    // Use this for initialization
    [System.Serializable]
    public class ItemData
    {
        public string ID = "";
        public string Name = "";
        public float TimeCountdown;
        public float Price;
        [HideInInspector]
        public int Quantity
        {
            get
            {
                return ObscuredPrefs.GetInt("Item_Quantiy_" + ID, 0);
            }
            set
            {

            }
        }

    }

    [System.Serializable]
    public class RecoverHealth : ItemData
    {
        public float HealthRecover = 100; //pecent. full health

    }
    public RecoverHealth recoverHealth = new RecoverHealth();

    [System.Serializable]
    public class Freeze : ItemData
    {
        public float TimeEffect = 5; //pecent. full health
    }
    public Freeze freeze = new Freeze();

    [System.Serializable]
    public class FireBall : ItemData// kill all enemy
    {
        public float Damage = 10000; //pecent. full health
    }
    public FireBall fireBall = new FireBall();

    void Awake()
    {
        GetQuantity();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GetQuantity()
    {
        recoverHealth.Quantity = ObscuredPrefs.GetInt("Item_Quantiy_" + recoverHealth.ID, 0);
        freeze.Quantity = ObscuredPrefs.GetInt("Item_Quantiy_" + freeze.ID, 0);
        fireBall.Quantity = ObscuredPrefs.GetInt("Item_Quantiy_" + fireBall.ID, 0);
    }

    public void SetQuantity()
    {

    }

}
