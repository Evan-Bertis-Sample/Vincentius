using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    [Header("State Management")]
    public bool paused;
    public InputAction pauseAction;
    
    [Header("Pause State")]
    public RectTransform pauseElement;
    public float showTime = 0.5f;
    public Vector3 hiddenPos = new Vector3(0, -250, 0);
    public Vector3 showPos = Vector3.zero;
    public float fadeAmount = 0.8f;
    public Vector3 rightPageLocation;
    public List<Level> cannotPauseLevels = new List<Level>();
    private float originalFadeAmount;

    [Header("Quest State")]
    public RectTransform questElement;
    public GameObject questCursor;
    public RectTransform questsDisplayTransform;
    public float itemDistance;
    public Vector3 descriptionOffset = new Vector3(20, -50, 0);
    public TMP_FontAsset itemFont;
    public Color itemColor;
    public float itemFontSize = 32f;
    public Color descriptionColor;
    private List<GameObject> questItems = new List<GameObject>();

    [Header("Options State")]
    public RectTransform optionsElement;
    public GameObject optionsCursor;


    public void Awake()
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
        pauseAction.Enable();
        pauseAction.started += context => HandlePauseState();
    }

    public void Start()
    {
        paused = false;
        pauseElement.anchoredPosition = hiddenPos;
    }

    public void HandlePauseState()
    {
        if (cannotPauseLevels.Contains(LevelManager.Instance.activeLevel)) return;
        
        if (!paused)
        {
            paused = true;
            OnPause();
        }
        else
        {
            paused = false;
            OnPauseExit();
        }
    }

    public void OnPause()
    {
        Debug.Log("Pause");
        Time.timeScale = 0;
        originalFadeAmount = ScreenFader.Instance.GetAlpha();
        //pauseElement.anchoredPosition = hiddenPos;
        pauseElement.DOKill();
        pauseElement.DOAnchorPos(showPos, showTime).SetUpdate(true);
        ScreenFader.Instance.FadeSceneSpeed(fadeAmount, showTime).SetUpdate(true);
        optionsElement.gameObject.SetActive(false);
        questElement.gameObject.SetActive(false);
        optionsCursor.SetActive(false);
        questCursor.SetActive(false);

    }

    public void OnPauseExit()
    {
        Debug.Log("Resume");
        Time.timeScale = 1;
        //pauseElement.anchoredPosition = showPos;
        pauseElement.DOKill();
        pauseElement.DOAnchorPos(hiddenPos, showTime);
        ScreenFader.Instance.ResetFaderSpeed(showTime, 0);
    }

    public void Resume()
    {
        paused = false;
        OnPauseExit();
    }

    public void ShowQuests()
    {
        if(!paused) return;
        
        Debug.Log("Showing Quests");

        optionsElement.gameObject.SetActive(false);
        questElement.gameObject.SetActive(true);
        questElement.anchoredPosition = rightPageLocation;
        questCursor.SetActive(true);
        optionsCursor.SetActive(false);

        //Generate quests
        List<Quest> activeQuests = QuestManager.Instance.activeQuests;

        if (questItems.Count > 0)
        {
            foreach(GameObject q in questItems)
            {
                Destroy(q);
            }
            questItems.Clear();
        }

        if (activeQuests.Count == 0)
        {
            GameObject noQuest = CreateTextBox(questsDisplayTransform.position, "There are no active quests");
            noQuest.transform.parent = questsDisplayTransform;
            questItems.Add(noQuest);
        }
        else
        {
            for (int i = 0; i < activeQuests.Count; i++)
            {
                Debug.Log(i * itemDistance);
                Vector3 curItemPosition = new Vector2(0, -(i * itemDistance));
                Debug.Log(curItemPosition);
                GameObject quest = CreateTextBox(curItemPosition, activeQuests[i].questName);
                quest.transform.localScale = Vector3.one;
                quest.transform.parent = questsDisplayTransform;
                quest.transform.GetComponent<RectTransform>().anchoredPosition = curItemPosition;
                questItems.Add(quest);

                if (activeQuests[i].questDescription == "") continue;

                GameObject description = CreateTextBox(curItemPosition + descriptionOffset, activeQuests[i].questDescription);
                description.transform.parent = quest.transform;
                description.GetComponent<TextMeshProUGUI>().color = descriptionColor;
                description.GetComponent<RectTransform>().anchoredPosition = descriptionOffset;
            }
        }
        
    }

    private GameObject CreateTextBox(Vector3 pos, string text)
    {
        GameObject itemObject = new GameObject($"Item");
        RectTransform itemTransform = itemObject.AddComponent<RectTransform>();
        TextMeshProUGUI tm = itemObject.AddComponent<TextMeshProUGUI>();

        itemTransform.anchoredPosition = pos;
        tm.font = itemFont;
        tm.color = itemColor;
        tm.fontSize = itemFontSize;
        tm.enableWordWrapping = false;
        itemTransform.localScale = Vector3.one;
        tm.SetText(text);

        return itemObject;
    }

    public void ShowOptions()
    {
        if(!paused) return;

        Debug.Log("Showing Options");

        questElement.gameObject.SetActive(false);
        optionsElement.gameObject.SetActive(true);
        optionsElement.anchoredPosition = rightPageLocation;
        optionsCursor.SetActive(true);
        questCursor.SetActive(false);

    }

}
