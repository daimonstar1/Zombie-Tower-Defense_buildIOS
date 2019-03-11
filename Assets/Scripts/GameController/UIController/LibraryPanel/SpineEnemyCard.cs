using UnityEngine;
using System.Collections;
using Spine.Unity;

public class SpineEnemyCard : MonoBehaviour
{

  // Use this for initialization
  void Start ()
  {
    GetComponent<SkeletonAnimation> ().timeScale = 99;
    GetComponent<SkeletonAnimation> ().loop = false;
    GetComponent<SkeletonAnimation> ().AnimationState.SetAnimation (0, "Walk", false);
  }
	
  // Update is called once per frame
  void Update ()
  {
	
  }
}
