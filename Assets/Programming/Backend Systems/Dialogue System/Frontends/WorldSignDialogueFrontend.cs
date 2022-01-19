using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class WorldSignDialogueFrontend : DialogueFrontend
{
    [Header("Element Position")]
    public Canvas parentCanvas;
    public GameObject UIElement;
    public SpriteRenderer backImage;
    public Vector3 elementOffset = new Vector3(0, 1, 0);
    private GameObject lastEmitter;

    [Header("Back Animation")]
    public Sprite[] backImageAnimation;
    public int currentSpriteIndex;
    public float textBoxAnimationTime = 0.25f;
    public float timeElapsed;
    public int displayStartFrame = 8;

    [Header("Display")]
    public TextMeshPro mainTextBox;
    public SpriteRenderer displayImage;


    private void Start() 
    {
        UIElement.gameObject.SetActive(false);
        if (parentCanvas == null) parentCanvas = UIElement.transform.parent.GetComponent<Canvas>();
        lastEmitter = null;
    }

    private void LateUpdate()
    {
        SetElementLocation(lastEmitter);
    }

    public override void StartConversation(DialogueEmitter emitter)
    {
        timeElapsed = 0;
        backImage.sprite = backImageAnimation[0];
        UIElement.gameObject.SetActive(true);
        displayImage.gameObject.SetActive(false);
        mainTextBox.gameObject.SetActive(false);
    }

    public override void OnDisplayStart(DialogueEmitter emitter, Dialogue dialogue)
    {
        lastEmitter = emitter.gameObject;
        backImage.sprite = backImageAnimation[0];
        displayImage.gameObject.SetActive(false);
        mainTextBox.gameObject.SetActive(false);
    }

    public override void OnDisplayUpdate(DialogueEmitter emitter, Dialogue dialogue)
    {
        timeElapsed += Time.deltaTime;
        currentSpriteIndex = Mathf.RoundToInt(Mathf.Lerp(0, backImageAnimation.Length - 1, Mathf.InverseLerp(0, textBoxAnimationTime, timeElapsed)));
        backImage.sprite = backImageAnimation[currentSpriteIndex];

        if (currentSpriteIndex >= displayStartFrame)
        {
            mainTextBox.gameObject.SetActive(true);
            displayedText = TextUnwrapper.Instance.UnwrapText(dialogue.text);
            mainTextBox.SetText(displayedText);
        }
    }

    public override void OnDisplayEnd(DialogueEmitter emitter, Dialogue dialogue)
    {
        TextUnwrapper.Instance.RemoveRequest(dialogue.text);
        backImage.sprite = backImageAnimation[backImageAnimation.Length - 1];
        mainTextBox.SetText(displayedText);

        if (dialogue.GetType() != typeof(ImageDialogue)) return;

        ImageDialogue imgDi = (ImageDialogue)dialogue;

        displayImage.sprite = imgDi.image;
        displayImage.gameObject.SetActive(true);

        displayImage.color = new Color(1, 1, 1, 0);
        displayImage.DOFade(1, 0.5f);
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

    public Vector3 worldToUISpace(Canvas parentCanvas, Vector3 worldPos)
    {
        //Convert the world for screen point so that it can be used with ScreenPointToLocalPointInRectangle function
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        Vector2 movePos;

        //Convert the screenpoint to ui rectangle local point
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, screenPos, parentCanvas.worldCamera, out movePos);
        //Convert the local point to world point
        return parentCanvas.transform.TransformPoint(movePos);
    }

    
    private void SetElementLocation(GameObject gObject)
    {
        if (gObject == null) return;
        UIElement.transform.position = gObject.transform.position + elementOffset;
    }
}
