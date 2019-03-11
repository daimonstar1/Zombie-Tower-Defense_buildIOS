using UnityEngine;
using System.Collections;

public class Enemy09_Poison : MonoBehaviour
{

    // Use this for initialization
    GameObject weaponCollider;
    public float damage;
    public float timeEffect;
    public float timePerDamage;
    void Awake()
    {
        gameObject.SetActive(true);
        weaponCollider = Master.GetChildByName(gameObject, "PoisonEnemy");
        weaponCollider.SetActive(false);
        Destroy(gameObject, 2);
    }

    void Start()
    {
        gameObject.SetActive(true);
        Master.WaitAndDo(0.8f, () =>
        {
            weaponCollider.SetActive(true);
            Master.Audio.PlaySound("snd_enemy09_attack", 1f);
        }, this);
    }
}
