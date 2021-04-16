using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PromptHandler : MonoBehaviour
{
    // Dictionary containing the different values for promptoption constructor
    Dictionary<PromptType, PromptOption[]> allPromptOptions = new Dictionary<PromptType, PromptOption[]>();

    // Dictionairy with the different questions connected to the specific prompts
    Dictionary<PromptType, string> allPromptQuestions = new Dictionary<PromptType, string>();

    // Used to load in the necesarry prefab
    GameObject promptQuestionPrefab;
    GameObject promptOptionPrefab;

    // Used to save the current instance of the prompt question prefab
    GameObject instantiatedPromptQuestionObject;

    private ScrollHandler scrollHandler;

    //Array used to store the prompt options for the specified type
    private PromptOption[] retrievedOptions;

    public OnClick OnResponse = new OnClick();

    /* CTor */
    public PromptHandler Instantiate(PromptType type)
    {
        allPromptOptions.Clear(); allPromptQuestions.Clear();
        //Populates the Dictionaries with prompt types and their corresponding keys
        foreach (PromptType promptType in (PromptType[])Enum.GetValues(typeof(PromptType)))
        {
            allPromptOptions.Add(promptType, GetPromptOptions(promptType));
            allPromptQuestions.Add(promptType, GetPromptQuestion(promptType));
        }

        /// Loads the prompt option prefab and saves it in a variable
        promptOptionPrefab = Resources.Load("Prefabs/PromptOptionPrefab", typeof(GameObject)) as GameObject;
        if (!promptOptionPrefab) Debug.LogError("Prefab PromptOptionPrefab does not exist!");

        // Sets the instance of the prompt object to the prefab
        instantiatedPromptQuestionObject = instantiatedPromptQuestionObject ?? Instantiate(promptQuestionPrefab);

        //Checks if any of the questions in the library match the specified prompt type, and sets the text on the question prefab accordingly
        if (allPromptQuestions.ContainsKey(type) && allPromptQuestions.TryGetValue(type, out string questionToAsk))
        {
            var instantiatedQuestionTextMesh = instantiatedPromptQuestionObject.GetComponentInChildren<TextMeshPro>();
            if (instantiatedQuestionTextMesh) instantiatedQuestionTextMesh.text = questionToAsk;
        }

        //If any of the promptOption contain the specified key, save those promptoptions to this array. 
        if (allPromptOptions.ContainsKey(type))
        {
            retrievedOptions = GetPromptOptions(type);
            foreach (var item in retrievedOptions)
            {
                item.SwitchPrefab(promptOptionPrefab);
            }
        }

        StartScroll();

        return this;
    }


    /* Private Methods */

    /// <summary>
    ///  Instantiates a scrollHandler, initialises it with promptOptions, sets properties on instantiated prefabs, and subscribes to the scrollHandler's ItemSelected event
    /// </summary>
    private void StartScroll()
    {
        scrollHandler = scrollHandler ??
            Instantiate((Resources.Load("Prefabs/ScrollHandler", typeof(GameObject)) as GameObject), transform).GetComponent<ScrollHandler>().Instantiate(retrievedOptions);

        if (!scrollHandler) Debug.LogError("Cant find scrollHandler Prefab");

        foreach (var option in retrievedOptions)
        {
            SetPropertiesOnOptions(option);
        }

        //Ensures the Question gameobject has the same distance from the camera as the scrollable items.
        var questionDistance = instantiatedPromptQuestionObject.GetComponent<RadialView>();
        questionDistance.MinDistance = scrollHandler.Radius;
        questionDistance.MaxDistance = scrollHandler.Radius;

        //Returns the selected option, then destroys the ScrollHandler and promptQuestion
        scrollHandler.ItemSelected.AddListener((int returnVar) =>
        {
            Debug.Log("Option selected: " + returnVar);
            OnResponse.Invoke(returnVar);
            DestroyScrollHandler();
        });
    }

    /// <summary>
    /// Sets text value on specified instantiated promp Object
    /// </summary>
    /// <param name="promptOption">The prompt option on which to set parameters</param>
    private void SetPropertiesOnOptions(PromptOption promptOption)
    {
        if (promptOption == null)
            return;

        if (promptOption.OptionText != null && Helper.FindChildWithTag(promptOption.InstantiatedScrollableItem, "OptionText", out var optionText))
            optionText.GetComponent<TextMeshPro>()?.SetText(promptOption.OptionText, false);
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
                   new PromptOption("Bekræft", promptOptionPrefab, null, 0),
                   new PromptOption("Fortryd", promptOptionPrefab, null, 1),
                };
            case PromptType.StartManualPick:
                return new PromptOption[]
                {
                   new PromptOption("text 1", promptOptionPrefab, null, 0),
                   new PromptOption("text 2" , promptOptionPrefab, null, 1),
                   new PromptOption("text 3", promptOptionPrefab, null, 2),
                   new PromptOption("text 4", promptOptionPrefab, null, 3),
                };
            case PromptType.ConfirmOrder:
                return new PromptOption[]
                {
                   new PromptOption("Bekræft", promptOptionPrefab, null, 0),
                   new PromptOption("Fortryd", promptOptionPrefab, null, 1),
                };
            default:
                return null;
                // throw new Exception("Tried to retrieve options for an unhandled PromptType");
        }
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
                return "print order label?";
            case PromptType.StartManualPick:
                return "Print label?";
            case PromptType.ConfirmOrder:
                return "Bekræft odre?";
            default:
                return null;
                // throw new Exception("Tried to retrieve question for an unhandled PromptType");
        }
    }

    /// <summary>
    /// Called when an option has been chosen, destroys all gameobject related to the prompt.
    /// </summary>
    private void DestroyScrollHandler()
    {
        print("Destroying ScrollHandler");
        Destroy(scrollHandler.gameObject);
        Destroy(instantiatedPromptQuestionObject);
    }
}
