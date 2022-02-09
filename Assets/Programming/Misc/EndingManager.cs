using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingManager : MonoBehaviour
{
    [System.Serializable]
    public class Ending
    {
        public Sprite back;
        public Dialogue startingDialogue;
        public TimeOfDay timeOfDay;
    }

    public Ending killedVulcanEnding;
    public Ending allowedVulcanEnding;
    public Ending convincedVulcanEnding;
    public Ending failedConvincedEnding;

    public AutoEmitter emitter;
    SpriteRenderer sr;

    private void Start()
    {
        string choice = VulcanChoice.Instance.choice;
        if (choice == "") return;

        sr = GetComponent<SpriteRenderer>();
        emitter.gameObject.SetActive(false);

        DialogueManager.OnConversationEnd += emitter =>
        {
            if (emitter == this.emitter)
            {
                DialogueManager.Instance.RemoveEmitter(this.emitter);
            }
        };

        LevelManager.OnSceneLateChange += newScene =>
        {
            Ending ending;
            switch (choice)
            {
                case "KilledVulcan":
                    ending = killedVulcanEnding;
                    break;
                case "DestroyPompeii":
                    ending = allowedVulcanEnding;
                    break;
                case "ConvincedVulcan":
                    ending = convincedVulcanEnding;
                    break;
                case "FailedConvinced":
                    ending = failedConvincedEnding;
                    break;
                default:
                    ending = killedVulcanEnding;
                    break;
            }

            StartCoroutine(SetEnding(ending));
        };
    }

    IEnumerator SetEnding(Ending ending)
    {
        yield return null;
        sr.sprite = ending.back;

        List<Dialogue> newStart = new List<Dialogue>();
        newStart.Add(ending.startingDialogue);

        emitter.startingDialogues = newStart;
        Debug.Log($"Starting dialogue is : {ending.startingDialogue.text}");
        emitter.gameObject.SetActive(true);

    }
}
