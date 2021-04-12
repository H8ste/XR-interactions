using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateHandler : MonoBehaviour
{
    // Start is called before the first frame update

    DataHandler dataHandler;
    TabHandler tabHandler;

    StateType currentState = StateType.Login;

    void Start()
    {
        // instantiate apiHandler

        // instantiate datahandler
        this.dataHandler = new DataHandler( /* apiHandler */);

        // instantiate tabhandler
        this.tabHandler = gameObject.AddComponent<TabHandler>().Instantiate(dataHandler);

        // instantiate loginHandler -- then wait for login


    }

    void OnLogin()
    {
        currentState = StateType.Play;

        dataHandler.FetchRoundInfo();

        currentState = StateType.Play;

        tabHandler.SwitchTab(TabType.PickingHandler);
    }
}
