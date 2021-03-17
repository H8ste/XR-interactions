using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSwipeListener : MonoBehaviour
{
    [SerializeField]
    private Interactable confirm;
    [SerializeField]
    private Interactable cancel;

    
    // Start is called before the first frame update
    void Start()
    {
        //SwipeGesture sG = GameObject.FindGameObjectWithTag("SwipeGesture").GetComponent<SwipeGesture>();
        SwipeGesture.OnSwiped += Swiped;
    }

    private void Swiped(int direction)
    {
        if (confirm != null || cancel != null)
        {
            switch (direction)
            {
                case 0:
                    cancel.SetInputUp();
                    confirm.SetInputDown();
                    break;
                case 1:
                    confirm.SetInputUp();
                    cancel.SetInputDown();
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
