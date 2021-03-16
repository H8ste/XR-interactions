using Microsoft.MixedReality.Toolkit.UI;
using System.Diagnostics;
using UnityEngine;

public class GazeConfirm : MonoBehaviour
{
    public Stopwatch timer;
    Interactable interactable;
    bool timerStarted = false;
    bool inputDown = false;
    // Start is called before the first frame update
    void Start()
    {
        FindInteractableComponent(false);
        timer = new Stopwatch();
    }

    private void FindInteractableComponent(bool update = true)
    {
        if (update)
            UnityEngine.Debug.Log("couldn't find interactable, re-referencing");
        interactable = this.GetComponent<Interactable>();
    }

    private bool hasFocus = false;
    private bool triggered = false;

    // Update is called once per frame
    void Update()
    {

        if (hasFocus && !triggered)
        {
            if (timer.ElapsedMilliseconds > 2000)
            {
                triggered = true;
                interactable.SetInputDown();
                UnityEngine.Debug.Log("triggered");
                timer.Stop();
            }
        }

        if (interactable == null)
        {
            FindInteractableComponent();
            return;
        }
    }


    public void OnFocusOn()
    {

        UnityEngine.Debug.Log("start focus");
        hasFocus = true;
        timer = Stopwatch.StartNew();
        timerStarted = true;
    }

    public void OnFocusOff()
    {
        UnityEngine.Debug.Log("end focus");
        hasFocus = false;
        timer.Stop();
        timerStarted = false;


        if (triggered)
        {
            //interactable.SetInputUp();
            triggered = false;
        }

    }
}
