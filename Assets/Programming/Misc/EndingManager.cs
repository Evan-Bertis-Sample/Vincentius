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

    public Ending chosenEnding;

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

        LevelManager.OnSceneChange += newScene =>
        {
            switch (choice)
            {
                case "KilledVulcan":
                    chosenEnding = killedVulcanEnding;
                    break;
                case "DestroyPompeii":
                    chosenEnding = allowedVulcanEnding;
                    break;
                case "ConvincedVulcan":
                    chosenEnding = convincedVulcanEnding;
                    break;
                case "FailedConvinced":
                    chosenEnding = failedConvincedEnding;
                    break;
                default:
                    chosenEnding = killedVulcanEnding;
                    break;
            }
            StartCoroutine(SetEnding(chosenEnding));
        };

        LevelManager.OnSceneLateChange += newScene =>
        {
            emitter.gameObject.SetActive(true);
        };
    }

    IEnumerator SetEnding(Ending ending)
    {
        yield return null;
        sr.sprite = ending.back;

        List<Dialogue> newStart = new List<Dialogue>();
        newStart.Add(ending.startingDialogue);

        emitter.startingDialogues = newStart;
        //Debug.Log($"Starting dialogue is : {ending.startingDialogue.text}");
        //emitter.gameObject.SetActive(true);

    }
}
