using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
[RequireComponent(typeof(Rigidbody))]
public class ConfirmCancelInteraction : MonoBehaviour, IMixedRealityTouchHandler
{

    #region Event handlers
    //public TouchEvent OnTouchStarted = new TouchEvent();
    //public TouchEvent OnTouchCompleted = new TouchEvent();
    //public TouchEvent OnTouchUpdated = new TouchEvent();
    #endregion

    [SerializeField]
    string collision_tag = "IndexFingerCollider";


    Interactable interactable;
    public void Start()
    {
        interactable = this.gameObject.GetComponent<Interactable>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == collision_tag)
        {
            Debug.Log("touch started");
            //interactable.SetToggled(true);
            interactable.SetInputDown();
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.tag == collision_tag)
        {
            Debug.Log("touch completed");

            interactable.SetInputUp();
        }
    }

    private void OnTriggerStay(Collider collision)
    {

    }


    void IMixedRealityTouchHandler.OnTouchCompleted(HandTrackingInputEventData eventData)
    {
        Debug.Log("touch completed");
        //OnTouchCompleted.Invoke(eventData);
    }

    void IMixedRealityTouchHandler.OnTouchStarted(HandTrackingInputEventData eventData)
    {
        Debug.Log("touch started");
        //OnTouchStarted.Invoke(eventData);
    }

    void IMixedRealityTouchHandler.OnTouchUpdated(HandTrackingInputEventData eventData)
    {
        //OnTouchUpdated.Invoke(eventData);
    }
}
