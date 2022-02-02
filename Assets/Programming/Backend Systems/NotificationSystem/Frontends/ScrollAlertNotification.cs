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
    public TextMeshProUGUI subTextBox;

    [Header("Back Animation")]
    public Sprite[] backImageAnimation;
    public int currentSpriteIndex;
    public float textBoxAnimationTime = 0.25f;
    public float timeElapsed;
    public int mainTextStartFrame = 14;
    public int noSubTextEndFrame = 20;
    public int subTextStartFrame = 22;
    public float objectFadeTime = 0;
    private bool startedObjectFade;
    private bool ready;

    [Header("Text Animation")]
    public float textFadeTime = 0.5f;
    private bool textFadeComplete;

    public override bool StopDisplay(Notification notification)
    {
        bool animationComplete;
        if (notification.notificationSubText == null)
        {
            animationComplete = (currentSpriteIndex == noSubTextEndFrame);
        }
        else
        {
            animationComplete = (currentSpriteIndex == backImageAnimation.Length - 1);
        }
        return (textFadeComplete && animationComplete);
    }

    public override void OnNotificationStart(Notification notification)
    {
        timeElapsed = 0;
        backImage.sprite = backImageAnimation[0];
        UIElement.gameObject.SetActive(true);
        mainTextBox?.gameObject.SetActive(false);
        subTextBox?.gameObject.SetActive(false);
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


        if (notification.notificationSubText == null && currentSpriteIndex > noSubTextEndFrame)
        {
            currentSpriteIndex = noSubTextEndFrame;
        }

        backImage.sprite = backImageAnimation[currentSpriteIndex];

        if (currentSpriteIndex >= mainTextStartFrame)
        {
            if (mainTextBox.gameObject.activeInHierarchy == false)
            {
                mainTextBox.color = new Color(1, 1, 1, 0);
                mainTextBox.DOFade(1, textFadeTime).OnComplete(() => textFadeComplete = (notification.notificationSubText == null));
            }
            mainTextBox.gameObject.SetActive(true);
            mainTextBox.SetText(notification.notificationText);
        }

        if (currentSpriteIndex >= subTextStartFrame)
        {
            if (subTextBox.gameObject.activeInHierarchy == false)
            {
                subTextBox.color = new Color(1, 1, 1, 0);
                subTextBox.DOFade(1, textFadeTime).OnComplete(() => textFadeComplete = true);
            }
            subTextBox.gameObject.SetActive(true);
            subTextBox.SetText(notification.notificationSubText);
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
            backImage.DOFade(0, objectFadeTime).OnComplete(() =>
            {
                ready = true;
                UIElement.gameObject.SetActive(false);
                mainTextBox.gameObject.SetActive(false);
                subTextBox?.gameObject.SetActive(false);
            });
            mainTextBox.DOFade(0, objectFadeTime);
            subTextBox.DOFade(0, objectFadeTime);
            startedObjectFade = true;
        }
        return ready;
    }
}
