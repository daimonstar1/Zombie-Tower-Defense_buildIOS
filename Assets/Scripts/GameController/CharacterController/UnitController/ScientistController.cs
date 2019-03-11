using UnityEngine;
using System.Collections;
using Spine.Unity;

public class ScientistController : MonoBehaviour
{

  // Use this for initialization
  public int id;
  GameObject sprite;
  public float health;
  public int currentLane;
  public float percentIncreaseHealthPerLevel = 5;
  bool isDead = false;
  float currentHealth;
  [HideInInspector]
  public SkeletonAnimation anim;
  UISlider healthSlider;

  void Awake ()
  {
    sprite = Master.GetChildByName (gameObject, "Sprite");
    healthSlider = Master.GetChildByName (gameObject, "HealthBar").GetComponent<UISlider> ();
    anim = GetComponent<SkeletonAnimation> ();
    sprite.gameObject.SetActive (false);
  }

  private void Idle ()
  {
    anim.AnimationState.ClearTracks ();
    anim.AnimationState.SetAnimation (0, "Idle", true);
    anim.AnimationState.Event += _SpineEvent;
    anim.loop = true;
  }

  private IEnumerator Dead ()
  {
    anim.AnimationState.ClearTracks ();
    anim.AnimationState.SetAnimation (0, "Dead", false);
    anim.AnimationState.Event += _SpineEvent;
    anim.loop = false;
    yield return new WaitForSeconds (0.7f);
    Master.Level.ScientistDead (id);
    Master.Lane.RemoveCharacterAtLane (currentLane, gameObject);
    Destroy (gameObject);

  }

  private void _SpineEvent (Spine.TrackEntry track, Spine.Event e)
  {
    anim.AnimationState.Event -= _SpineEvent;
    Debug.LogError (e.Data.Name + "");
  }

  void Start ()
  {
    currentHealth = health;
//    FrameAnimation.PlayAnimation (sprite.GetComponent<SpriteRenderer> (), "Textures/Characters/Units/Scientist/Idle", -1, -1, 0.05f, true, null, this);
    Idle ();
    Master.Tutorial.CheckAndStartTutorial (TutorialController.TutorialsIndex.SaveTheScientist, gameObject.transform.position - new Vector3 (0, 0.1f, 0), true, 5);
    transform.localScale = new Vector3 (31f, 31f, 31f);
    anim.GetComponent<MeshRenderer> ().sortingOrder = -1000 - (int)gameObject.transform.localPosition.y;
  }

  // Update is called once per frame
  void Update ()
  {
    if (currentHealth == health) {
      healthSlider.gameObject.SetActive (false);
    } else {
      healthSlider.gameObject.SetActive (true);
    }

    if (healthSlider.value <= 0 && !isDead) {
      isDead = true;
      healthSlider.gameObject.SetActive (false);
      Master.Audio.PlaySound ("snd_unitDead");
//      FrameAnimation.PlayAnimation (sprite.GetComponent<SpriteRenderer> (), "Textures/Characters/Units/Scientist/Dead", -1, -1, 0.05f, false, () => {
//      }, this);
      StartCoroutine (Dead ());
    }

    if (!isDead) {
      if (healthSlider.value > currentHealth / health) {
        healthSlider.value -= Time.deltaTime;
      }
      if (healthSlider.value <= currentHealth / health) {
        healthSlider.value = currentHealth / health;
      }
    }

  }

  void CollisionController (GameObject obj)
  {
    if (obj.tag == "WeaponEnemy") {
      if (!isDead) {
        GetHit (obj.transform.parent.gameObject.GetComponent<EnemyController> ().data.Damage);
      }
    }
  }

  public virtual void GetHit (float damage)
  {
    currentHealth -= damage;
  }

  void OnCollisionEnter2D (Collision2D coll)
  {
    CollisionController (coll.gameObject);
  }

  void OnTriggerEnter2D (Collider2D coll)
  {
    CollisionController (coll.gameObject);
  }


}
