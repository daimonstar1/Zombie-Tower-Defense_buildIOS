using UnityEngine;
using System.Collections;
using Spine.Unity;

public class Unit07 : UnitController
{
	// Use this for initialization
	public override void Shoot ()
	{
		if (action.Dead || action.Shoot || !status.IsCanShoot || status.IsFreezing)
			return;

		Master.Gameplay.UnitGetExp (data.UnitID, Master.UnitData.GetExpPerShootAtLevel (data.Level));

		status.CurrentShoot++;
		status.CurrentAmmo--;
		if (isSpineAnim) {
			if (anim == null)
				anim = transform.GetComponent<SkeletonAnimation> ();
			anim.timeScale = DEFAULT_SPEED_1;
		} else
			animator.speed = 0.4f;
		SetStatus ("Shoot");
		SetTimeToShoot ();
		PlayAnimation (Shoot_Name, () => {
			Idle ();
		});

		CreateBullet (-1, 0.2f);



	}
}
