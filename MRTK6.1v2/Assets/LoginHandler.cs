using System.Linq;
using UnityEngine;

public class LoginHandler : MonoBehaviour
{
    QRProvider qrProvider;
    StateHandler stateHandler;
    
    OnLogin loginEvent = new OnLogin();
    public OnLogin LoginEvent { get { return loginEvent; } }
    

    /* CTor */
    public LoginHandler Instantiate(StateHandler stateHandler)
    {
        this.stateHandler = stateHandler;

        return this;
    }


    /* Unity Methods */
    void Update()
    {
        if (stateHandler?.CurrentState == StateType.Login)
        {
            if (qrProvider.QrCodesAsStrings != null && qrProvider.QrCodesAsStrings.Any() && IsValidLogin(qrProvider.QrCodesAsStrings.First()))
            {
                // login was valid
                EndLogin();

                loginEvent.Invoke();
            }
        }
    }


    /* Public Methods */
    public void BeginLogin()
    {
        // begin cameraAcess etc.
        qrProvider = qrProvider ?? gameObject.AddComponent<QRProvider>().Instantiate();
    }


    /* Private Methods */
    private bool IsValidLogin(string parsedQRCode)
    {
        if (parsedQRCode.Contains('@'))
        {
            var splitQRCode = parsedQRCode.Split('@');
            if (splitQRCode.Length == 2)
            {
                if (splitQRCode[0].Contains('/') && short.TryParse(splitQRCode[1], out short roundNo))
                {
                    Debug.Log("user: " + splitQRCode[0] + ". roundNo: " + roundNo);

                    // should also, through apiHandler, ensure correct login
                    return true;
                }
            }
        }

        return false;
    }

    private void EndLogin()
    {
        // end qrProvider etc.
        Destroy(qrProvider);
        qrProvider = null;
    }
}
