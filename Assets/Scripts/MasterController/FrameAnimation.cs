using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class FrameAnimation : MonoBehaviour
{
    public string pathToTextures;
    public int startIndex = -1;
    public int endIndex = -1;
    public float delay = 0.05f;
    public bool isLoop = true;
    public bool playOnAwake = true;
    public EventDelegate onComplete;
    private static List<EventDelegate> onCompletes = new List<EventDelegate>();
    // Use this for initialization

    void Start()
    {

        if (playOnAwake)
        {
            Play();
        }
    }

    public void Play()
    {
        Object imageComponent = null;
        if (GetComponent<SpriteRenderer>() != null)
        {
            imageComponent = GetComponent<SpriteRenderer>();
        }
        else if (GetComponent<UITexture>() != null)
        {
            imageComponent = GetComponent<UITexture>();
        }

        if (onComplete != null)
        {
            onCompletes.Add(onComplete);
        }
        PlayAnimation(imageComponent, pathToTextures, startIndex, endIndex, delay, isLoop, null, this);

    }

    public static void PlayAnimation(Object imageComponent, string pathToTuxturesAnimation, int startIndex, int endIndex, float delay, bool isLoop, System.Action onComplete = null, MonoBehaviour mono = null)
    {
        //check if is UITexture
        if (mono != null) mono.StopAllCoroutines();

        Object[] listTextures;
        bool isSprite;

        try
        {
            SpriteRenderer spriteRenderer = (SpriteRenderer)imageComponent;
            listTextures = Resources.LoadAll(pathToTuxturesAnimation, typeof(Sprite)).Cast<Sprite>().OrderBy(x => int.Parse(x.name)).ToArray();
            isSprite = true;
        }
        catch
        {
            listTextures = Resources.LoadAll(pathToTuxturesAnimation, typeof(Texture2D)).Cast<Texture2D>().OrderBy(x => int.Parse(x.name)).ToArray();
            isSprite = false;
        }

        doPlayAnimation(startIndex, imageComponent, isSprite, listTextures, startIndex, endIndex, delay, isLoop, onComplete, mono);
    }

    private static void doPlayAnimation(int index, Object imageComponent, bool isSprite, Object[] listTextures, int startIndex, int endIndex, float delay, bool isLoop, System.Action onComplete, MonoBehaviour mono)
    {

        Master.WaitAndDo(delay, () =>
        {
            if (imageComponent == null)
            {
                return;
            }

            SpriteRenderer spriteRenderer = null;
            UITexture uiTexture = null;
            Sprite[] listSprites = null;
            Texture2D[] listTextures2D = null;
            if (isSprite)
            {
                spriteRenderer = (SpriteRenderer)imageComponent;
                listSprites = (Sprite[])listTextures;
            }
            else
            {
                uiTexture = (UITexture)imageComponent;
                listTextures2D = (Texture2D[])listTextures;
            }

            if (startIndex != -1)
            {
                if (index > endIndex || index >= listTextures.Length)
                {
                    if (isLoop)
                    {
                        index = startIndex;
                    }
                    else
                    {

                        if (onComplete == null)
                        {
                            if (onCompletes.Count > 0)
                                EventDelegate.Execute(onCompletes);
                        }
                        else
                        {
                            onComplete();
                        }

                        return;
                    }
                }

                if (index >= startIndex && index <= endIndex)
                {
                    if (isSprite)
                    {
                        spriteRenderer.sprite = listSprites[index];
                    }
                    else
                    {
                        uiTexture.mainTexture = listTextures2D[index];
                    }
                    index++;
                }
            }
            else
            {
                if (index == -1)
                {
                    index = 0;
                }

                if (index >= listTextures.Length)
                {
                    if (isLoop)
                    {
                        index = 0;
                    }
                    else
                    {

                        if (onComplete == null)
                        {
                            if (onCompletes.Count > 0)
                                EventDelegate.Execute(onCompletes);
                        }
                        else
                        {
                            onComplete();
                        }
                        return;
                    }
                }

                if (isSprite)
                {
                    spriteRenderer.sprite = listSprites[index];
                }
                else
                {
                    uiTexture.mainTexture = listTextures2D[index];
                }
                index++;
            }
            doPlayAnimation(index, imageComponent, isSprite, listTextures, startIndex, endIndex, delay, isLoop, onComplete, mono);
        }, mono, true);
    }

}
