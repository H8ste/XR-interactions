using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

public class StateHandler : MonoBehaviour
{
    // a dictionary with references to all instantiated states
    // - Each StateType has a IState reference
    private Dictionary<StateType, IState> allStates = new Dictionary<StateType, IState>();
    Dictionary<StateType, IState> AllStates { get { return allStates; } }

    // variable used to denote the active state
    // used to show correct state in hololens
    private IState activeState;
    IState ActiveState
    {
        get { return activeState; }
        set
        {
            previousState = activeState;
            activeState = value;
        }
    }

    // variable used to denote the previous state the system was in
    // used for logic such as "back" functionality
    private IState previousState;
    IState PreviousState { get { return previousState; } }

    // variable containing all order items within order
    private OrderItem[] orderItems;
    OrderItem[] OrderItems { get { return orderItems; } set { orderItems = value; } }

    // // variable used to denote which order item is being processed
    // private int currentIndex;
    // int CurrentIndex { get { return currentIndex; } set { currentIndex = value; } }

    // variable denoting the beginning state
    private StateType beginningState = StateType.PickingHandler;


    /* Unity Methods */
    void Start()
    {
        foreach (var stateType in (StateType[])Enum.GetValues(typeof(StateType)))
        {
            AllStates.Add(stateType, InstantiateAndReturnState(stateType));
        }

        if (EditorApplication.isPlaying) PrintDictionary(AllStates);

        StartState(beginningState);
    }

    /* Public Methods */
    /// <summary>
    /// Used to start given stateType
    /// </summary>
    /// <param name="stateTypeToStart">the type of state to start</param>
    public void StartState(StateType stateTypeToStart)
    {
        if (AllStates.ContainsKey(stateTypeToStart) && AllStates.TryGetValue(stateTypeToStart, out IState stateToStart))
        {
            SwitchState(stateToStart);
        }
    }

    /* Private Methods */

    /// <summary>
    /// Used to handle the enabling/disabling of new and previous states
    /// </summary>
    /// <param name="newState">the state to switch to</param>
    private void SwitchState(IState newState)
    {
        // set active state as previous
        previousState = activeState;
        // disable previous state
        previousState?.Disable();

        // set new state as active state
        activeState = newState;
        // enable active state
        activeState?.Enable();
    }

    /// <summary>
    /// Method used to instantiate and set dictionary containing possible states
    /// </summary>
    /// <param name="type">the type of state to instantiate</param>
    private IState InstantiateAndReturnState(StateType type)
    {
        switch (type)
        {
            case StateType.PickingHandler:
                throw new Exception("Appropriate stateHandler for PickingHandler has not been set in StateHandler.cs");
                // new state() 
                // state.Instantiate(); -- maybe put this in constructor for each state
                return null;
                break;

            case StateType.ManualOrderPickingHandler:
                throw new Exception("Appropriate stateHandler for ManualOrderPickingHandler has not been set in StateHandler.cs");

                return null;
                break;

            case StateType.OffloadHandler:
                throw new Exception("Appropriate stateHandler for OffloadHandler has not been set in StateHandler.cs");

                return null;
                break;

            case StateType.RestockHandler:
                throw new Exception("Appropriate stateHandler for RestockHandler has not been set in StateHandler.cs");

                return null;
                break;

            default:
                Debug.LogError("Statetype: " + type + " does not have an appropriate InstantiateAndReturnState call");
                return null;
        }
    }

    /// <summary>
    /// Debugging method to print contents of state-dictionary
    /// </summary>
    /// <param name="dict">the dictionary to print contents off</param>
    private void PrintDictionary(Dictionary<StateType, IState> dict)
    {
        foreach (KeyValuePair<StateType, IState> kvp in dict)
        {
            print(kvp.Key + ": " + kvp.Value);
        }
    }
}
