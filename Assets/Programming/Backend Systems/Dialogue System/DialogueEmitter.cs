using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class DialogueEmitter : MonoBehaviour
{
    public string characterName;
    public Sprite characterPortrait;

    //List in order of priority
    [Header("Dialogues")]
    public List<Dialogue> startingDialogues;
    public List<Dialogue> endingDialogues;

    [Header("Options")]
    public bool cannotBeObstructed = true;
    public bool showSpeechBubble = true;
    public bool active;
    public bool requireInput = true;
    public bool finishBeforeExit = false;
    public bool disablePlayerMovement = false;

    [Header("Speech Bubble Options")]
    public Vector2 speechBubbleOffset = new Vector2(0.5f, 0.5f);
    private GameObject speechBubble;
    private SpriteRenderer speechBubbleSr;
    public bool isShowingSpeechBubble;

    [Header("Display Options")]
    public DialogueFrontend frontend;
    public int frontendIndex = 0;
    private DialogueManager DM;

    DialogueManager.OnConvo OnConvoEnd;

    private void Start() 
    {
        DM = DialogueManager.Instance;
        speechBubble = new GameObject(gameObject.name + " SpeechBubble");
        speechBubble.transform.parent = transform;
        speechBubbleSr = speechBubble.AddComponent<SpriteRenderer>();
        speechBubbleSr.sprite = DM.speechBubbleSprite;
        speechBubbleSr.sortingLayerName = DM.dialogueSortingLayer;
        speechBubbleSr.material = DM.spriteMaterial;
        speechBubble.SetActive(false);

        frontend = DialogueManager.Instance.GetFrontend(frontendIndex);

        InitEmitter();

        DialogueManager.OnConversationEnd += (em => {
            if (em == this && finishBeforeExit) DM.RemoveEmitter(this); 
        });

        DialogueManager.OnConversationEnd += OnConvoEnd;
    }

    public virtual void InitEmitter()
    {
        //Default Behavior
    }

    public Dialogue GetStartingDialogue()
    {
        if(startingDialogues.Count == 0) return null;
        if(startingDialogues.Count == 1) return startingDialogues.First();

        List<Dialogue> canSee = startingDialogues.Where((d) => d.CheckDialogue() == true).ToList();
        return canSee.First();
    }

    public Dialogue GetEndingDialogue()
    {
        if(endingDialogues.Count == 0) return null;
        if(endingDialogues.Count == 1) return endingDialogues.First();

        List<Dialogue> canSee = endingDialogues.Where((d) => d.CheckDialogue() == true).ToList();
        return canSee.First();
    }

    public void ShowSpeechBubble(bool right)
    {
        if (!showSpeechBubble) return;
        
        bool alreadyActive = speechBubble.activeInHierarchy;

        speechBubble.SetActive(true);
        
        speechBubble.transform.position = (Vector2)transform.position + new Vector2(speechBubbleOffset.x * ((right) ? 1 : -1), speechBubbleOffset.y);
        speechBubbleSr.flipX = !right;

        if(!alreadyActive)
        {
            speechBubble.transform.DOScale(Vector3.one, DM.speechBubbleDisplayTime).SetEase(Ease.InOutBounce);
            speechBubble.transform.DOMoveY(speechBubble.transform.position.y + DM.speechBubbleBobAmplitude, DM.speechBubbleBobSpeed).SetLoops(-1,LoopType.Yoyo).SetEase(Ease.InOutCubic);
            isShowingSpeechBubble = true;
        }
    }

    public void HideSpeechBubble()
    {
        if(isShowingSpeechBubble == false) return;
        isShowingSpeechBubble = false;
        speechBubble.transform.DOKill();

        speechBubble.transform.DOScale(Vector3.zero, DM.speechBubbleDisplayTime).SetEase(Ease.InCubic).OnComplete(() => {
            speechBubble.transform.DOKill();
            speechBubble.SetActive(false);
        });
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.GetComponent<DialogueInteractor>())
        {
            DM.AddEmitter(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (!finishBeforeExit) DM.RemoveEmitter(this);
    }
}
