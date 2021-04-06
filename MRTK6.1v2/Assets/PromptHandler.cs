using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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


    CancellationTokenSource cancellationTokenSource;

    private ScrollHandler scrollHandler;
    //Array used to store the prompt options for the specified type
    private PromptOption[] retrievedOptions;

    private int optionID = -1;
    private bool OptionHasBeenSelected { get { return optionID != -1; } }

    /// <summary>
    /// Constructor for PromptHandler
    /// </summary>
    public PromptHandler()
    {

    }

    private void Awake()
    {

        promptQuestionPrefab = (GameObject)(Resources.Load("Prefabs/PromptQuestionPrefab"));
        if (!promptQuestionPrefab) Debug.LogError("Prefab PromptQuestionPrefab does not exist");


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
    public async Task<int> ShowPrompt(PromptType type)
    {
        // Sets the instance of the prompt object to the prefab
        instantiatedPromptQuestionObject = Instantiate(promptQuestionPrefab);

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
        }

        Enable();
                
        cancellationTokenSource = new CancellationTokenSource();
        return await WaitForOptionSelect(cancellationTokenSource);


    }

    /* Private Methods */
    /// <summary>
    /// Sets the prompoption to the correct prefab for each option. 
    /// </summary>
    private void Enable()
    {
        /// Loads the prompt option prefab and saves it in a variable
        promptOptionPrefab = Resources.Load("Prefabs/PromptOptionPrefab", typeof(GameObject)) as GameObject;
        if (!promptOptionPrefab) Debug.LogError("Prefab PromptOptionPrefab does not exist!");

        foreach (var item in retrievedOptions)
        {
            item.SwitchPrefab(promptOptionPrefab);
        }
        StartScroll();
    }

    /// <summary>
    ///  Instantiates a scrollHandler, initialises it with promptOptions, sets properties on instantiated prefabs, and subscribes to the scrollHandler's ItemSelected event
    /// </summary>
    private void StartScroll()
    {
        scrollHandler = Instantiate((Resources.Load("Prefabs/ScrollHandler", typeof(GameObject)) as GameObject), transform).GetComponent<ScrollHandler>();
        if (!scrollHandler) Debug.LogError("Cant find scrollHandler Prefab");

        scrollHandler.Init(retrievedOptions);

        foreach (var option in retrievedOptions)
        {
            SetPropertiesOnOptions(option);
        }
        //Returns the selected option, then destroys the ScrollHandler and promptQuestion
        scrollHandler.ItemSelected.AddListener((int returnVar) =>
        {
            Debug.Log("Option selected: " + returnVar);
            optionID = returnVar;
            DestroyScrollHanlder();
        });

        //Ensures the Question gameobject has the same distance from the camera as the scrollable items.
        var questionDistance = instantiatedPromptQuestionObject.GetComponent<RadialView>();
        questionDistance.MinDistance = scrollHandler.Radius;
        questionDistance.MaxDistance = scrollHandler.Radius;
    }

    private async Task<int> WaitForOptionSelect(CancellationTokenSource cts)
    {
        while (!OptionHasBeenSelected)
        {
            // if at any point cancellation is requested
            if (cts.IsCancellationRequested)
            {
                return -1;
            }

            await Task.Delay(TimeSpan.FromSeconds(1));
            Debug.Log("Still waiting");
        }
        // An promptOption has been selected
        return optionID;
    }
    /// <summary>
    /// Sets text value on specified instantiated promp Object
    /// </summary>
    /// <param name="promptOption">The prompt option on which to set parameters</param>
    private void SetPropertiesOnOptions(PromptOption promptOption)
    {
        if (promptOption == null)
            return;

        var optionText = Helper.FindChildWithTag(promptOption.InstantiatedScrollableItem, "OptionText")?.GetComponent<TextMeshPro>();
        if (optionText != null && promptOption.OptionText != null)
            optionText.SetText(promptOption.OptionText, false);
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
                   new PromptOption("Fortryd", promptOptionPrefab, null, 0),
                };
            case PromptType.StartManualPick:
                return new PromptOption[]
                {
                   new PromptOption("text 1", promptOptionPrefab, null, 0),
                   new PromptOption("text 2" , promptOptionPrefab, null, 0),
                   new PromptOption("text 3", promptOptionPrefab, null, 0),
                   new PromptOption("text 4", promptOptionPrefab, null, 0),
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
    /// <summary>
    /// Called when an option has been chosen, destroys all gameobject related to the prompt.
    /// </summary>
    private void DestroyScrollHanlder()
    {
        print("Destroying ScrollHandler");
        Destroy(scrollHandler.gameObject);
        Destroy(instantiatedPromptQuestionObject);
    }

}
