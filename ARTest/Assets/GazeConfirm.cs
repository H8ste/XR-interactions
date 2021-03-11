using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class GazeConfirm : MonoBehaviour
{
    public Stopwatch timer;
    Interactable interactable;
    bool timerStarted = false;
    bool inputDown = false;
    // Start is called before the first frame update
    void Start()
    {
        interactable = this.GetComponent<Interactable>();
        timer = new Stopwatch();
    }

    // Update is called once per frame
    void Update()
    {
        if (interactable.HasFocus)
        {
            if (!timerStarted)
            {
                timer.Start();
                timerStarted = true;
            }
            if (timer.ElapsedMilliseconds > 2000 && !inputDown)
            {
                interactable.SetInputDown();
                inputDown = true;              
            }  
           
            
        }
        else if (!interactable.HasFocus)
        {
            timerStarted = false;
            inputDown = false;
            timer.Reset();
        }
    }
}
