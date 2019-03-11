using UnityEngine;
using System.Collections;

public class QuestPanelController : MonoBehaviour
{

  // Use this for initialization
  private UIGrid questGrid;
  private GameObject pf_questItem;

  void Awake ()
  {
    AssignObject ();
  }

  void AssignObject ()
  {
    questGrid = Master.GetChildByName (gameObject, "ListQuests").GetComponent<UIGrid> ();
    pf_questItem = Master.GetGameObjectInPrefabs ("UI/QuestItem");
  }

  public void OnOpen ()
  {
    SetListQuest ();
    if (Master.Tutorial.isDoingTutorial && Master.Tutorial.currentTutorialIndex == TutorialController.TutorialsIndex.GetRewardOfQuest) {
      Master.Tutorial.GoToNextStep ();
      Vector3 posArrow = Vector3.zero;
      foreach (QuestItemController quest in FindObjectsOfType<QuestItemController>()) {

        if (quest.isComplete) {
          posArrow = quest.transform.position;
          break;
        }
       
      }
      Master.GetChildByName (Master.Tutorial.currentStepGO, "Arrows").transform.position = posArrow - new Vector3 (-0.43f, 0.05f, 0);
    }
  }

  public void SetListQuest ()
  {
    Master.QuestData.LoadQuestData ();
    ClearQuestObject ();
    foreach (QuestDataController.QuestData questData in Master.QuestData.questDataCollection.ListQuestData) {
      if (questData.CurrentProgressValue > questData.RequireValue.Value)
        questData.isCompleted = 1;
    }
    Master.QuestData.questDataCollection.ListQuestData.Sort ((a, b) => {
      // compare b to a to get descending order
      int result = b.isCompleted.CompareTo (a.isCompleted);
      return result;
    });
    foreach (QuestDataController.QuestData questData in Master.QuestData.questDataCollection.ListQuestData) {
      GameObject questItem = NGUITools.AddChild (questGrid.gameObject, pf_questItem);
      questItem.GetComponent<QuestItemController> ().SetAttribute (questData);
    }
    questGrid.Reposition ();

  }

  void ClearQuestObject ()
  {
    while (questGrid.gameObject.transform.childCount > 0)
      NGUITools.Destroy (questGrid.gameObject.transform.GetChild (0).gameObject);
  }
}
