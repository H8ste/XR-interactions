using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateHandler : MonoBehaviour
{
    // Start is called before the first frame update

    DataHandler dataHandler;
    TabHandler tabHandler;
    LoginHandler loginHandler;

    StateType currentState = StateType.Login;
    public StateType CurrentState { get { return currentState; } set { currentState = value; } }

    void Start()
    {
        // instantiate apiHandler

        // instantiate datahandler
        this.dataHandler = new DataHandler( /* apiHandler */);

        // instantiate tabhandler
        this.tabHandler = gameObject.AddComponent<TabHandler>().Instantiate(dataHandler);

        // instantiate loginHandler -- then wait for login
        this.loginHandler = gameObject.AddComponent<LoginHandler>();
        this.loginHandler.Instantiate(this);
        this.loginHandler.LoginEvent.AddListener(OnLogin);
        this.loginHandler.BeginLogin();
    }

    void OnLogin()
    {
        Debug.Log("Logged in!");
        currentState = StateType.Play;

        dataHandler.FetchRoundInfo();

        tabHandler.SwitchTab(TabType.PickingHandler);
    }
}
