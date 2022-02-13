using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;
    public Image box;
    public RectTransform boxTransform;

    public Color faderColor;
    public float fadeTime = 0.5f;

    public Vector3 position = Vector3.zero;
    public Tween currentTween;

    public int currentHighestPriority;

    public float startFadeAmount;

    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        if (box == null) box = GetComponent<Image>();
        boxTransform = box.GetComponent<RectTransform>();
        boxTransform.anchoredPosition = position;
        box.color = new Color(faderColor.r, faderColor.g, faderColor.b, 0);
    }

    private void LateUpdate() {
        if (currentTween != null)
        {
            if(currentTween.IsComplete() || !currentTween.IsActive())
            {
                currentHighestPriority = 0;
            }
        }
    }

    public Tween FadeScene(float to, float from = -1, int priority = 1)
    {

        if (priority < currentHighestPriority) return null;

        currentHighestPriority = priority;
        startFadeAmount = box.color.a;

        if (from != -1) box.color = new Color(faderColor.r, faderColor.g, faderColor.b, from);

        if (currentTween != null)
        {
            if(!currentTween.IsComplete())
            {
                box.DOKill();
            }
        }

        currentTween = box.DOFade(to, fadeTime).SetEase(Ease.InOutCubic);

        return currentTween;
    }

    public Tween FadeSceneSpeed(float to, float speed, float from = -1, int priority = 1)
    {
        if (priority < currentHighestPriority) return null;

        currentHighestPriority = priority;
        startFadeAmount = box.color.a;

        if (from != -1) box.color = new Color(faderColor.r, faderColor.g, faderColor.b, from);

        if (currentTween != null)
        {
            if(!currentTween.IsComplete())
            {
                box.DOKill();
            }
        }

        currentTween = box.DOFade(to, speed).SetEase(Ease.InOutCubic).OnComplete(() => currentHighestPriority = 0);

        return currentTween;
    }

    public void SetAlpha(float a, int priority)
    {
        if (priority <= currentHighestPriority) return;
        box.color = new Color(faderColor.r, faderColor.g, faderColor.b, a);
    }

    public float GetAlpha()
    {
        return box.color.a;
    }

    public Tween ResetFader(int priority = 1)
    {
        if (priority < currentHighestPriority) return null;

        currentHighestPriority = priority;

        if (currentTween != null)
        {
            if(!currentTween.IsComplete())
            {
                box.DOKill();
            }
        }

        float fadeAmount = startFadeAmount;
        if (TimeOfDayManager.Instance.current != null)
        {
            fadeAmount = TimeOfDayManager.Instance.current.fadeAmount;
        }
        currentTween = box.DOFade(fadeAmount, fadeTime).SetEase(Ease.InOutCubic).OnComplete(() => currentHighestPriority = 0);

        return currentTween;
    }

    public Tween ResetFaderSpeed(float speed, int priority = 1)
    {
        if (priority < currentHighestPriority) return null;
        Debug.Log("Resetting Fade");

        currentHighestPriority = priority;

        if (currentTween != null)
        {
            if(!currentTween.IsComplete())
            {
                box.DOKill();
            }
        }

        float fadeAmount = startFadeAmount;
        if (TimeOfDayManager.Instance.current != null)
        {
            fadeAmount = TimeOfDayManager.Instance.current.fadeAmount;
        }
        currentTween = box.DOFade(fadeAmount, fadeTime).SetEase(Ease.InOutCubic).OnComplete(() => currentHighestPriority = 0);

        return currentTween;
    }
}
