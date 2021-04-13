using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TabHandler : MonoBehaviour
{
    // a dictionary with references to all instantiated states
    // - Each TabType has a ITab reference
    private Dictionary<TabType, ITab> allTabs = new Dictionary<TabType, ITab>();
    Dictionary<TabType, ITab> AllTabs { get { return allTabs; } }

    // variable used to denote the active state
    // used to show correct state in hololens
    private ITab activeTab;
    ITab ActiveTab
    {
        get { return activeTab; }
        set
        {
            previousTab = activeTab;
            activeTab = value;
        }
    }

    // variable used to denote the previous state the system was in
    // used for logic such as "back" functionality
    private ITab previousTab;

    // variable denoting the beginning state
    private TabType beginningTab = TabType.PickingHandler;

    private DataHandler dataHandler;

    /* CTor */
    public TabHandler Instantiate(DataHandler dataHandler)
    {
        this.dataHandler = dataHandler;

        // instantiate all possible tabs
        foreach (var TabType in (TabType[])Enum.GetValues(typeof(TabType)))
        {
            AllTabs.Add(TabType, InstantiateAndReturnTab(TabType));
        }

        if (EditorApplication.isPlaying) PrintDictionary(AllTabs);

        //StartTab(beginningTab);

        return this;
    }

    /* Public Methods */
    /// <summary>
    /// Used to start given TabType
    /// </summary>
    /// <param name="TabTypeToStart">the type of state to start</param>
    public void StartTab(TabType TabTypeToStart)
    {
        if (AllTabs.ContainsKey(TabTypeToStart) && AllTabs.TryGetValue(TabTypeToStart, out ITab tabToStart))
        {
            SwitchTab(tabToStart);
        }
    }

    /* Private Methods */

    /// <summary>
    /// Used to handle the enabling/disabling of new and previous states
    /// </summary>
    /// <param name="newTab">the state to switch to</param>
    private void SwitchTab(ITab newTab)
    {
        // disable previous state
        previousTab?.Disable();

        // set new state as active state
        ActiveTab = newTab;

        // enable active state
        ActiveTab?.Enable();
    }

    public void SwitchTab(TabType newTabType)
    {
        if (AllTabs.ContainsKey(newTabType) && AllTabs.TryGetValue(newTabType, out ITab newTab))
        {
            SwitchTab(newTab);
        }
    }

    public void PreviousTab()
    {
        if (previousTab != null)
        {
            ActiveTab?.Disable();

            ActiveTab = previousTab;

            ActiveTab?.Enable();
        }
    }


    /// <summary>
    /// Method used to instantiate and set dictionary containing possible states
    /// </summary>
    /// <param name="type">the type of state to instantiate</param>
    private ITab InstantiateAndReturnTab(TabType type)
    {
        switch (type)
        {
            case TabType.PickingHandler:
                return gameObject.AddComponent<PickingHandler>().Construct(dataHandler);

            case TabType.ManualOrderPickingHandler:
                return gameObject.AddComponent<ManualOrderPickHandler>().Construct(dataHandler);

            case TabType.OffloadHandler:
                //throw new Exception("Appropriate tabHandler for OffloadHandler has not been set in TabHandler.cs");

                return null;
                break;

            case TabType.RestockHandler:
                //throw new Exception("Appropriate tabHandler for RestockHandler has not been set in TabHandler.cs");

                return null;
                break;

            default:
                Debug.LogError("TabType: " + type + " does not have an appropriate InstantiateAndReturnTab call");
                return null;
        }
    }

    /// <summary>
    /// Debugging method to print contents of state-dictionary
    /// </summary>
    /// <param name="dict">the dictionary to print contents off</param>
    private void PrintDictionary(Dictionary<TabType, ITab> dict)
    {
        foreach (KeyValuePair<TabType, ITab> kvp in dict)
        {
            Debug.Log(kvp.Key + ": " + kvp.Value);
        }
    }
}
