using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickingHandler: MonoBehaviour
{
    DisplayInformationHandler displayInformationHandler;
    Display currentDisplay;
    Display nextDisplay;
    OrderItem current;
    OrderItem next;
    GameObject displayObject;
    GameObject currentDisplayObject;
    GameObject nextDisplayObject;
    
    //ScanFeedback scanFeedback;
    //PromptHandler prompt;
    //PromptType promptType;
    
    //TODO: Merge with prompt code
    //TODO: Make sure the next item is placed correctly

    void Start()
    {
        current = new OrderItem(1,2,"test", 123, false);
        next = new OrderItem(4,5,"test2", 678, false);

        PickItemHandler(current, next);
    }

    void Update()
    {
            
    }

    public bool PickItemHandler(OrderItem activePick, OrderItem nextPick)
    {
        var displayHolderPrefab = (GameObject)(Resources.Load("Prefabs/DisplayHolder"));

        displayObject = Instantiate(displayHolderPrefab);

        currentDisplayObject = Helper.FindChildWithTag(displayObject, "CurrentDisplay");
        currentDisplay = currentDisplayObject.GetComponent<Display>();
        currentDisplay.SetInformation(activePick);

        currentDisplayObject = Helper.FindChildWithTag(displayObject, "NextDisplay");
        nextDisplay = nextDisplayObject.GetComponent<Display>();
        nextDisplay.SetInformation(activePick);


        return true;
    }
    
    private void Enable()
    {
          

    }
    private void Disable()
    {
        
    }
    private void OnScan()
    {

    }
    private void onConfirm()
    {

    }
    private void PrintLabelPrompt()
    {

    }

}
