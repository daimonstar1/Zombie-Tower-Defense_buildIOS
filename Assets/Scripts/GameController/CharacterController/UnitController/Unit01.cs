using UnityEngine;
using System.Collections;
using Spine.Unity;

public class Unit01 : UnitController
{

  // Use this for initialization

  public override void OnStart ()
  {
    data.Damage = data.Damage / 2;
  }

  public override void OnUpdate ()
  {
    base.OnUpdate ();
    if (!status.IsActive) {
      if (isSpineAnim) {
        if (anim == null)
          anim = transform.GetComponent<SkeletonAnimation> ();
        anim.GetComponent<MeshRenderer> ().sortingOrder = 1000;
      }
    }
  }

  public override void Shoot ()
  {
    if (action.Dead || action.Shoot || !status.IsCanShoot || status.IsFreezing)
      return;

    Master.Gameplay.UnitGetExp (data.UnitID, Master.UnitData.GetExpPerShootAtLevel (data.Level));
    status.CurrentShoot++;
    status.CurrentAmmo--;
    SetTimeToShoot ();
    SetStatus ("Shoot");
    if (!status.IsUpgraded) {
      if (isSpineAnim) {
        if (anim == null)
          anim = transform.GetComponent<SkeletonAnimation> ();
        anim.timeScale = DEFAULT_SPEED;
      } else
        animator.speed = 0.28f;
    } else {
      if (isSpineAnim) {
        if (anim == null)
          anim = transform.GetComponent<SkeletonAnimation> ();
        anim.timeScale = DEFAULT_SPEED_4;
      } else
        animator.speed = 0.22f;
    }

    PlayAnimation (Shoot_Name, () => {
      Idle ();
    });

    Master.WaitAndDo (0.15f, () => {
      CreateBullet (3);
      Master.WaitAndDo (0.15f, () => {
        CreateBullet (3);
      }, this);
    }, this);
  }


}
