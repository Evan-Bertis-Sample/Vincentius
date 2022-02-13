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
        public AudioClip backgroundMusic;
    }

    public Ending killedVulcanEnding;
    public Ending allowedVulcanEnding;
    public Ending convincedVulcanEnding;
    public Ending failedConvincedEnding;

    public Ending chosenEnding;

    public AutoEmitter emitter;
    SpriteRenderer sr;


    LevelManager.SceneChange SceneChange;
    LevelManager.SceneChange LateSceneChange;

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

        SceneChange = new LevelManager.SceneChange(newScene =>
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
            LevelManager.OnSceneChange -= SceneChange;
        });

        LevelManager.OnSceneChange += SceneChange;

        LateSceneChange = new LevelManager.SceneChange(newScene =>
        {
            emitter.gameObject.SetActive(true);
            LevelManager.OnSceneLateChange -= LateSceneChange;
        });
 
        LevelManager.OnSceneLateChange += LateSceneChange;
    }

    IEnumerator SetEnding(Ending ending)
    {
        yield return null;
        sr.sprite = ending.back;

        List<Dialogue> newStart = new List<Dialogue>();
        newStart.Add(ending.startingDialogue);

        emitter.startingDialogues = newStart;

        yield return null;

        Debug.Log($"Ending Music: {ending.backgroundMusic.name}");
        AudioManager.Instance.SetBackgroundMusic(ending.backgroundMusic);
        //Debug.Log($"Starting dialogue is : {ending.startingDialogue.text}");
        //emitter.gameObject.SetActive(true);

    }
}
