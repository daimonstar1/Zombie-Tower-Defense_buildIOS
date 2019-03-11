using UnityEngine;
using System.Collections;
using Spine.Unity;

public class Unit04 : UnitController
{
  // Use this for initialization
  public override void Shoot ()
  {
    if (action.Dead || action.Shoot || !status.IsCanShoot || status.IsFreezing)
      return;

    Master.Gameplay.UnitGetExp (data.UnitID, Master.UnitData.GetExpPerShootAtLevel (data.Level));

    status.CurrentShoot++;
    status.CurrentAmmo--;
    SetStatus ("Shoot");
    if (isSpineAnim) {
      if (anim == null)
        anim = transform.GetComponent<SkeletonAnimation> ();
      anim.timeScale = DEFAULT_SPEED_0;
    } else
      animator.speed = 0.3f;
    SetTimeToShoot ();
    PlayAnimation (Shoot_Name, () => {
      Idle ();
    });

    CreateBullet (4, 0.3f);
  }

  public override void OnStart ()
  {
    data.Damage = data.Damage - ((data.Damage * 30f) / 100f);
    data.Range = data.Range - ((data.Range * 30f) / 100f);
    data.AttackSpeed = data.AttackSpeed - ((data.AttackSpeed * 30f) / 100f);
  }

}
