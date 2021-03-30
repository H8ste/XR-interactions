using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class PromptHandler : MonoBehaviour
{
    Dictionary<PromptType, PromptOption[]> allPromptOptions;
    Dictionary<PromptType, string> allPromptQuestions;
    Prompt promptOption;
    //[SerializeField]
    GameObject promptOptionPrefab;
    GameObject instantiatedPromptObject;
   // ScrollHandler scrollhandler;


    void Start()
    {
        promptOptionPrefab = (GameObject)(Resources.Load("Prefabs/PromptPrefab"));


        ShowPrompt(PromptType.StartManualPick); 
    }

    void Update()
    {
        
    }
   
    
    public PromptHandler()
    {
        allPromptOptions = new Dictionary<PromptType, PromptOption[]>();
        allPromptQuestions = new Dictionary<PromptType, string>();
        foreach (PromptType promptType in (PromptType[])Enum.GetValues(typeof(PromptType)))
        {
            allPromptOptions.Add(promptType, GetPromptOptions(promptType));
            allPromptQuestions.Add(promptType, GetPromptQuestion(promptType));
        }

    }
    public int ShowPrompt(PromptType type)
    {
        instantiatedPromptObject = Instantiate(promptOptionPrefab);
        var promptQuestion = instantiatedPromptObject.GetComponentInChildren<TextMeshPro>();
        
        promptQuestion.text = allPromptQuestions[type];
        print(allPromptQuestions[type]);

        return 0;

    }

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
