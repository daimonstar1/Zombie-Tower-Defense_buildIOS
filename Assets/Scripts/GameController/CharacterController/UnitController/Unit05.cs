using UnityEngine;
using System.Collections;
using Spine.Unity;

public class Unit05 : UnitController
{
	// Use this for initialization
	public override void Shoot ()
	{
		if (action.Dead || action.Shoot || !status.IsCanShoot || status.IsFreezing)
			return;

		Master.Gameplay.UnitGetExp (data.UnitID, Master.UnitData.GetExpPerShootAtLevel (data.Level));

		CreateBullet ();
		status.CurrentShoot++;
		status.CurrentAmmo--;
		SetStatus ("Shoot");
		SetTimeToShoot ();
		if (isSpineAnim) {
			if (anim == null)
				anim = transform.GetComponent<SkeletonAnimation> ();
			anim.timeScale = DEFAULT_SPEED_2;
		} else
			animator.speed = 0.5f;

		PlayAnimation (Shoot_Name, () => {
			Idle ();
		});

	}

}
