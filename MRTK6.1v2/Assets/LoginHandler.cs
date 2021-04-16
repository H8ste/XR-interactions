using System.Linq;
using UnityEngine;

public class LoginHandler : MonoBehaviour
{

    OnLogin loginEvent = new OnLogin();
    CameraAccess qrHandler;
    StateHandler stateHandler;

    public OnLogin LoginEvent { get { return loginEvent; } }

    public LoginHandler Instantiate(StateHandler stateHandler)
    {
        this.stateHandler = stateHandler;

        return this;
    }

    // Update is called once per frame
    void Update()
    {
        if (stateHandler?.CurrentState == StateType.Login)
        {
            if (qrHandler.Result != null && qrHandler.Result.Any() && IsValidLogin(qrHandler.Result.First()))
            {
                // login was valid

                EndLogin();

                loginEvent.Invoke();
            }
        }
    }

    private bool IsValidLogin(string parsedQRCode)
    {
        if (parsedQRCode.Contains('@'))
        {
            var splitQRCode = parsedQRCode.Split('@');
            if (splitQRCode.Length == 2)
            {
                if (splitQRCode[0].Contains('/') && short.TryParse(splitQRCode[1],out short roundNo))
                {
                    Debug.Log("user: " + splitQRCode[0] + ". roundNo: " + roundNo);
                    return true;
                }
            }
        }

        return false;
      
    }

    public void BeginLogin()
    {
        // begin cameraAcess etc.
        qrHandler = qrHandler ?? gameObject.AddComponent<CameraAccess>();
    }

    private void EndLogin()
    {
        Debug.Log("Ending login");
        // end cameraAcess etc.
        Destroy(qrHandler);
        qrHandler = null;
    }




}
