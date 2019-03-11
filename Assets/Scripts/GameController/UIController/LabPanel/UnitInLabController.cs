using UnityEngine;
using System.Collections;

public class UnitInLabController : MonoBehaviour
{

	public string unitID;

	void Start ()
	{
		transform.localScale = Vector3.one * 1.2f;
	}

	public void Active (string unitID)
	{
		this.unitID = unitID;
		SpriteRenderer spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer> ();
		spriteRenderer.sprite = Resources.Load<Sprite> ("Textures/Characters/Units/Unit_" + unitID + "/Normal/Idle");
		spriteRenderer.sortingOrder = 100 - (int)gameObject.transform.parent.localPosition.y;
	}

	public void OnClick ()
	{
		Master.UIMenu.panels [1].GetComponent<UnitPanelController> ().unitSelectedID = unitID;
		Master.UIMenu.OpenPanel (1);
		Master.UIMenu.panels [1].GetComponent<UnitPanelController> ().SetInfo ();
		Master.UIMenu.panels [1].GetComponent<UnitPanelController> ().SetUnitSelectButtonTexture ();

		if (Master.Tutorial.isDoingTutorial) {
			Master.Tutorial.currentStepGO.SetActive (false);
		}

	}
}
