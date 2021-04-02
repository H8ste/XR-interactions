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
    //ScanFeedback scanFeedback;
    //PromptHandler prompt;
    //PromptType promptType;
    

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
        var displayPrefab = (GameObject)(Resources.Load("Prefabs/InformationDisplay"));

        currentDisplay = Instantiate(displayPrefab).GetComponent<Display>();
        currentDisplay.SetInformation(activePick);

        nextDisplay = Instantiate(displayPrefab).GetComponent<Display>();
        nextDisplay.SetInformation(nextPick);

        

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
