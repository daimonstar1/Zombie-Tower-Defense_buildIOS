using UnityEngine;
using System.Collections;
using Spine.Unity;

public class Unit03 : UnitController
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
			anim.timeScale = DEFAULT_SPEED_1;
		} else
			animator.speed = 0.4f;
		SetTimeToShoot ();
		PlayAnimation (Shoot_Name, () => {
			Idle ();
		});

		if (!status.IsUpgraded) {
			CreateBullet (-1, 0.3f);
		} else {
			CreateBullet (0.25f, 0.3f);
		}


	}



}
