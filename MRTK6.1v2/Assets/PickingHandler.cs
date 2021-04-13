using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PickingHandler : MonoBehaviour, ITab
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

    DataHandler dataHandler;

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

        
    }
    /// <summary>
    /// Starts a new pick and spawns information displays with info corresponding to the current order items
    /// </summary>
    /// <returns></returns>
    public void BeginNewPick()
    {
        promptHandler = promptHandler ?? gameObject.AddComponent<PromptHandler>();
        
        PrintLabelPrompt();
        InstantiateDisplay();

        SetDisplayInfo();

        
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
    /// <summary>
    /// Sets the information on the displays to the current and next orderItem respectively
    /// </summary>
    private void SetDisplayInfo()
    {
        var currentPick = dataHandler.SelectedItem;
        currentDisplay.SetInformation(currentPick);

        //Finds the index of the current orderitem from the DataHandler and sets the other display to the next item in the list.
        nextDisplay.SetInformation(dataHandler.AllOrderItems[dataHandler.SelectedItemIndex +1]);

    }


    /// <summary>
    /// Instantiates the display holder object and the current/next display objects
    /// </summary>
    private void InstantiateDisplay()
    {
        displayObject = Instantiate(displayHolder);

        currentDisplayObject = Helper.FindChildWithTag(displayObject, "CurrentDisplay");
        nextDisplayObject = Helper.FindChildWithTag(displayObject, "NextDisplay");

        currentDisplay = currentDisplayObject.GetComponent<Display>();
        nextDisplay = nextDisplayObject.GetComponent<Display>();
    }

    public ITab Construct(DataHandler dataHandler)
    {
        this.dataHandler = dataHandler;
        displayHolder = (GameObject)(Resources.Load("Prefabs/DisplayHolder"));

        return this;
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
