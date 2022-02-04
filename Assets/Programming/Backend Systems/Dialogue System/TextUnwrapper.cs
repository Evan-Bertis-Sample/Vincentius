using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TextUnwrapper : MonoBehaviour
{
    [System.Serializable]
    public class UnwrapRequest
    {
        public string originalText;
        public string currentText;
        public string outputText;
        public bool characterUnwrap = false;
        public float defaultWaitTime {get; private set;} = 0.25f;
        public float currentWaitTime;
        public float pauseTime = 0;
        public List<String> foundCommands = new List<string>();
        public UnwrapRequest(string text)
        {
            originalText = text;
            currentText = "";
            currentWaitTime = defaultWaitTime;
        }

        public string GetOutputText()
        {
            return outputText;
        }

        public void SetDefaultWaitTime(float newTime)
        {
            defaultWaitTime = newTime;
            currentWaitTime = defaultWaitTime;
        }
    }

    public static TextUnwrapper Instance;
    public List<UnwrapRequest> requests = new List<UnwrapRequest>();
    public float defaultWaitTime = 0.1f;
    public bool unwrapCharDefault = true;

    public delegate void Notify(string id);
    public event Notify TextEvent;

    [TextArea]
    public string startingText = "Test";
    [TextArea]
    public string endingText = "";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        DontDestroyOnLoad(this);
    }

    private void Update() {
        endingText = UnwrapText(startingText); 
    }

    public string UnwrapText(string text)
    {
        foreach(UnwrapRequest ur in requests)
        {
            if(ur.originalText == text)
            {
                //It has already been requested
                return ur.GetOutputText();;
            }
        }

        //Add to requests
        UnwrapRequest request = new UnwrapRequest(text);
        request.SetDefaultWaitTime(defaultWaitTime);
        request.characterUnwrap = unwrapCharDefault;
        int index = requests.Count;
        requests.Add(request);
        StartCoroutine(UnwrapText(index));
        return request.GetOutputText();
    }

    public void RemoveRequest(string text)
    {
        foreach(UnwrapRequest ur in requests)
        {
            if(ur.originalText == text)
            {
                requests.Remove(ur);
                break;
            }
        }
    }

    IEnumerator UnwrapText(int index)
    {
        UnwrapRequest request = requests[index];
        while(request.currentText != request.originalText)
        {
            string unrevealed = request.originalText.Remove(0, request.currentText.Length);

            if(unrevealed.Length == 0) break;;

            string nextWord = GetStringBetweenCharacters(unrevealed, unrevealed[0], ' '); //Next word is "word ". The following space is included

            if (nextWord.Contains('<'))
            {
                //Command is starting
                ExecuteCommand(request, unrevealed);
                continue;
            }
            yield return new WaitForSeconds( (request.pauseTime != 0) ? request.pauseTime : request.currentWaitTime);
            request.pauseTime = 0;

            if(!request.characterUnwrap)
            {
                request.currentText += $"{nextWord}";
            }
            else
            {
                char nextChar = unrevealed[0];
                request.currentText += nextChar;
            }
            request.outputText = RemoveCommands(request.currentText);
        }
    }

    #region Reading commands
    private void ExecuteCommand(UnwrapRequest request, string unrevealed)
    {
        //Commands have the syntax <CommandName(parameter1, parameter2, etc)>
        //Debug.Log("Found Command");
        string fullCommand = GetStringBetweenCharacters(unrevealed, '<', '>', 0, false); //Outputs commandName(parameter1, parameter2)
        string marked = (GetStringBetweenCharacters(unrevealed, '<', '>') + " ");
        //if (!marked.Contains('>')) return; //It is not a command
        request.foundCommands.Add(GetStringBetweenCharacters(unrevealed, '<', '>') + " ");
        string commandName = ExtractCommandName(fullCommand);
        string[] parameters = ExtractParameters(fullCommand, commandName);

        switch(commandName)
        {
            case "WaitTime":
                WaitTime(request, parameters);
                break;
            case "Event":
                DialogueEvent(request, parameters);
                break;
            case "UnwrapCharacters":
                UnwrapCharacters(request, parameters);
                break;
            case "Pause":
                Pause(request, parameters);
                break;
            default:
                Debug.LogWarning($"There is no command labled {commandName} that is defined");
                break;
        }

        request.currentText += GetStringBetweenCharacters(unrevealed, unrevealed[0], '>') + " "; //Skip Ahead
    }
    private string ExtractCommandName(string fullCommand)
    {
        string commandName = RemoveWhiteSpace(GetStringBetweenCharacters(fullCommand, fullCommand[0], '(')); //Outputs commandName=
        commandName = commandName.Remove(commandName.Length - 1); //Outputs commandName
        //Debug.Log("The command found is titled: " + commandName);
        return commandName;
    }

    private string[] ExtractParameters(string fullCommand, string commandName)
    {
        string fullParameters = RemoveWhiteSpace(fullCommand.Remove(0, commandName.Length)); //Outputs (parameter1,parameter2)
        fullParameters = GetStringBetweenCharacters(fullParameters, '(', ')', 0, false); //Outputs parameter1,parameter2
        List<string> parameters = new List<string>();

        while(fullParameters.Length > 0)
        {
            string currentParmeter = GetStringBetweenCharacters(fullParameters, fullParameters[0], ',');
            fullParameters = fullParameters.Remove(0, currentParmeter.Length);
            parameters.Add(currentParmeter.TrimEnd( (',') ));
        }
        //Debug.Log($"The parameters of {commandName} are {string.Join(", ", parameters)}");

        return parameters.ToArray();
    }
    
    #endregion

    #region Commands
    private void WaitTime(UnwrapRequest request, string[] parameters)
    {
        if (parameters[0] == "default")
        {
            request.currentWaitTime = request.defaultWaitTime;
            return;
        }
        float newWaitTime = float.Parse(parameters[0]);
        request.currentWaitTime = newWaitTime;
    }

    private void DialogueEvent(UnwrapRequest request, string[] parameters)
    {
        Debug.Log("Called");
        TextEvent?.Invoke(parameters[0]);
    }

    private void UnwrapCharacters(UnwrapRequest request, string[] parameters)
    {
        request.characterUnwrap = (bool.Parse(parameters[0]));
    }

    private void Pause(UnwrapRequest request, string[] parameters)
    {
        request.pauseTime = float.Parse(parameters[0]);
    }

    public String RemoveCommands(string request)
    {
        string examine = request;
        List<string> commands = new List<string>();

        for(int i = 0; i < examine.Length; i++)
        {
            if(examine[i] == '<')
            {
                //Found command
                string suspectedCommand = (GetStringBetweenCharacters(examine, '<', '>', i));
                //if (!suspectedCommand.Contains('>')) continue; //This is not a full command
                commands.Add(suspectedCommand);
            }
        }

        foreach(string com in commands)
        {
            examine = ReplaceFirst(examine, com, "");
        }

        return examine;
    }

    #endregion

    #region General String Functions

    private string RemoveWhiteSpace(string input)
    {
        return String.Concat(input.Where(c => !Char.IsWhiteSpace(c)));
    }

    private int FindNextNonwhitespaceChar(string input)
    {
        int nonwhitespaceCharacterIndex = -1;
        for (int i = 0; i < input.Length; i++)
        {
            char current = input.ToCharArray()[i];
            if (current != ' ')
            {
                nonwhitespaceCharacterIndex = i;
            }
        }

        if (nonwhitespaceCharacterIndex == -1)
        {
            Debug.LogWarning($"Could not find non-whitespace character in string: {input}");
            return 0;
        }

        return nonwhitespaceCharacterIndex;
    }

    private string GetStringBetweenCharacters(string input, char charFrom, char charTo, int startingIndex = 0, bool inclusive = true)
    {
        int posFrom = input.IndexOf(charFrom, startingIndex);
        if (posFrom != -1) //if found char
        {
            int posTo = input.IndexOf(charTo, posFrom + 1);
            if (posTo != -1) //if found char
            {
                return (inclusive) ? input.Substring(posFrom, posTo - posFrom + 1) : input.Substring(posFrom + 1, posTo - posFrom - 1);
            }
            else
            {
                return input.Substring(posFrom, input.Length - posFrom); //Just return the end of the string
            }
        }

        return string.Empty;
    }

    private string ReplaceFirst(string text, string search, string replace)
    {
        int pos = text.IndexOf(search);
        if (pos < 0)
        {
            return text;
        }
        return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
    }

    #endregion
}
