using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class ScrollAlertNotification : NotificationFrontend
{
    [Header("Elements")]
    public RectTransform UIElement;
    public Image backImage;
    public TextMeshProUGUI mainTextBox;

    [Header("Back Animation")]
    public Sprite[] backImageAnimation;
    public int currentSpriteIndex;
    public float textBoxAnimationTime = 0.25f;
    public float timeElapsed;
    public int displayStartFrame = 0;
    public float objectFadeTime = 0;
    private bool startedObjectFade;
    private bool ready;

    [Header("Text Animation")]
    public float textFadeTime = 0.5f;
    private bool textFadeComplete;

    public override bool StopDisplay(Notification notification)
    {
        return (textFadeComplete);
    }

    public override void OnNotificationStart(Notification notification)
    {
        timeElapsed = 0;
        backImage.sprite = backImageAnimation[0];
        UIElement.gameObject.SetActive(true);
        mainTextBox?.gameObject.SetActive(false);
        backImage.SetNativeSize();
        UIElement.anchoredPosition = Vector2.zero;
        textFadeComplete = false;
        backImage.color = new Color(1, 1, 1, 1);
        startedObjectFade = false;
        ready = false;
    }

    public override void OnNotificationUpdate(Notification notification)
    {
        timeElapsed += Time.deltaTime;
        currentSpriteIndex = Mathf.RoundToInt(Mathf.Lerp(0, backImageAnimation.Length - 1, Mathf.InverseLerp(0, textBoxAnimationTime, timeElapsed)));
        backImage.sprite = backImageAnimation[currentSpriteIndex];

        if (currentSpriteIndex >= displayStartFrame)
        {
            if (mainTextBox.gameObject.activeInHierarchy == false) 
            {
                mainTextBox.color = new Color(1, 1, 1, 0);
                mainTextBox.DOFade(1, textFadeTime).OnComplete(() => textFadeComplete = true);
            }
            mainTextBox.gameObject.SetActive(true);
            mainTextBox.SetText(notification.notificationText);
        }
    }

    public override void OnNotificationEnd(Notification notification)
    {
        textFadeComplete = false;
        TextUnwrapper.Instance.RemoveRequest(notification.notificationText);
    }

    public override bool IsReadyForNext(Notification notification)
    {
        if (!startedObjectFade)
        {
            Debug.Log("Beginning Fade");
            backImage.DOFade(0, objectFadeTime).OnComplete(() =>
            {
                Debug.Log("Finished Fade");
                ready = true;
                UIElement.gameObject.SetActive(false);
                mainTextBox.gameObject.SetActive(false);
            });
            mainTextBox.DOFade(0, objectFadeTime);
            startedObjectFade = true;
        }
        return ready;
    }
}
