using UnityEngine;
using System.Collections;
using DG.Tweening;

public class GoldDrop : MonoBehaviour
{
  // Use this for initialization
  private Rigidbody2D rgBody2d;
  private float firstY;
  bool isMoving;

  void Start ()
  {
    gameObject.layer = LayerMask.NameToLayer ("UI");
    gameObject.transform.parent = Master.UIGameplay.uiRoot.transform;
    rgBody2d = GetComponent<Rigidbody2D> ();
    firstY = transform.position.y;
    SetMove ();
    Invoke ("GotGold", 1f);
  }

  // Update is called once per frame
  void Update ()
  {
    if (transform.position.y < firstY) {
      Destroy (rgBody2d);
    }

//        Ray ray = Master.Gameplay.camera.ScreenPointToRay(Input.mousePosition);
//        RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction, Mathf.Infinity);
//        foreach (RaycastHit2D hit in hits)
//        {
//            if (hit)
//            {
//                if (hit.collider.gameObject == gameObject && !isMoving)
//                {
//                    GotGold();
//                }
//            }
//        }
  }

  void SetMove ()
  {
    int[] randomX = new int[] { -20, -18, -16, 16, 18, 20 };
    rgBody2d.AddForce (new Vector3 (randomX [Random.Range (0, randomX.Length)], Random.Range (60, 70)));
  }

  void GotGold ()
  {
    isMoving = true;
    Master.Audio.PlaySound ("snd_getGold");
    transform.DOMove (Master.UIGameplay.totalGoldLabel.transform.position, 0.7f).OnComplete (() => {
      Master.Gameplay.gold += StatsController.GoldPerCoin;
      Destroy (gameObject);
    });
  }

  //public void OnTouchIn()
  //{
  //    Master.Audio.PlaySound("snd_getGold");
  //    transform.DOMove(Master.UIGameplay.totalGoldLabel.transform.position, 0.7f).OnComplete(() =>
  //    {
  //        Master.Gameplay.gold += StatsController.GoldPerCoin;
  //        Destroy(gameObject);
  //    });
  //}

  //public void OnTouching()
  //{
  //    Master.Audio.PlaySound("snd_getGold");
  //    transform.DOMove(Master.UIGameplay.totalGoldLabel.transform.position, 0.7f).OnComplete(() =>
  //    {
  //        Master.Gameplay.gold += StatsController.GoldPerCoin;
  //        Destroy(gameObject);
  //    });
  //}

}
