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
    private float alpha;
    public Color faderColor;
    public float fadeTime = 0.5f;

    public Vector3 position = Vector3.zero;

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

    public Tween FadeScene(float to)
    {
        box.DOKill();
        return box.DOFade(to, fadeTime).SetEase(Ease.InOutCubic);
    }
}
