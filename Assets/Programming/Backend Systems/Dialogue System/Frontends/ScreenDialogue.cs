using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Linq;

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

    public GameObject responsePrefab;
    public RectTransform responseTransform;
    public RectTransform selectedCursor;
    public Image selectedCursorImage;
    public Vector3 startingResponsePosOffset = new Vector3(0, 200f, 0);
    public Vector3 cursorOffset = new Vector3(-60f, 0, 0);
    public float responseDistance;
    public List<TextMeshProUGUI> responses = new List<TextMeshProUGUI>();

    private void Start()
    {
        if (selectedCursor == null) return;
        selectedCursor.parent = responseTransform;
        selectedCursorImage = selectedCursor.GetComponent<Image>();
        selectedCursor.gameObject.SetActive(false);
    }

    public override void StartConversation(DialogueEmitter emitter)
    {
        Debug.Log("Start");

        displayImage.DOKill();
        textElement.DOKill();
        buttonPrompt.DOKill();
        buttonWords.DOKill();
        selectedCursorImage?.DOKill();

        fadeTween = ScreenFader.Instance.FadeScene(fadeAmount, -1, 10);
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

        if (responsePrefab != null)
        {
            for (int i = 0; i < responseTransform.childCount; i++)
            {
                if (selectedCursor != null)
                {
                    if (responseTransform.GetChild(i).gameObject == selectedCursor.gameObject) continue;
                }
                Debug.Log("Destroying " + i);
                Destroy(responseTransform.GetChild(i).gameObject);
            }
        }

        if (selectedCursor != null) selectedCursor.gameObject.SetActive(false);


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
        Debug.Log($"Updating... {dialogue.text}");
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

        if (selectedCursorImage != null) selectedCursorImage.DOFade(1, ScreenFader.Instance.fadeTime);
    }

    public override void OnResponseStart(List<NextDialogue> dialogues, NextDialogue selectedDialogue)
    {
        responses.Clear();
        if (dialogues == null) return;
        if (dialogues.Count == 0) return;
        if (responsePrefab == null) return;

        if (dialogues.Where(d => d.previewText != "").Any() == false) return;

        Sequence introSequence = DOTween.Sequence();

        for (int i = 0; i < dialogues.Count; i++)
        {
            Vector3 finalPosOffset = new Vector3(0, -(i * responseDistance));
            GameObject response = Instantiate(responsePrefab, this.responseTransform);
            response.SetActive(true);

            RectTransform curResponseTransform = response.GetComponent<RectTransform>();
            curResponseTransform.parent = response.transform;
            curResponseTransform.anchoredPosition = startingResponsePosOffset + finalPosOffset;

            TextMeshProUGUI responseText = response.GetComponent<TextMeshProUGUI>();
            responseText.SetText(dialogues[i].previewText);
            responses.Add(responseText);
            introSequence.Append(curResponseTransform.DOAnchorPos(finalPosOffset, 0.25f));
        }

        if (selectedCursor == null) return;
        introSequence.OnComplete(() =>
        {
            selectedCursorImage.color = Color.white;
            selectedCursor.gameObject.SetActive(true);
        });
    }

    public override void OnResponseUpdate(List<NextDialogue> dialogues, NextDialogue selectedDialogue)
    {
        if (selectedCursor == null || responsePrefab == null) return;
        if (dialogues == null) return;
        if (dialogues.Count == 0) return;
        if (dialogues.Where(d => d.previewText != "").Any() == false) return;

        int index = dialogues.IndexOf(selectedDialogue);
        Vector3 position = cursorOffset + new Vector3(0, -(index * responseDistance));
        selectedCursor.anchoredPosition = position;
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

        if (responses != null)
        {
            foreach (TextMeshProUGUI tm in responses)
            {
                tm.DOFade(0, ScreenFader.Instance.fadeTime);
            }
        }

        if (selectedCursorImage != null) selectedCursorImage.DOFade(0, ScreenFader.Instance.fadeTime);

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
                ScreenFader.Instance.ResetFader(10);
            }

            for (int i = 0; i < responseTransform.childCount; i++)
            {
                if (selectedCursor != null)
                {
                    if (responseTransform.GetChild(i).gameObject == selectedCursor.gameObject) continue;
                }
                Debug.Log("Destroying " + i);
                Destroy(responseTransform.GetChild(i).gameObject);
            }

        });
    }
}
