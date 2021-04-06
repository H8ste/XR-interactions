using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptTester : MonoBehaviour
{
    private PromptHandler promptHandler;

    void Start()
    {
        promptHandler = gameObject.AddComponent<PromptHandler>();
        promptHandler.ShowPrompt(PromptType.StartManualPick);
        
    }


    void Update()
    {
        
    }
}
