using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Transition : MonoBehaviour
{

    // Use this for initialization
    public System.Action tempOnHalfComplete;
    public System.Action tempOnComplete;

    GameObject leftDoor;
    GameObject rightDoor;
    GameObject centerDoor;
    Transform[] leftDoorPos = new Transform[2];
    Transform[] rightDoorPos = new Transform[2];
    Transform[] centerDoorPos = new Transform[2];

    GameObject loadingLabel;
    //   GameObject logo;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void doTransition(System.Action onHalfComplete = null, System.Action onComplete = null, float time = 0.6f, float delay = 0.2f)
    {
        leftDoor = Master.GetChildByName(gameObject, "LeftDoor");
        rightDoor = Master.GetChildByName(gameObject, "RightDoor");
        centerDoor = Master.GetChildByName(gameObject, "CenterDoor");
        loadingLabel = Master.GetChildByName(gameObject, "LoadingLabel");
        leftDoorPos[0] = Master.GetChildByName(gameObject, "LeftDoorFirstPos").transform;
        leftDoorPos[1] = Master.GetChildByName(gameObject, "LeftDoorEndPos").transform;
        rightDoorPos[0] = Master.GetChildByName(gameObject, "RightDoorFirstPos").transform;
        rightDoorPos[1] = Master.GetChildByName(gameObject, "RightDoorEndPos").transform;
        centerDoorPos[0] = Master.GetChildByName(gameObject, "CenterDoorFirstPos").transform;
        centerDoorPos[1] = Master.GetChildByName(gameObject, "CenterDoorEndPos").transform;

        // logo = Master.GetChildByName(gameObject, "Logo");

        //logo.GetComponent<UITexture>().color = new Color(1, 1, 1, 0);
        //TweenAlpha.Begin(logo, time, 1).ignoreTimeScale = true;
        leftDoor.transform.position = leftDoorPos[0].position;
        rightDoor.transform.position = rightDoorPos[0].position;
        centerDoor.transform.position = centerDoorPos[0].position;

        loadingLabel.SetActive(false);
        //   float firstPos = leftDoor.transform.position.x;

        Master.Audio.PlaySound("snd_transition", 1);

        centerDoor.transform.DOMoveY(centerDoorPos[1].position.y, time).SetUpdate(true);

        leftDoor.transform.DOMoveX(leftDoorPos[1].position.x, time).SetUpdate(true);
        rightDoor.transform.DOMoveX(rightDoorPos[1].position.x, time).SetUpdate(true).OnComplete(() =>
        {
            loadingLabel.SetActive(true);
            Master.Audio.StopSound();
            if (tempOnHalfComplete != null)
            {
                tempOnHalfComplete();
                tempOnHalfComplete = null;
            }
            else if (onHalfComplete != null)
            {
                onHalfComplete();
            }

            Master.WaitAndDo(delay, () =>
            {
                loadingLabel.SetActive(false);
                Master.Audio.PlaySound("snd_transition", 1);
                // TweenAlpha.Begin(logo, time * 0.5f, 0).ignoreTimeScale = true;
                leftDoor.transform.DOMoveX(leftDoorPos[0].position.x, time).SetUpdate(true);
                centerDoor.transform.DOMoveY(centerDoorPos[0].position.y, time).SetUpdate(true);
                rightDoor.transform.DOMoveX(rightDoorPos[0].position.x, time).SetUpdate(true).OnComplete(() =>
                {
                    Master.Audio.StopSound();

                    if (tempOnComplete != null)
                    {
                        tempOnComplete();
                        tempOnComplete = null;
                    }
                    else if (onComplete != null)
                    {
                        onComplete();
                    }
                    Destroy(gameObject);
                });
            }, this, true);

        });
    }
}
