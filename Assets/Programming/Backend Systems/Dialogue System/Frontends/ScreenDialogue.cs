using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ScreenDialogue : DialogueFrontend
{
    [Header("Elements")]
    public RectTransform UIElement;
    public TextMeshProUGUI textElement;
    public Image displayImage;
    public Image buttonPrompt;
    public TextMeshProUGUI buttonWords;

    private Tween fadeTween;
    public bool fadeInComplete;

    public float fadeAmount = 0.5f;

    public bool transitionLevel = false;
    public Level nextLevel;
    public string doorwayID = "default";

    public override void StartConversation(DialogueEmitter emitter)
    {
        Debug.Log("Start");

        displayImage.DOKill();
        textElement.DOKill();
        buttonPrompt.DOKill();
        buttonWords.DOKill();

        fadeTween = ScreenFader.Instance.FadeScene(fadeAmount, -1, 1);
        fadeTween.OnComplete(() =>
        {
            fadeInComplete = true;
        });
        UIElement.gameObject.SetActive(true);
        fadeInComplete = false;
        buttonPrompt.gameObject.SetActive(false);
        textElement.color = new Color(textElement.color.r, textElement.color.g, textElement.color.b, 1);
    }

    public override void OnDisplayStart(DialogueEmitter emitter, Dialogue dialogue)
    {
        Debug.Log("Starting to display dialogue: " + dialogue.text);
        buttonPrompt.gameObject.SetActive(false);

        if (dialogue.GetType() != typeof(ImageDialogue))
        {
            displayImage.gameObject.SetActive(false);
            return;
        };

        ImageDialogue imgDi = (ImageDialogue)dialogue;

        displayImage.gameObject.SetActive(imgDi.image != null);

        if (displayImage.gameObject.activeInHierarchy == false) return;

        displayImage.sprite = imgDi.image;
        displayImage.SetNativeSize();
        displayImage.gameObject.SetActive(true);
        displayImage.color = new Color(1, 1, 1, 0);
        displayImage.DOFade(1, ScreenFader.Instance.fadeTime);
    }

    public override void OnDisplayUpdate(DialogueEmitter emitter, Dialogue dialogue)
    {
        textElement.SetText(displayedText);
        if (fadeInComplete == false) return;
        displayedText = TextUnwrapper.Instance.UnwrapText(dialogue.text);
    }

    public override void OnDisplayEnd(DialogueEmitter emitter, Dialogue dialogue)
    {
        Debug.Log("End");
        TextUnwrapper.Instance.RemoveRequest(dialogue.text);
        textElement.SetText(displayedText);

        buttonPrompt.gameObject.SetActive(true);
        buttonPrompt.color = new Color(1, 1, 1, 0);
        buttonPrompt.DOFade(1, ScreenFader.Instance.fadeTime);
        buttonWords.color = new Color(buttonWords.color.r, buttonWords.color.g, buttonWords.color.b, 0);
        buttonWords.DOFade(1, ScreenFader.Instance.fadeTime);
    }

    public override void OnResponseStart(List<NextDialogue> dialogues, NextDialogue selectedDialogue)
    {

    }

    public override void OnResponseUpdate(List<NextDialogue> dialogues, NextDialogue selectedDialogue)
    {

    }

    public override void EndConversation(DialogueEmitter emitter)
    {
        Debug.Log("End Conversation");

        displayImage.DOKill();
        textElement.DOKill();
        buttonPrompt.DOKill();
        buttonWords.DOKill();

        displayImage.DOFade(0, ScreenFader.Instance.fadeTime);
        buttonPrompt.DOFade(0, ScreenFader.Instance.fadeTime);
        buttonWords.DOFade(0, ScreenFader.Instance.fadeTime);

        textElement.DOFade(0, ScreenFader.Instance.fadeTime).OnComplete(() =>
        {
            if (transitionLevel)
            {
                //Debug.Log("Transition");
                LevelManager.Instance.TransitionLevel(nextLevel, doorwayID);
                return;
            }
            else
            {
                ScreenFader.Instance.FadeScene(0);
            }
        });
    }
}
