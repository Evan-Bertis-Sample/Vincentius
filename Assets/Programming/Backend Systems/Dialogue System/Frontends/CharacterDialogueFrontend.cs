using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class CharacterDialogueFrontend : DialogueFrontend
{
    [Header("Box Positions")]
    public Vector3 elementPosition;
    public Vector3 hiddenElementPosition;

    [Header("Objects")]
    public GameObject UIElement;
    private RectTransform UIElementTransform;
    public TextMeshProUGUI mainTextBox;
    public TextMeshProUGUI nameTextBox;

    [Header("Response Settings")]
    public RectTransform responseTransform;
    public float responseDistance;
    public float responseStartX = 200;
    public TMP_FontAsset responseFont;
    public Color responseColor;
    public float responseFontSize = 32f;
    public RectTransform selectedCursor;
    public float cursorXOffset = -50f;
    public float cursorYOffset = 25f;

    [Header("Camera Adjustment Settings")]
    public FollowOffset fo;
    public float minYScreen = 0.5f;

    [Header("Sound Settings")]
    public string startSound = "PageTurn";

    public void Start() 
    {
        UIElementTransform = UIElement.GetComponent<RectTransform>();
        UIElementTransform.anchoredPosition = hiddenElementPosition;
        fo = FollowOffset.main;

        selectedCursor.parent = responseTransform;
        selectedCursor.gameObject.SetActive(false);
    }
    
    public override void StartConversation(DialogueEmitter emitter)
    {
        mainTextBox?.SetText("");
        nameTextBox?.SetText(emitter.characterName);
        UIElementTransform.DOAnchorPos(elementPosition, 0.5f);

        float startCamPos = Mathf.Lerp(0, 1, Mathf.InverseLerp(0, Camera.main.pixelHeight, Camera.main.WorldToScreenPoint(fo.transform.parent.position).y));
        if (startCamPos >= minYScreen) return;

        float newFollowY = Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelHeight * (minYScreen - startCamPos))).y;

        fo.SetPosition(new Vector3(fo.GetPosition().x, newFollowY));

        AudioManager.Instance.PlaySound(startSound);
    }

    public override void OnDisplayStart(DialogueEmitter emitter, Dialogue dialogue)
    {
        mainTextBox?.SetText("");
        Debug.Log($"Displaying Dialogue: {dialogue.text}");

        for(int i = 0; i < responseTransform.childCount; i++)
        {
            if(responseTransform.GetChild(i).gameObject == selectedCursor.gameObject) continue;
            Destroy(responseTransform.GetChild(i).gameObject);
        }

        selectedCursor.gameObject.SetActive(false);
    }

    public override void OnDisplayUpdate(DialogueEmitter emitter, Dialogue dialogue)
    {
        Debug.Log("Updating Dialogue");
        displayedText = TextUnwrapper.Instance.UnwrapText(dialogue.text);
        mainTextBox?.SetText(displayedText);
    }

    public override void OnDisplayEnd(DialogueEmitter emitter, Dialogue dialogue)
    {
        Debug.Log($"Finished Displaying Dialogue: {dialogue.text}");
        TextUnwrapper.Instance.RemoveRequest(dialogue.text);
        mainTextBox?.SetText(displayedText);
    }

    public override void OnResponseStart(List<NextDialogue> dialogues, NextDialogue selectedDialogue)
    {
        if (dialogues == null) return;
        if (dialogues.Count == 0 || dialogues.Count == 1) return;

        Sequence introSequence = DOTween.Sequence();

        for (int i = 0; i < dialogues.Count; i ++)
        {
            Vector3 position = new Vector3(0,- (i * responseDistance));
            GameObject responseObject = new GameObject($"Response: {i}");
            RectTransform curResponseTransform = responseObject.AddComponent<RectTransform>();
            TextMeshProUGUI tm = responseObject.AddComponent<TextMeshProUGUI>();

            curResponseTransform.parent = responseTransform;
            curResponseTransform.anchoredPosition = new Vector3(responseStartX, position.y);
            introSequence.Append(curResponseTransform.DOAnchorPos(position, 0.25f));
            tm.font = responseFont;
            tm.color = responseColor;
            tm.fontSize = responseFontSize;
            tm.enableWordWrapping = false;
            curResponseTransform.localScale = Vector3.one;

            tm.SetText(dialogues[i].previewText);
        }

        introSequence.OnComplete(() => selectedCursor.gameObject.SetActive(true));
    }

    public override void OnResponseUpdate(List<NextDialogue> dialogues, NextDialogue selectedDialogue)
    {
        int index = dialogues.IndexOf(selectedDialogue);
        Vector3 position = new Vector3(cursorXOffset, -(index * responseDistance) + cursorYOffset);
        selectedCursor.anchoredPosition = position;
    }

    public override void EndConversation(DialogueEmitter emitter)
    {
        UIElementTransform.DOAnchorPos(hiddenElementPosition, 0.5f);
        fo.ResetPosition();
    }

}
