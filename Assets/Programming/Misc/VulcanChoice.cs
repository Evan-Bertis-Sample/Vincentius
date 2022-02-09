using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VulcanChoice : MonoBehaviour
{
    public static VulcanChoice Instance;
    TextUnwrapper.Notify UpdateChoiceEvent;

    public string choice;
    public Level endLevel;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        UpdateChoiceEvent = new TextUnwrapper.Notify((id, args) =>
        {
            if (id == "Vulcan")
            {
                UpdateChoice(args[0]);
            }
        });
        TextUnwrapper.Instance.TextEvent += UpdateChoiceEvent;

        DialogueManager.OnConversationEnd += emitter => TransitionToEnd(emitter);

        choice = "";
    }

    void UpdateChoice(string choice)
    {
        this.choice = choice;
    }

    void TransitionToEnd(DialogueEmitter emitter)
    {
        if (emitter.name != "Vulcan" || choice == "") return;

        LevelManager.Instance.TransitionLevel(endLevel, "Default");
    }

}
