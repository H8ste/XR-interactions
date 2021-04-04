using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

// TODO:: Missing functionality that, on prompt option "selection", prompthandler should return which option
// TODO:: was selected to whatever entity started the prompthandler

public class PromptHandler : ScriptableObject
{
    // Dictionary containing the different values for promptoption constructor
    Dictionary<PromptType, PromptOption[]> allPromptOptions = new Dictionary<PromptType, PromptOption[]>();

    // Dictionairy with the different questions connected to the specific prompts
    Dictionary<PromptType, string> allPromptQuestions = new Dictionary<PromptType, string>();

    // Used to load in the necesarry prefab
    GameObject promptQuestionPrefab;
    GameObject promptOptionPrefab;

    // Used to save the current instance of the prompt
    GameObject instantiatedPromptQuestionObject;
    GameObject instantiatedPromptOptionPrefab;

    private ScrollHandler scrollhandler;
    /// <summary>
    /// Constructor for PromptHandler
    /// </summary>
    public PromptHandler()
    {
        promptQuestionPrefab = (GameObject)(Resources.Load("Prefabs/PromptQuestionQuestionPrefab"));
        if (!promptOptionPrefab) Debug.LogError("Prefab PromptQuestionPrefab does not exist");
        promptOptionPrefab = (GameObject)(Resources.Load("Prefabs/PromptOptionPrefab"));
        if (!promptOptionPrefab) Debug.LogError("Prefab PromptOptionPrefab does not exist");

        //Populates the Dictionaries with prompt types and their corresponding keys
        foreach (PromptType promptType in (PromptType[])Enum.GetValues(typeof(PromptType)))
        {
            allPromptOptions.Add(promptType, GetPromptOptions(promptType));
            allPromptQuestions.Add(promptType, GetPromptQuestion(promptType));
        }
    }

    /* Public Methods */

    /// <summary>
    /// Spawns a prompt based on the specified prompt type
    /// </summary>
    /// <param name="type">type of prompt to show</param>
    /// <returns></returns>
    public async Task<int> ShowPrompt(PromptType type)//Function is a WIP
    {
        // Sets the instance of the prompt object to the prefab
        instantiatedPromptQuestionObject = Instantiate(promptQuestionPrefab);
        instantiatedPromptOptionPrefab = Instantiate(promptOptionPrefab);


        if (allPromptQuestions.ContainsKey(type) && allPromptQuestions.TryGetValue(type, out string questionToAsk))
        {
            // Sets the text element of the prefab to the question specified for that type.
            var instantiatedQuestionTextMesh = instantiatedPromptQuestionObject.GetComponentInChildren<TextMeshPro>();
            if (instantiatedQuestionTextMesh) instantiatedQuestionTextMesh.text = questionToAsk;
        }


        if (allPromptOptions.ContainsKey(type))
        {
            PromptOption[] retrievedoptions;
            retrievedoptions = GetPromptOptions(type);
            foreach (var promptOption in retrievedoptions)
            {
                Debug.Log("Setting properties...");

            }





        }
        // TODO:: set the text element of the prefab to options specified for that type

        return 0;
    }

    /* Private Methods */
    private void SetPropertiesOnOptions(PromptOption promptOption)
    {
        var optionText = promptOption.InstatiatedScrollableItem.GetcomponentInChildren<TextMeshPro>();
    }
    /// <summary>
    /// Gets the respective options for the given PromptType
    /// </summary>
    /// <param name="type">the type of promptType to get options for</param>
    /// <returns>A set of PromptOptions for the given PromptType</returns>
    private PromptOption[] GetPromptOptions(PromptType type)
    {
        switch (type)
        {
            // retrieve set of questions/options from database on instantiation of prompthandler
            case PromptType.PrintOrderLabel:
                return new PromptOption[]
                {
                   new PromptOption("text", promptOptionPrefab, null, 0),
                   new PromptOption("text", promptOptionPrefab, null, 0)
                };
            case PromptType.StartManualPick:
                return new PromptOption[]
                {
                   new PromptOption("text", promptOptionPrefab, null, 0),
                   new PromptOption("text", promptOptionPrefab, null, 0)
                };

            default:
                throw new Exception("Tried to retrieve options for an unhandled PromptType");
                break;
        }
        return null;
    }

    /// <summary>
    /// Gets the respective question for the given PromptType
    /// </summary>
    /// <param name="type">the type of promptType to get question for</param>
    /// <returns>A string (question) for the given PromptType</returns>
    private string GetPromptQuestion(PromptType type)
    {
        switch (type)
        {
            // retrieve set of questions/options from database on instantiation of prompthandler
            case PromptType.PrintOrderLabel:
                return "Would you like to print the order label?";
            case PromptType.StartManualPick:
                return "Print label manually?";

            default:
                throw new Exception("Tried to retrieve question for an unhandled PromptType");
                return null;
        }
    }
}
