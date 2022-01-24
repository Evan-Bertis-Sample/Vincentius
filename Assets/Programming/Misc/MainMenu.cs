using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Tween sceneFade;
    public RectTransform title;
    public List<RectTransform> buttons = new List<RectTransform>();
    private List<Vector2> originalPositions = new List<Vector2>();

    public float transitionTime = 0.75f;

    public Vector2 spawnOffset = new Vector2(400, 0);

    public void Start()
    {
        LevelManager.Instance.player.gameObject.SetActive(false);
        sceneFade = ScreenFader.Instance.FadeScene(0, 1f);

        Vector2 originalPosition = title.anchoredPosition;
        title.anchoredPosition -= spawnOffset;

        foreach(RectTransform button in buttons)
        {
            originalPositions.Add(button.anchoredPosition);
            button.anchoredPosition -= spawnOffset;
        }

        title.DOAnchorPos(originalPosition, transitionTime).OnComplete(AnimateButtons);
    }

    private void AnimateButtons()
    {
        if (buttons == null) return;

        Sequence buttonSequence = DOTween.Sequence();

        for (int i = 0; i < buttons.Count; i++)
        {
            Vector2 originalPosition = originalPositions[i];
            buttonSequence.Append(buttons[i].DOAnchorPos(originalPosition, transitionTime));
        }
    }

    public void LoadScene(string sceneName)
    {
        Level newLevel = LevelManager.Instance.FindLevel(sceneName);
        LevelManager.Instance.TransitionLevel(newLevel, "Default");
    }
}
