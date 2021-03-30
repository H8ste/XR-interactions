using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class PromptHandler : MonoBehaviour
{
    //Dictionary containing the different values for promptoption constructor
    Dictionary<PromptType, PromptOption[]> allPromptOptions;
    //Dictionairy with the different questions connected to the specific prompts
    Dictionary<PromptType, string> allPromptQuestions;
    Prompt promptOption;
    //Used to load in the necesarry prefab
    GameObject promptOptionPrefab;
    //Used to save the current instance of the prompt
    GameObject instantiatedPromptObject;



    void Start()
    {
        //Prefab is loaded 
        promptOptionPrefab = (GameObject)(Resources.Load("Prefabs/PromptPrefab"));


        ShowPrompt(PromptType.StartManualPick); 
    }

    void Update()
    {
        
    }
   
    /// <summary>
    /// Constructor for PromptHanlder
    /// </summary>
    public PromptHandler()
    {
        
        allPromptOptions = new Dictionary<PromptType, PromptOption[]>();
        allPromptQuestions = new Dictionary<PromptType, string>();
        //Populates the Dictionaries with prompt types and their corresponding keys
        foreach (PromptType promptType in (PromptType[])Enum.GetValues(typeof(PromptType)))
        {
            allPromptOptions.Add(promptType, GetPromptOptions(promptType));
            allPromptQuestions.Add(promptType, GetPromptQuestion(promptType));
        }

    }
    /// <summary>
    /// Spawns a prompt based on the specified prompt type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public int ShowPrompt(PromptType type)//Function is a WIP
    {
        //Sets the instance of the prompt object to the prefab
        instantiatedPromptObject = Instantiate(promptOptionPrefab);
        //Accesses the text element of the prefab
        var promptQuestion = instantiatedPromptObject.GetComponentInChildren<TextMeshPro>();
        //Sets the text element of the prefab to the question specified for that type.
        promptQuestion.text = allPromptQuestions[type];
        

        return 0;

    }
    /// <summary>
    /// Based on given promptype returns series of prompt objects
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private PromptOption[] GetPromptOptions(PromptType type)
    {
        switch (type)
        {
            case PromptType.PrintOrderLabel:
                return new PromptOption[]
               {
                   new PromptOption("text", promptOptionPrefab, null, 0),
                   new PromptOption("text", promptOptionPrefab, null, 0)
                };
                break;
            case PromptType.StartManualPick:
                return new PromptOption[]
               {
                   new PromptOption("text", promptOptionPrefab, null, 0),
                   new PromptOption("text", promptOptionPrefab, null, 0)
                };
                break;
            default:
                break;
        }
        return  null;
    }
    /// <summary>
    /// /// Based on given promptype returns series of prompt question
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private string GetPromptQuestion(PromptType type)
    {
        switch (type)
        {
            case PromptType.PrintOrderLabel:
                return "Would you like to print the order label?";
            case PromptType.StartManualPick:
                return "Print label manually?";

            default:
                throw new Exception("No question found for type, maybe a definition is missing");
                return null;
        }

    }
}
