using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour
{


	[HideInInspector]
	public float damage;
	[HideInInspector]
	public bool isCrit;
	[HideInInspector]
	public Animator animator;
	public bool isUpgraded;
	public bool isChild;

	[Header ("For Animation")]
	public bool autoPlayAnimation;
	public string animationName;
	public bool playAnimationWhenHitZombie;
	public string animationNameWhenHitZombie;

	[Header ("For Movement")]
	public Movement movement = Movement.Move;
	public float moveSpeed;

	[Header ("For Sound")]
	public bool isPlaySoundWhenHitEnemy;
	public string soundName;

	private bool isHitEnemy;
	public string unit_id;

	public enum Movement
	{
		Fixed,
		Move
	}

	void Awake ()
	{
		animator = gameObject.GetComponent<Animator> ();
		Destroy (gameObject, 5f);
	}

	void Start ()
	{
		CheckAutoAnimation ();

		if (isChild) {
			damage = transform.parent.gameObject.GetComponent<BulletController> ().damage;
		}
		OnStart ();

		//if (isCrit)
		//{
		//    damage = damage * 2;
		//}
	}

	void Update ()
	{
		CheckOutOfScreen ();
		MovementController ();
		OnUpdate ();
	}

	public virtual void OnStart ()
	{

	}

	public virtual void OnUpdate ()
	{
	}

	public void MovementController ()
	{
		if (movement == Movement.Move && !isHitEnemy) {
			transform.Translate (new Vector3 (moveSpeed * Time.deltaTime, 0, 0));
		}
	}

	private GameObject tail;

	public void CheckAutoAnimation ()
	{
		if (!unit_id.Equals ("06") && !unit_id.Equals ("07") && !unit_id.Equals ("08")) {
			GameObject bullet_tail = Master.GetGameObjectInPrefabs ("bullet_tail");
			tail = Instantiate (bullet_tail, transform.position, bullet_tail.transform.rotation);
		} else if (unit_id.Equals ("07")) {
			GameObject bullet_tail = Master.GetGameObjectInPrefabs ("rocket_tail");
			tail = Instantiate (bullet_tail, transform.position, bullet_tail.transform.rotation);
		}
		if (autoPlayAnimation) {
			animator.Play (animationName);
		}
	}

	public virtual void CollisionController (GameObject obj)
	{
		if (obj.tag == "Enemy") {
			if (tail != null)
				Destroy (tail);
			if (!unit_id.Equals ("06") && !unit_id.Equals ("07") && !unit_id.Equals ("08")) {
				GameObject bullet_ex = Master.GetGameObjectInPrefabs ("bullet_ex");
				Instantiate (bullet_ex, transform.position, bullet_ex.transform.rotation);
			} else if (unit_id.Equals ("07")) {
				GameObject bullet_ex = Master.GetGameObjectInPrefabs ("impact_ex");
				Instantiate (bullet_ex, transform.position, bullet_ex.transform.rotation);
			}
			isHitEnemy = true;

//			isCrit = true;
			obj.GetComponent<EnemyController> ().GetHit (damage, isCrit);
			if (isUpgraded) {
				if (unit_id.Equals ("03") || unit_id.Equals ("06") || unit_id.Equals ("08")) {
					if (playAnimationWhenHitZombie) {
						if (isPlaySoundWhenHitEnemy) {
							Master.Audio.PlaySound (soundName);
						}

						PlayAnimation (animationNameWhenHitZombie, () => {
							Destroy (gameObject);
						});
					}
				} else
					Destroy (gameObject, 0.1f);

			} else {
				if (unit_id.Equals ("06") || unit_id.Equals ("08")) {
					if (playAnimationWhenHitZombie) {
						if (isPlaySoundWhenHitEnemy) {
							Master.Audio.PlaySound (soundName);
						}

						PlayAnimation (animationNameWhenHitZombie, () => {
							Destroy (gameObject);
						});
					}
				} else
					Destroy (gameObject, 0.1f);
			}

		}

	}

	void OnCollisionEnter2D (Collision2D col)
	{
		CollisionController (col.gameObject);
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		CollisionController (col.gameObject);
	}

	public void CheckOutOfScreen ()
	{
		float x = gameObject.transform.localPosition.x;
		float maxX = 1280;
		if (x > maxX) {
			Destroy (gameObject);
		}
	}

	public void PlayAnimation (string animationClipName, System.Action onAnimationComplete = null)
	{
		animator.Play (animationClipName, 0, 0);
		Master.WaitAndDo (0.01f, () => {
			if (gameObject != null)
				StartCoroutine (DetectAnimationOnComplete (onAnimationComplete));
		});
	}

	IEnumerator DetectAnimationOnComplete (System.Action onComplete = null)
	{
		while (animator.GetCurrentAnimatorStateInfo (0).normalizedTime <= 1) {
			yield return null;
		}

		if (onComplete != null && !animator.IsInTransition (0)) {
			onComplete ();
			onComplete = null;
		}
	}

}
