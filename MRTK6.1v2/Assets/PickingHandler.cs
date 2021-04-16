using UnityEngine;
using UnityEngine.UI;

/* SUBJECT TO CHANGE  -  DON'T READ :)*/
public class PickingHandler : MonoBehaviour, ITab
{
    DisplayInformationHandler displayInformationHandler;
    Display currentDisplay;
    Display nextDisplay;
    GameObject displayObject;
    GameObject displayPrefab;
    ScanFeedback scanFeedback;
    private PromptHandler promptHandler;
    PromptType promptType;
    OrderItem current;
    OrderItem next;

    private RawImage scanImage;

    DataHandler dataHandler;

    /* CTor */
    public ITab Instantiate(DataHandler dataHandler)
    {
        this.dataHandler = dataHandler;
        displayPrefab = (GameObject)(Resources.Load("Prefabs/DisplayHolder"));

        return this;
    }

    /// <summary>
    /// Called when the picking handler tab is switched to. 
    /// </summary>
    public void Enable()
    {
        BeginNewPick();
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
    /// Starts a new pick and spawns information displays with info corresponding to the current order items
    /// </summary>
    /// <returns></returns>
    public void BeginNewPick()
    {
        promptHandler = promptHandler ?? gameObject.AddComponent<PromptHandler>().Instantiate(PromptType.PrintOrderLabel);
        promptHandler.OnResponse.AddListener((responseValue) =>
        {
            if (responseValue == 0)
            {
                //print label
            }

            InstantiateDisplay();
            Destroy(promptHandler);
        });
    }

    /// <summary>
    /// Instantiates the display holder object and the current/next display objects
    /// </summary>
    private void InstantiateDisplay()
    {
        displayObject = displayObject ?? Instantiate(displayPrefab);

        if (Helper.FindChildWithTag(displayObject, "CurrentDisplay", out var currentDisplayObject) && Helper.FindChildWithTag(displayObject, "NextDisplay", out var nextDisplayObject))
        {
            currentDisplay = currentDisplayObject.GetComponent<Display>();
            nextDisplay = nextDisplayObject.GetComponent<Display>();

            SetDisplayInfo();
        }
    }

    /// <summary>
    /// Sets the information on the displays to the current and next orderItem respectively
    /// </summary>
    private void SetDisplayInfo()
    {
        var currentPick = dataHandler.SelectedItem;
        currentDisplay.SetInformation(currentPick);

        //Finds the index of the current orderitem from the DataHandler and sets the other display to the next item in the list.
        nextDisplay.SetInformation(dataHandler.AllOrderItems[dataHandler.SelectedItemIndex + 1]);

    }

    private void OnScan()
    {
        var correctTex = Resources.Load<Texture>("Images/Correct");
        var inCorrectTex = Resources.Load<Texture>("Images/Incorrect");
        var img = currentDisplay.GetComponentInChildren<RawImage>();

        //Check if the scan is correct or not

        //If it is correct
        scanFeedback = new ScanFeedback(true, correctTex, inCorrectTex, img);
        //If it is not correct
        scanFeedback = new ScanFeedback(false, correctTex, inCorrectTex, img);
    }

    private void OnConfirm()
    {
        //When the user has confirmed the current/active pick. 
        //Check if the correct item has been scanned?
        // var selection = await promptHandler.ShowPrompt(PromptType.ConfirmOrder);
        // if (promptHaOnResponseselection == 0)
        // {

        //     //SetDisplayInfo
        // }
        // else
        // {
        //     //Close prompt (handled in prompt handler)
        // }

    }
}
