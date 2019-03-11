using UnityEngine;
using System.Collections;
using DG.Tweening;
using Spine.Unity;

public class Enemy05 : EnemyController
{
  public override void OnStart ()
  {
    timeToAttack = 0;
  }

  private GameObject ex;

  public override void Attack ()
  {
    if (stopAllAction && !status.IsCanAttack)
      return;
    if (isSpineAnim) {
      if (anim == null)
        anim = transform.GetComponent<SkeletonAnimation> ();
      anim.timeScale = 0.9f;
    } else
      animator.speed = 0.7f;
    SetActionStatus ("Attack");
    SetTimeToAttack ();
    Master.Effect.ShakeCamera (1, 0.2f);
    Master.Audio.PlaySound ("snd_explosion");
    weapon.SetActive (true);
    Debug.Log (weapon);
    ex = Resources.Load<GameObject> ("Prefabs/explosion1");
    GameObject clone = Instantiate (ex, transform.position, Quaternion.identity);
    clone.name = "explosion1";
//    GameObject explosion = NGUITools.AddChild (gameObject, Master.GetGameObjectInPrefabs ("explosion1"));
//		Destroy (explosion, 1f);
    Dead ();
    //PlayAnimation("Attack", () =>
    //{

    //});

  }

  public override void Dead ()
  {
    if (stopAllAction)
      return;

    stopAllAction = true;
    general.SetActive (false);
    SetActionStatus ("Dead");
    Master.Lane.RemoveCharacterAtLane (status.CurrentLane, gameObject);
    Destroy (gameObject.GetComponent<BoxCollider2D> ());
    DropGoldController ();
    Master.Audio.PlaySound ("snd_zombieDead" + Random.Range (1, 4));
    Master.QuestData.IncreaseProgressValue ("01");
    Master.Gameplay.CheckLevelComplete ();

    PlayAnimation ("Dead", () => {
      weapon.SetActive (false);
//      DOTween.ToAlpha (() => spriteRenderer.color, x => spriteRenderer.color = x, 0, 1).OnComplete (() => {
//      });
      Destroy (gameObject);

    });
  }

}
