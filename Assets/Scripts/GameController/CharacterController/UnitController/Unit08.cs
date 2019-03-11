using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;

public class Unit08 : UnitController
{
	// Use this for initialization
	public override void Shoot ()
	{
		if (action.Dead || !status.IsCanShoot || status.IsFreezing)
			return;

		Master.Gameplay.UnitGetExp (data.UnitID, Master.UnitData.GetExpPerShootAtLevel (data.Level));

		status.CurrentShoot++;
		status.CurrentAmmo--;
		SetTimeToShoot ();
		if (isSpineAnim) {
			if (anim == null)
				anim = transform.GetComponent<SkeletonAnimation> ();
			anim.timeScale = DEFAULT_SPEED_1;
		} else
			animator.speed = 0.4f;
		SetStatus ("Shoot");
		PlayAnimation (Shoot_Name, () => {
			Idle ();
		});

		if (!status.IsUpgraded) {
			CreateBullet (0.3f, 0.3f);
		} else {
			Master.WaitAndDo (0.3f, () => {
				CreateBullet (0.3f);
				UpgradeShoot ();
			}, this);
		}
	}

	void UpgradeShoot ()
	{

		List<GameObject> listEnemyCanGetShootAtOtherLane = new List<GameObject> ();
		foreach (GameObject obj in Master.Lane.GetCharactersAtAllLaneByTag("Enemy")) {
			if (transform.position.x > obj.transform.position.x) {
				continue;
			}
			float distance = Vector3.Distance (gameObject.transform.position, obj.transform.position);
			//    Debug.Log(obj.name);
			if ((data.Range + (data.Range * 0.4f) > distance * 100) && obj.GetComponent<EnemyController> ().status.CurrentLane != status.CurrentLane) {
				if (listEnemyCanGetShootAtOtherLane.Count < 3) {
					listEnemyCanGetShootAtOtherLane.Add (obj);
				} else {
					break;
				}
				// isCanShoot = true;
			}
		}



		string bulletName = "Bullet_" + data.UnitID + "_Upgrade";


		GameObject pf_bullet = Resources.Load<GameObject> ("Prefabs/Bullets/" + bulletName);

		if (pf_bullet == null) {
			pf_bullet = Resources.Load<GameObject> ("Prefabs/Bullets/Bullet_" + data.UnitID);
		}

		if (listEnemyCanGetShootAtOtherLane.Count > 0) {
			foreach (GameObject enemy in listEnemyCanGetShootAtOtherLane) {
				GameObject obj_bullet = NGUITools.AddChild (Master.Gameplay.gameplayRoot, pf_bullet);
				obj_bullet.GetComponent<BulletController> ().damage = data.Damage;
				obj_bullet.GetComponent<BulletController> ().isUpgraded = status.IsUpgraded;
				obj_bullet.GetComponent<BulletController> ().unit_id = data.UnitID;
				obj_bullet.transform.position = upgradeSpawnBullet.position;
				obj_bullet.transform.localRotation = Quaternion.Euler (0, 0, AngleInDeg (obj_bullet.transform.position, enemy.transform.position));

				float distance = Vector3.Distance (obj_bullet.transform.position, enemy.transform.position);

				obj_bullet.transform.localScale = new Vector3 (distance * 0.7f, 1, 1);
				Destroy (obj_bullet, 0.3f);

			}
		}

	}

	public static float AngleInDeg (Vector3 vec1, Vector3 vec2)
	{
		return AngleInRad (vec1, vec2) * 180 / Mathf.PI;
	}

	public static float AngleInRad (Vector3 vec1, Vector3 vec2)
	{
		return Mathf.Atan2 (vec2.y - vec1.y, vec2.x - vec1.x);
	}

}
