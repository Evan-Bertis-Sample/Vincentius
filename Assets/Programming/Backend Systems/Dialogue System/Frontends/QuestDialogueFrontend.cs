using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestDialogueFrontend : DialogueFrontend
{
    [Header("Elements")]
    public RectTransform UIElement;
    public Image backImage;
    public TextMeshPro mainTextBox;

    [Header("Back Animation")]
    public Sprite[] backImageAnimation;
    public int currentSpriteIndex;
    public float textBoxAnimationTime = 0.25f;
    public float timeElapsed;
    public int displayStartFrame = 0;

    private void Start() 
    {
        UIElement.gameObject.SetActive(false);
    }

    public override void StartConversation(DialogueEmitter emitter)
    {
        timeElapsed = 0;
        backImage.sprite = backImageAnimation[0];
        UIElement.gameObject.SetActive(true);
        mainTextBox?.gameObject.SetActive(false);
        backImage.SetNativeSize();
    }

    public override void OnDisplayStart(DialogueEmitter emitter, Dialogue dialogue)
    {
        backImage.sprite = backImageAnimation[0];
        mainTextBox?.gameObject.SetActive(false);
    }

    public override void OnDisplayUpdate(DialogueEmitter emitter, Dialogue dialogue)
    {
        timeElapsed += Time.deltaTime;
        currentSpriteIndex = Mathf.RoundToInt(Mathf.Lerp(0, backImageAnimation.Length - 1, Mathf.InverseLerp(0, textBoxAnimationTime, timeElapsed)));
        backImage.sprite = backImageAnimation[currentSpriteIndex];

        if (currentSpriteIndex >= displayStartFrame)
        {
            mainTextBox?.gameObject.SetActive(true);
            displayedText = TextUnwrapper.Instance.UnwrapText(dialogue.text);
            mainTextBox?.SetText(displayedText);
        }
    }

    public override void OnDisplayEnd(DialogueEmitter emitter, Dialogue dialogue)
    {
        TextUnwrapper.Instance.RemoveRequest(dialogue.text);
        backImage.sprite = backImageAnimation[backImageAnimation.Length - 1];
        mainTextBox?.SetText(displayedText);
    }

    public override void OnResponseStart(List<NextDialogue> dialogues, NextDialogue selectedDialogue)
    {

    }

    public override void OnResponseUpdate(List<NextDialogue> dialogues, NextDialogue selectedDialogue)
    {

    }

    public override void EndConversation(DialogueEmitter emitter)
    {
        UIElement.gameObject.SetActive(false);
    }

    
    private void SetElementLocation(GameObject gObject)
    {

    }
}
