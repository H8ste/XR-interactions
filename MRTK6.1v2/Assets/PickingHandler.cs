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
    OrderItem current;
    OrderItem next;

    private RawImage scanImage;

    void Start()
    {

    }

    void Update()
    {

    }
    /// <summary>
    /// Called when the picking handler tab is switched to. 
    /// </summary>
    public void Enable()
    {

        BeginNewPick();

        InstantiateDisplay();

        

    }
    /// <summary>
    /// Starts a new pick and spawns information displays with info corresponding to the current order items
    /// </summary>
    /// <returns></returns>
    public async Task<bool> BeginNewPick()
    {
        promptHandler = promptHandler ?? gameObject.AddComponent<PromptHandler>();

        PrintLabelPrompt();
        InstantiateDisplay();

        //Debugging orderItems.
        current = new OrderItem(null, new LocPK(40, 10, 10, 10, 10, "X"), 0, 33, 1, "Pumpe", 254, false);
        next = new OrderItem(null, new LocPK(50, 20, 12, 10, 10, "L"), 1, 110, 9, "Cencor", 251, true);

        SetDisplayInfo(current, next);
        
        return true;
    }
    private void SetDisplayInfo(OrderItem ActivePick, OrderItem NextPick)
    {
        //Talk to DataHandler here to get the relevant info. 
        currentDisplay.SetInformation(ActivePick);
        nextDisplay.SetInformation(NextPick);
    }

  
    /// <summary>
    /// Instantiates the display holder object and the current/next display objects
    /// </summary>
    private void InstantiateDisplay()
    {
        displayHolder = (GameObject)(Resources.Load("Prefabs/DisplayHolder"));
        displayObject = Instantiate(displayHolder);

        currentDisplayObject = Helper.FindChildWithTag(displayObject, "CurrentDisplay");
        currentDisplay = currentDisplayObject.GetComponent<Display>();

        nextDisplayObject = Helper.FindChildWithTag(displayObject, "NextDisplay");
        nextDisplay = nextDisplayObject.GetComponent<Display>();
    }
    /// <summary>
    /// Called when the picking handler tab is switched away from
    /// </summary>
    public void Disable()
    {
        Destroy(displayObject.gameObject);
        Destroy(scanFeedback);
        Destroy(promptHandler);
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

            //SetDisplayInfo
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
