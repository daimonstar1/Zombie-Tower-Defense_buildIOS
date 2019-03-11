using UnityEngine;
using System.Collections;
using DG.Tweening;
using Spine.Unity;

public class Enemy09 : EnemyController
{
  public float timeEffect = 3f;
  public float timePerDamage = 0.5f;

  public override void OnAwake ()
  {
    weapon = Resources.Load<GameObject> ("Prefabs/Characters/Enemies/Enemy_09_Poison");
  }

  public override void Attack ()
  {
    if (stopAllAction || !status.IsCanAttack)
      return;

    status.IsAttacking = true;
    if (isSpineAnim) {
      if (anim == null)
        anim = transform.GetComponent<SkeletonAnimation> ();
      anim.timeScale = 1.1f;
    } else
      animator.speed = 1;
    SetActionStatus ("Attack");
    SetTimeToAttack ();

    PlayAnimation ("Attack", () => {
      Idle ();
    });

    Master.WaitAndDo (0.5f, () => {
      if (!status.IsCanAttack) {
        status.IsAttacking = false;
        return;
      }
      GameObject poison = NGUITools.AddChild (Master.Gameplay.enemiesRoot, weapon);
      poison.transform.position = transform.position;
      poison.GetComponent<Enemy09_Poison> ().damage = data.Damage;
      poison.GetComponent<Enemy09_Poison> ().timeEffect = timeEffect;
      poison.GetComponent<Enemy09_Poison> ().timePerDamage = timePerDamage;
      poison.SetActive (true);
      Master.WaitAndDo (2, () => {
        status.IsAttacking = false;
      }, this);
    }, this);
  }


}
