using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PickingHandler : MonoBehaviour
{
    DisplayInformationHandler displayInformationHandler;
    Display currentDisplay;
    Display nextDisplay;
    GameObject displayObject;
    GameObject currentDisplayObject;
    GameObject nextDisplayObject;
    GameObject displayHolder;
    ScanFeedback scanFeedback;
    private PromptHandler promptHandler;
    PromptType promptType;


    private RawImage scanImage;

    void Start()
    {

    }

    void Update()
    {

    }

    public async Task<bool> PickItemHandler(OrderItem activePick, OrderItem nextPick)
    {
        Enable();

        currentDisplay.SetInformation(activePick);
        nextDisplay.SetInformation(nextPick);


        return true;
    }
    public void UpdatePickItems(OrderItem newActivePick, OrderItem newNextPick)
    {
        //Current  is done
        //Set new
        currentDisplay.SetInformation(newActivePick);
        nextDisplay.SetInformation(newNextPick);
    }

    private void Enable()
    {
        promptHandler = promptHandler ?? gameObject.AddComponent<PromptHandler>();

        PrintLabelPrompt();

        displayHolder = (GameObject)(Resources.Load("Prefabs/DisplayHolder"));
        displayObject = Instantiate(displayHolder);

        currentDisplayObject = Helper.FindChildWithTag(displayObject, "CurrentDisplay");
        currentDisplay = currentDisplayObject.GetComponent<Display>();

        nextDisplayObject = Helper.FindChildWithTag(displayObject, "NextDisplay");
        nextDisplay = nextDisplayObject.GetComponent<Display>();

    }
    private void Disable()
    {
        Destroy(displayObject.gameObject);
        Destroy(scanFeedback);
    }

    private void OnScan()
    {
        var correctTex = Resources.Load<Texture>("Images/Correct");
        var inCorrectTex = Resources.Load<Texture>("Images/Incorrect");
        var img = currentDisplayObject.GetComponentInChildren<RawImage>();

        //Check if the scan is correct or not

        //If it is correct
        scanFeedback = new ScanFeedback(true, correctTex, inCorrectTex, img);
        //If it is not correct
        scanFeedback = new ScanFeedback(false, correctTex, inCorrectTex, img);
    }
    private async void OnConfirm()
    {
        //When the user has confirmed the current/active pick. 
        //Check if the correct item has been scanned? 
        var selection = await promptHandler.ShowPrompt(PromptType.ConfirmOrder);
        if (selection == 0)
        {

           
        }
        else
        {
            //Close prompt (handled in prompt handler)
        }

    }
    private async void PrintLabelPrompt()
    {

        var selection = await promptHandler.ShowPrompt(PromptType.PrintOrderLabel);
        print(selection);
        if (selection == 0)
        {
            //printLabel
        }
        else
        {
            //Do not print label
        }
    }

}
