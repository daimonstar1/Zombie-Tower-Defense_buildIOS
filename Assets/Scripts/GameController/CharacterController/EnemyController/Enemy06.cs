using UnityEngine;
using System.Collections;

public class Enemy06 : EnemyController
{

	private Transform HealthBar;

	public override void OnUpdate ()
	{
		base.OnUpdate ();
		if (HealthBar == null)
			HealthBar = general.transform.Find ("HealthBar");
		if (HealthBar != null) {
			HealthBar.localPosition = new Vector3 (HealthBar.localPosition.x, 5.4f, 0f);
		}
	}
}
